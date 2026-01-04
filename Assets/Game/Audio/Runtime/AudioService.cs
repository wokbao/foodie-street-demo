using System;
using System.Collections.Generic;
using System.Threading;
using Core.Feature.AssetManagement.Runtime;
using Core.Feature.Logging.Abstractions;
using Core.Feature.ObjectPooling.Abstractions;
using Cysharp.Threading.Tasks;
using Game.Audio.Runtime.Abstractions;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Game.Audio.Runtime
{
    /// <summary>
    /// 音频服务实现
    /// 
    /// <para><b>职责</b>：</para>
    /// <list type="bullet">
    ///   <item>管理 BGM 和 SFX 的播放</item>
    ///   <item>AudioSource 池化复用</item>
    ///   <item>分通道音量控制</item>
    ///   <item>淡入淡出效果</item>
    /// </list>
    /// 
    /// <para><b>依赖</b>：</para>
    /// <list type="bullet">
    ///   <item>IAssetProvider：加载 AudioClip</item>
    ///   <item>IObjectPoolManager：AudioSource 池化</item>
    ///   <item>ILogService：日志记录</item>
    ///   <item>AudioConfig：配置参数</item>
    /// </list>
    /// </summary>
    public class AudioService : IAudioService, IStartable, IDisposable
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IObjectPoolManager _poolManager;
        private readonly ILogService _logService;
        private readonly AudioConfig _config;

        // 音量状态
        private readonly Dictionary<AudioChannel, float> _volumes = new();
        private readonly Dictionary<AudioChannel, ReactiveProperty<float>> _volumeSubjects = new();

        // 当前播放的 BGM
        private AudioHandle _currentBGM;

        // 活跃的音效句柄
        private readonly List<AudioHandle> _activeHandles = new();

        // AudioSource 根节点
        private GameObject _audioRoot;

        // 暂停状态
        private bool _isPaused;

        // 取消令牌
        private CancellationTokenSource _cts;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="assetProvider">资源管理器，用于加载 AudioClip</param>
        /// <param name="poolManager">对象池管理器，用于 AudioSource 复用</param>
        /// <param name="logService">日志服务</param>
        /// <param name="config">音频配置</param>
        /// <exception cref="ArgumentNullException">任何依赖参数为 null 时抛出</exception>
        public AudioService(
            IAssetProvider assetProvider,
            IObjectPoolManager poolManager,
            ILogService logService,
            AudioConfig config)
        {
            _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
            _poolManager = poolManager ?? throw new ArgumentNullException(nameof(poolManager));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _cts = new CancellationTokenSource();

            // 初始化音量
            InitializeVolumes();
        }

        #region IStartable

        /// <summary>
        /// 启动音频服务
        /// </summary>
        /// <remarks>
        /// 创建 AudioSource 根节点并预加载常用音效
        /// </remarks>
        public void Start()
        {
            _logService.Information(LogCategory.Audio, "[AudioService] 音频服务启动");

            // 注册静态访问器，使 MonoBehaviour 组件（如 UIButtonSound）可访问
            AudioManager.Register(this);

            // 创建 AudioSource 根节点
            _audioRoot = new GameObject("[AudioRoot]");
            UnityEngine.Object.DontDestroyOnLoad(_audioRoot);

            // 预加载常用音效
            PreloadClipsAsync(_cts.Token).Forget();
        }

        #endregion

        #region 播放控制

        /// <summary>
        /// 异步播放背景音乐（BGM）
        /// </summary>
        /// <param name="key">音频资源 Key（Addressables Address）</param>
        /// <param name="fadeInDuration">淡入时长（秒），默认 0.5 秒</param>
        /// <param name="loop">是否循环播放，默认为 true</param>
        /// <param name="ct">取消令牌</param>
        /// <returns>音频句柄，用于控制播放状态；加载失败时返回 null</returns>
        /// <exception cref="ArgumentNullException">key 为空时抛出</exception>
        /// <remarks>
        /// 如果当前有 BGM 正在播放，会先将其淡出停止，再播放新的 BGM。
        /// 支持链接外部和内部 CancellationToken，确保取消安全。
        /// </remarks>
        public async UniTask<AudioHandle> PlayBGMAsync(string key, float fadeInDuration = 0.5f, bool loop = true, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "BGM Key 不能为空");
            }

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, _cts.Token);

            _logService.Information(LogCategory.Audio, $"[AudioService] 播放 BGM：{key}（淡入 {fadeInDuration}s）");

            // 停止当前 BGM
            if (_currentBGM != null && _currentBGM.IsPlaying)
            {
                await StopAsync(_currentBGM, _config.BGMFadeOutDuration, linkedCts.Token);
            }

            // 加载音频
            var clip = await _assetProvider.LoadAssetAsync<AudioClip>(key, linkedCts.Token);
            if (clip == null)
            {
                _logService.Warning(LogCategory.Audio, $"[AudioService] 无法加载 BGM：{key}");
                return null;
            }

            // 创建 AudioSource
            var source = CreateAudioSource();
            source.clip = clip;
            source.loop = loop;
            source.volume = 0f;
            source.Play();

            // 创建句柄
            var handle = new AudioHandle(key, AudioChannel.BGM, source, OnHandleStopped);
            _currentBGM = handle;
            _activeHandles.Add(handle);

            // 淡入
            if (fadeInDuration > 0)
            {
                await FadeVolumeAsync(source, 0f, GetEffectiveVolume(AudioChannel.BGM), fadeInDuration, linkedCts.Token);
            }
            else
            {
                source.volume = GetEffectiveVolume(AudioChannel.BGM);
            }

            return handle;
        }

        /// <summary>
        /// 异步播放音效（SFX）
        /// </summary>
        /// <param name="key">音频资源 Key（Addressables Address）</param>
        /// <param name="volume">相对音量（0-1），会与通道音量相乘</param>
        /// <param name="channel">音频通道，默认为 SFX</param>
        /// <param name="ct">取消令牌</param>
        /// <returns>音频句柄；并发数达到上限或加载失败时返回 null</returns>
        /// <exception cref="ArgumentNullException">key 为空时抛出</exception>
        /// <remarks>
        /// 音效播放完成后会自动回收 AudioSource 到对象池。
        /// 受 MaxSimultaneousSFX 并发限制保护。
        /// </remarks>
        public async UniTask<AudioHandle> PlaySFXAsync(string key, float volume = 1f, AudioChannel channel = AudioChannel.SFX, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "SFX Key 不能为空");
            }

            // 检查并发上限
            if (GetActiveSFXCount() >= _config.MaxSimultaneousSFX)
            {
                _logService.Warning(LogCategory.Audio, $"[AudioService] 音效并发达到上限 {_config.MaxSimultaneousSFX}，跳过：{key}");
                return null;
            }

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, _cts.Token);

            // 加载音频
            var clip = await _assetProvider.LoadAssetAsync<AudioClip>(key, linkedCts.Token);
            if (clip == null)
            {
                _logService.Warning(LogCategory.Audio, $"[AudioService] 无法加载 SFX：{key}");
                return null;
            }

            // 创建 AudioSource
            var source = CreateAudioSource();
            source.clip = clip;
            source.loop = false;
            source.volume = volume * GetEffectiveVolume(channel);
            source.Play();

            // 创建句柄
            var handle = new AudioHandle(key, channel, source, OnHandleStopped);
            _activeHandles.Add(handle);

            // 播放完成后自动回收
            AutoReleaseWhenFinished(handle, linkedCts.Token).Forget();

            return handle;
        }

        /// <summary>
        /// 异步播放 3D 空间音效
        /// </summary>
        /// <param name="key">音频资源 Key（Addressables Address）</param>
        /// <param name="position">3D 世界坐标位置</param>
        /// <param name="volume">相对音量（0-1）</param>
        /// <param name="ct">取消令牌</param>
        /// <returns>音频句柄；失败时返回 null</returns>
        /// <remarks>
        /// 内部调用 PlaySFXAsync 并将 spatialBlend 设置为 1.0（完全 3D）。
        /// </remarks>
        public async UniTask<AudioHandle> PlaySFX3DAsync(string key, Vector3 position, float volume = 1f, CancellationToken ct = default)
        {
            var handle = await PlaySFXAsync(key, volume, AudioChannel.SFX, ct);
            if (handle != null)
            {
                var source = handle.GetSource();
                if (source != null)
                {
                    source.spatialBlend = 1f; // 3D 音效
                    source.transform.position = position;
                }
            }
            return handle;
        }

        /// <summary>
        /// 异步停止指定音频
        /// </summary>
        /// <param name="handle">要停止的音频句柄</param>
        /// <param name="fadeOutDuration">淡出时长（秒），默认 0.3 秒</param>
        /// <param name="ct">取消令牌</param>
        /// <remarks>
        /// 如果 fadeOutDuration > 0，会先淡出再停止；否则立即停止。
        /// </remarks>
        public async UniTask StopAsync(AudioHandle handle, float fadeOutDuration = 0.3f, CancellationToken ct = default)
        {
            if (handle == null || !handle.IsPlaying) return;

            var source = handle.GetSource();
            if (source == null) return;

            if (fadeOutDuration > 0)
            {
                await FadeVolumeAsync(source, source.volume, 0f, fadeOutDuration, ct);
            }

            handle.Stop();
        }

        /// <summary>
        /// 异步停止指定通道的所有音频
        /// </summary>
        /// <param name="channel">音频通道</param>
        /// <param name="fadeOutDuration">淡出时长（秒），默认 0.3 秒</param>
        /// <param name="ct">取消令牌</param>
        /// <remarks>
        /// 并行停止该通道所有正在播放的音频，所有淡出动画同时进行。
        /// </remarks>
        public async UniTask StopAllAsync(AudioChannel channel, float fadeOutDuration = 0.3f, CancellationToken ct = default)
        {
            var handlesToStop = _activeHandles.FindAll(h => h.Channel == channel && h.IsPlaying);
            var tasks = new List<UniTask>();

            foreach (var handle in handlesToStop)
            {
                tasks.Add(StopAsync(handle, fadeOutDuration, ct));
            }

            await UniTask.WhenAll(tasks);
        }

        /// <summary>
        /// 暂停所有音频
        /// </summary>
        /// <remarks>
        /// 设置全局暂停标志并调用所有活跃 AudioSource 的 Pause 方法。
        /// </remarks>
        public void PauseAll()
        {
            _isPaused = true;
            foreach (var handle in _activeHandles)
            {
                var source = handle.GetSource();
                if (source != null && source.isPlaying)
                {
                    source.Pause();
                }
            }
            _logService.Information(LogCategory.Audio, "[AudioService] 所有音频已暂停");
        }

        /// <summary>
        /// 恢复所有音频
        /// </summary>
        /// <remarks>
        /// 清除全局暂停标志并调用所有活跃 AudioSource 的 UnPause 方法。
        /// </remarks>
        public void ResumeAll()
        {
            _isPaused = false;
            foreach (var handle in _activeHandles)
            {
                var source = handle.GetSource();
                if (source != null)
                {
                    source.UnPause();
                }
            }
            _logService.Information(LogCategory.Audio, "[AudioService] 所有音频已恢复");
        }

        #endregion

        #region 音量控制

        /// <summary>
        /// 设置指定通道的音量
        /// </summary>
        /// <param name="channel">音频通道</param>
        /// <param name="volume">音量值（0-1），会自动钳制到有效范围</param>
        /// <remarks>
        /// 设置后会立即更新该通道所有活跃音频的实际音量。
        /// Master 通道的音量会影响所有其他通道。
        /// </remarks>
        public void SetVolume(AudioChannel channel, float volume)
        {
            volume = Mathf.Clamp01(volume);
            _volumes[channel] = volume;

            if (_volumeSubjects.TryGetValue(channel, out var subject))
            {
                subject.Value = volume;
            }

            // 更新该通道所有活跃音频的音量
            UpdateChannelVolumes(channel);

            _logService.Information(LogCategory.Audio, $"[AudioService] 设置 {channel} 音量：{volume:F2}");
        }

        /// <summary>
        /// 获取指定通道的音量
        /// </summary>
        /// <param name="channel">音频通道</param>
        /// <returns>音量值（0-1）；未设置时返回 1.0</returns>
        public float GetVolume(AudioChannel channel)
        {
            return _volumes.TryGetValue(channel, out var volume) ? volume : 1f;
        }

        /// <summary>
        /// 订阅指定通道的音量变化
        /// </summary>
        /// <param name="channel">音频通道</param>
        /// <returns>R3 Observable，每次通道音量变化时发送新值</returns>
        /// <remarks>
        /// 如果通道对应的 ReactiveProperty 不存在，会自动创建。
        /// 订阅者需要妥善管理订阅的生命周期（建议使用 AddTo）。
        /// </remarks>
        public Observable<float> OnVolumeChanged(AudioChannel channel)
        {
            if (!_volumeSubjects.TryGetValue(channel, out var subject))
            {
                subject = new ReactiveProperty<float>(GetVolume(channel));
                _volumeSubjects[channel] = subject;
            }
            return subject;
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 初始化所有通道的默认音量
        /// </summary>
        /// <remarks>
        /// 遍历所有 AudioChannel 枚举值，从配置中读取默认音量并创建对应的 ReactiveProperty。
        /// </remarks>
        private void InitializeVolumes()
        {
            foreach (AudioChannel channel in Enum.GetValues(typeof(AudioChannel)))
            {
                var defaultVolume = _config.GetDefaultVolume(channel);
                _volumes[channel] = defaultVolume;
                _volumeSubjects[channel] = new ReactiveProperty<float>(defaultVolume);
            }
        }

        /// <summary>
        /// 获取通道的有效音量（Master 音量 × 通道音量）
        /// </summary>
        /// <param name="channel">音频通道</param>
        /// <returns>有效音量值（0-1）</returns>
        private float GetEffectiveVolume(AudioChannel channel)
        {
            var masterVolume = GetVolume(AudioChannel.Master);
            var channelVolume = GetVolume(channel);
            return masterVolume * channelVolume;
        }

        /// <summary>
        /// 从对象池创建或租借 AudioSource
        /// </summary>
        /// <returns>可用的 AudioSource 实例</returns>
        /// <remarks>
        /// 如果对象池中没有可用实例，会通过 CreateNewAudioSource 工厂方法创建新实例。
        /// </remarks>
        private AudioSource CreateAudioSource()
        {
            // 从对象池租借 AudioSource（如果池中没有则创建新的）
            var source = _poolManager.Rent(
                factory: CreateNewAudioSource,
                onRent: OnAudioSourceRent,
                onReturn: OnAudioSourceReturn,
                maxCapacity: _config.AudioSourcePoolSize);
            return source;
        }

        /// <summary>
        /// 创建新的 AudioSource 实例（对象池工厂方法）
        /// </summary>
        /// <returns>新的 AudioSource 组件</returns>
        /// <remarks>
        /// 创建的 GameObject 会作为 AudioRoot 的子节点，并设置 playOnAwake = false。
        /// </remarks>
        private AudioSource CreateNewAudioSource()
        {
            var go = new GameObject("PooledAudioSource");
            go.transform.SetParent(_audioRoot.transform);
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }

        /// <summary>
        /// AudioSource 从对象池租借时的回调
        /// </summary>
        /// <param name="source">租借的 AudioSource</param>
        /// <remarks>
        /// 激活 GameObject 以准备使用。
        /// </remarks>
        private void OnAudioSourceRent(AudioSource source)
        {
            if (source != null && source.gameObject != null)
            {
                source.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// AudioSource 归还到对象池时的回调
        /// </summary>
        /// <param name="source">归还的 AudioSource</param>
        /// <remarks>
        /// 停止播放、清除状态、禁用 GameObject，为下次租借做准备。
        /// </remarks>
        private void OnAudioSourceReturn(AudioSource source)
        {
            if (source != null && source.gameObject != null)
            {
                source.Stop();
                source.clip = null;
                source.volume = 1f;
                source.spatialBlend = 0f;
                source.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// AudioHandle 停止时的回调
        /// </summary>
        /// <param name="handle">停止的音频句柄</param>
        /// <remarks>
        /// 清理流程：
        /// 1. 从活跃列表移除
        /// 2. 归还 AudioSource 到对象池
        /// 3. 释放 Addressables 资源
        /// 4. 如果是当前 BGM，清空 _currentBGM 引用
        /// </remarks>
        private void OnHandleStopped(AudioHandle handle)
        {
            _activeHandles.Remove(handle);

            var source = handle.GetSource();
            if (source != null)
            {
                // 归还到对象池而不是销毁
                _poolManager.Return(source);
            }

            // 释放资源
            if (!string.IsNullOrEmpty(handle.Key))
            {
                _assetProvider.Release(handle.Key);
            }

            if (_currentBGM == handle)
            {
                _currentBGM = null;
            }
        }

        /// <summary>
        /// 音量淡入/淡出动画
        /// </summary>
        /// <param name="source">目标 AudioSource</param>
        /// <param name="from">起始音量</param>
        /// <param name="to">目标音量</param>
        /// <param name="duration">持续时长（秒）</param>
        /// <param name="ct">取消令牌</param>
        /// <remarks>
        /// 使用配置的 FadeCurve 进行插值，支持自定义淡变曲线（动画曲线）。
        /// 每帧更新音量，直到达到目标值或被取消。
        /// </remarks>
        private async UniTask FadeVolumeAsync(AudioSource source, float from, float to, float duration, CancellationToken ct)
        {
            if (source == null || duration <= 0)
            {
                if (source != null) source.volume = to;
                return;
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                ct.ThrowIfCancellationRequested();

                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var curveValue = _config.FadeCurve.Evaluate(t);
                source.volume = Mathf.Lerp(from, to, curveValue);

                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            source.volume = to;
        }

        /// <summary>
        /// 等待音效播放完成后自动回收
        /// </summary>
        /// <param name="handle">音频句柄</param>
        /// <param name="ct">取消令牌</param>
        /// <remarks>
        /// Fire-and-Forget 异步任务，用于 SFX 的自动生命周期管理。
        /// OperationCanceledException 会被静默捕获（正常取消流程）。
        /// </remarks>
        private async UniTaskVoid AutoReleaseWhenFinished(AudioHandle handle, CancellationToken ct)
        {
            try
            {
                var source = handle.GetSource();
                if (source == null || source.clip == null) return;

                // 等待播放完成
                await UniTask.WaitUntil(() => !source.isPlaying || !handle.IsPlaying, PlayerLoopTiming.Update, ct);

                if (handle.IsPlaying)
                {
                    handle.Stop();
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消，忽略
            }
        }

        /// <summary>
        /// 预加载配置中的音频资源
        /// </summary>
        /// <param name="ct">取消令牌</param>
        /// <remarks>
        /// 在服务启动时异步预加载常用音效，减少首次播放时的加载延迟。
        /// 加载失败不会影响服务正常运行，仅记录警告日志。
        /// </remarks>
        private async UniTaskVoid PreloadClipsAsync(CancellationToken ct)
        {
            if (_config.PreloadClipKeys == null || _config.PreloadClipKeys.Count == 0) return;

            _logService.Information(LogCategory.Audio, $"[AudioService] 预加载 {_config.PreloadClipKeys.Count} 个音效...");

            try
            {
                await _assetProvider.PreloadAsync(_config.PreloadClipKeys, ct);
                _logService.Information(LogCategory.Audio, "[AudioService] 音效预加载完成");
            }
            catch (OperationCanceledException)
            {
                // 正常取消
            }
            catch (Exception ex)
            {
                _logService.Warning(LogCategory.Audio, $"[AudioService] 音效预加载失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取当前活跃的音效（非 BGM）数量
        /// </summary>
        /// <returns>活跃的 SFX 数量</returns>
        /// <remarks>
        /// 用于并发限制检查，防止同时播放过多音效。
        /// </remarks>
        private int GetActiveSFXCount()
        {
            int count = 0;
            foreach (var handle in _activeHandles)
            {
                if (handle.Channel != AudioChannel.BGM && handle.IsPlaying)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 更新指定通道所有活跃音频的实际音量
        /// </summary>
        /// <param name="channel">音频通道</param>
        /// <remarks>
        /// 当通道音量变化时调用，立即应用到所有该通道的活跃音频。
        /// Master 通道变化时会影响所有通道。
        /// 对于 BGM，直接设置有效音量；对于 SFX，保持其相对音量设计。
        /// </remarks>
        private void UpdateChannelVolumes(AudioChannel channel)
        {
            foreach (var handle in _activeHandles)
            {
                if (handle.Channel == channel || channel == AudioChannel.Master)
                {
                    var source = handle.GetSource();
                    if (source != null)
                    {
                        // 对于 BGM，直接设置有效音量；对于 SFX，保持相对音量
                        if (handle.Channel == AudioChannel.BGM)
                        {
                            source.volume = GetEffectiveVolume(AudioChannel.BGM);
                        }
                    }
                }
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// 释放音频服务资源
        /// </summary>
        /// <remarks>
        /// 清理流程：
        /// 1. 取消并释放 CancellationTokenSource
        /// 2. 停止所有活跃音频
        /// 3. 销毁 AudioRoot GameObject
        /// 4. 释放所有 ReactiveProperty 订阅
        /// </remarks>
        public void Dispose()
        {
            // 取消静态访问器注册
            AudioManager.Unregister();

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            // 停止所有音频
            foreach (var handle in _activeHandles.ToArray())
            {
                handle.Stop();
            }
            _activeHandles.Clear();

            // 销毁根节点
            if (_audioRoot != null)
            {
                UnityEngine.Object.Destroy(_audioRoot);
                _audioRoot = null;
            }

            // 释放响应式属性
            foreach (var subject in _volumeSubjects.Values)
            {
                subject.Dispose();
            }
            _volumeSubjects.Clear();

            _logService.Information(LogCategory.Audio, "[AudioService] 音频服务已销毁");
        }

        #endregion
    }
}

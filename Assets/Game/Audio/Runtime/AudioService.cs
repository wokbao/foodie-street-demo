using System;
using System.Collections.Generic;
using System.Threading;
using Core.Feature.AssetManagement.Runtime;
using Core.Feature.Logging.Abstractions;
using Core.Feature.ObjectPooling.Abstractions;
using Cysharp.Threading.Tasks;
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

        public void Start()
        {
            _logService.Information(LogCategory.Audio, "[AudioService] 音频服务启动");

            // 创建 AudioSource 根节点
            _audioRoot = new GameObject("[AudioRoot]");
            UnityEngine.Object.DontDestroyOnLoad(_audioRoot);

            // 预加载常用音效
            PreloadClipsAsync(_cts.Token).Forget();
        }

        #endregion

        #region 播放控制

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
            var source = CreateAudioSource(AudioChannel.BGM);
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
            var source = CreateAudioSource(channel);
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

        public float GetVolume(AudioChannel channel)
        {
            return _volumes.TryGetValue(channel, out var volume) ? volume : 1f;
        }

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

        private void InitializeVolumes()
        {
            foreach (AudioChannel channel in Enum.GetValues(typeof(AudioChannel)))
            {
                var defaultVolume = _config.GetDefaultVolume(channel);
                _volumes[channel] = defaultVolume;
                _volumeSubjects[channel] = new ReactiveProperty<float>(defaultVolume);
            }
        }

        private float GetEffectiveVolume(AudioChannel channel)
        {
            var masterVolume = GetVolume(AudioChannel.Master);
            var channelVolume = GetVolume(channel);
            return masterVolume * channelVolume;
        }

        private AudioSource CreateAudioSource(AudioChannel channel)
        {
            var go = new GameObject($"AudioSource_{channel}");
            go.transform.SetParent(_audioRoot.transform);
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }

        private void OnHandleStopped(AudioHandle handle)
        {
            _activeHandles.Remove(handle);

            var source = handle.GetSource();
            if (source != null && source.gameObject != null)
            {
                UnityEngine.Object.Destroy(source.gameObject);
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

        public void Dispose()
        {
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

using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Game.Audio.Runtime.Abstractions
{
    /// <summary>
    /// 音频服务接口
    /// 
    /// <para><b>职责</b>：</para>
    /// <list type="bullet">
    ///   <item>BGM 播放与切换（支持淡入淡出）</item>
    ///   <item>SFX 播放（2D/3D）</item>
    ///   <item>分通道音量控制</item>
    ///   <item>全局暂停/恢复</item>
    /// </list>
    /// 
    /// <para><b>使用示例</b>：</para>
    /// <code>
    /// // 播放 BGM
    /// var bgmHandle = await _audioService.PlayBGMAsync("bgm_menu", fadeInDuration: 1.0f);
    /// 
    /// // 播放 UI 音效
    /// await _audioService.PlaySFXAsync("sfx_button_click", volume: 0.8f);
    /// 
    /// // 调节音量
    /// _audioService.SetVolume(AudioChannel.BGM, 0.5f);
    /// </code>
    /// 
    /// <para><b>注册位置</b>：GameLifetimeScope</para>
    /// </summary>
    public interface IAudioService
    {
        #region 播放控制

        /// <summary>
        /// 播放背景音乐（会停止当前 BGM）
        /// </summary>
        /// <param name="key">Addressables 资源 Key</param>
        /// <param name="fadeInDuration">淡入时长（秒）</param>
        /// <param name="loop">是否循环播放</param>
        /// <param name="ct">取消令牌</param>
        /// <returns>音频句柄</returns>
        UniTask<AudioHandle> PlayBGMAsync(string key, float fadeInDuration = 0.5f, bool loop = true, CancellationToken ct = default);

        /// <summary>
        /// 播放 2D 音效
        /// </summary>
        /// <param name="key">Addressables 资源 Key</param>
        /// <param name="volume">音量 0-1</param>
        /// <param name="channel">音频通道（默认 SFX）</param>
        /// <param name="ct">取消令牌</param>
        /// <returns>音频句柄</returns>
        UniTask<AudioHandle> PlaySFXAsync(string key, float volume = 1f, AudioChannel channel = AudioChannel.SFX, CancellationToken ct = default);

        /// <summary>
        /// 播放 3D 空间音效
        /// </summary>
        /// <param name="key">Addressables 资源 Key</param>
        /// <param name="position">世界坐标位置</param>
        /// <param name="volume">音量 0-1</param>
        /// <param name="ct">取消令牌</param>
        /// <returns>音频句柄</returns>
        UniTask<AudioHandle> PlaySFX3DAsync(string key, Vector3 position, float volume = 1f, CancellationToken ct = default);

        /// <summary>
        /// 停止指定音频
        /// </summary>
        /// <param name="handle">音频句柄</param>
        /// <param name="fadeOutDuration">淡出时长（秒）</param>
        /// <param name="ct">取消令牌</param>
        UniTask StopAsync(AudioHandle handle, float fadeOutDuration = 0.3f, CancellationToken ct = default);

        /// <summary>
        /// 停止指定通道的所有音频
        /// </summary>
        /// <param name="channel">音频通道</param>
        /// <param name="fadeOutDuration">淡出时长（秒）</param>
        /// <param name="ct">取消令牌</param>
        UniTask StopAllAsync(AudioChannel channel, float fadeOutDuration = 0.3f, CancellationToken ct = default);

        /// <summary>
        /// 暂停所有音频
        /// </summary>
        void PauseAll();

        /// <summary>
        /// 恢复所有音频
        /// </summary>
        void ResumeAll();

        #endregion

        #region 音量控制

        /// <summary>
        /// 设置通道音量
        /// </summary>
        /// <param name="channel">音频通道</param>
        /// <param name="volume">音量 0-1</param>
        void SetVolume(AudioChannel channel, float volume);

        /// <summary>
        /// 获取通道音量
        /// </summary>
        /// <param name="channel">音频通道</param>
        /// <returns>音量 0-1</returns>
        float GetVolume(AudioChannel channel);

        /// <summary>
        /// 音量变化事件流（响应式）
        /// </summary>
        /// <param name="channel">音频通道</param>
        /// <returns>音量值的 Observable</returns>
        Observable<float> OnVolumeChanged(AudioChannel channel);

        #endregion
    }
}

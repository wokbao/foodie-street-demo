using System;
using UnityEngine;

namespace Game.Audio.Runtime
{
    /// <summary>
    /// 音频播放句柄，用于控制单个播放中的音频实例
    /// 
    /// <para><b>用途</b>：</para>
    /// <list type="bullet">
    ///   <item>查询播放状态</item>
    ///   <item>停止播放（支持淡出）</item>
    ///   <item>调整音量/音调（后续扩展）</item>
    /// </list>
    /// 
    /// <para><b>使用示例</b>：</para>
    /// <code>
    /// var handle = await _audioService.PlayBGMAsync("bgm_menu");
    /// // ... 稍后停止播放
    /// handle.Stop(fadeOutDuration: 0.5f);
    /// </code>
    /// </summary>
    public class AudioHandle
    {
        private readonly AudioSource _source;
        private readonly Action<AudioHandle> _onStop;
        private bool _isStopped;

        /// <summary>资源 Key（Addressables 地址）</summary>
        public string Key { get; }

        /// <summary>所属音频通道</summary>
        public AudioChannel Channel { get; }

        /// <summary>是否正在播放</summary>
        public bool IsPlaying => !_isStopped && _source != null && _source.isPlaying;

        /// <summary>
        /// 创建音频句柄
        /// </summary>
        /// <param name="key">资源 Key</param>
        /// <param name="channel">音频通道</param>
        /// <param name="source">AudioSource 实例</param>
        /// <param name="onStop">停止时的回调（用于归还 AudioSource 到池）</param>
        internal AudioHandle(string key, AudioChannel channel, AudioSource source, Action<AudioHandle> onStop)
        {
            Key = key;
            Channel = channel;
            _source = source;
            _onStop = onStop;
            _isStopped = false;
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        /// <param name="fadeOutDuration">淡出时长（秒），0 表示立即停止</param>
        public void Stop(float fadeOutDuration = 0f)
        {
            if (_isStopped) return;
            _isStopped = true;

            if (_source != null)
            {
                // 淡出逻辑由 AudioService 内部处理
                _source.Stop();
            }

            _onStop?.Invoke(this);
        }

        /// <summary>
        /// 设置音量（相对于通道音量）
        /// </summary>
        /// <param name="volume">音量值 0-1</param>
        public void SetVolume(float volume)
        {
            if (_source != null && !_isStopped)
            {
                _source.volume = Mathf.Clamp01(volume);
            }
        }

        /// <summary>
        /// 获取内部 AudioSource（仅供 AudioService 内部使用）
        /// </summary>
        internal AudioSource GetSource() => _source;
    }
}

using System.Collections.Generic;
using Core.Runtime.Configuration;
using UnityEngine;

namespace Game.Audio.Runtime
{
    /// <summary>
    /// 音频系统配置
    /// 
    /// <para><b>用途</b>：</para>
    /// <list type="bullet">
    ///   <item>定义各通道默认音量</item>
    ///   <item>配置淡入淡出参数</item>
    ///   <item>设置音效并发上限</item>
    ///   <item>指定预加载音效列表</item>
    /// </list>
    /// 
    /// <para><b>加载方式</b>：通过 GameConfigManifest 自动加载并注入到容器</para>
    /// </summary>
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Game/Audio/AudioConfig")]
    public class AudioConfig : ScriptableObject, IValidatableConfig
    {
        #region 音量配置

        [Header("默认音量")]
        [Tooltip("主音量（控制所有通道）")]
        [Range(0f, 1f)]
        [SerializeField] private float _defaultMasterVolume = 1f;

        [Tooltip("背景音乐音量")]
        [Range(0f, 1f)]
        [SerializeField] private float _defaultBGMVolume = 0.7f;

        [Tooltip("游戏音效音量")]
        [Range(0f, 1f)]
        [SerializeField] private float _defaultSFXVolume = 0.8f;

        [Tooltip("UI 音效音量")]
        [Range(0f, 1f)]
        [SerializeField] private float _defaultUIVolume = 0.8f;

        [Tooltip("语音音量")]
        [Range(0f, 1f)]
        [SerializeField] private float _defaultVoiceVolume = 1f;

        #endregion

        #region 淡入淡出

        [Header("淡入淡出")]
        [Tooltip("BGM 切换时的淡入时长（秒）")]
        [Range(0f, 5f)]
        [SerializeField] private float _bgmFadeInDuration = 0.5f;

        [Tooltip("BGM 切换时的淡出时长（秒）")]
        [Range(0f, 5f)]
        [SerializeField] private float _bgmFadeOutDuration = 0.3f;

        [Tooltip("淡入淡出曲线")]
        [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        #endregion

        #region 性能配置

        [Header("性能")]
        [Tooltip("同时播放的最大音效数量")]
        [Range(8, 64)]
        [SerializeField] private int _maxSimultaneousSFX = 32;

        [Tooltip("AudioSource 对象池初始大小")]
        [Range(4, 32)]
        [SerializeField] private int _audioSourcePoolSize = 16;

        #endregion

        #region 预加载

        [Header("预加载")]
        [Tooltip("启动时预加载的音效 Key 列表（常用 UI 音效）")]
        [SerializeField] private List<string> _preloadClipKeys = new();

        #endregion

        #region 公开属性

        public float DefaultMasterVolume => _defaultMasterVolume;
        public float DefaultBGMVolume => _defaultBGMVolume;
        public float DefaultSFXVolume => _defaultSFXVolume;
        public float DefaultUIVolume => _defaultUIVolume;
        public float DefaultVoiceVolume => _defaultVoiceVolume;

        public float BGMFadeInDuration => _bgmFadeInDuration;
        public float BGMFadeOutDuration => _bgmFadeOutDuration;
        public AnimationCurve FadeCurve => _fadeCurve;

        public int MaxSimultaneousSFX => _maxSimultaneousSFX;
        public int AudioSourcePoolSize => _audioSourcePoolSize;

        public IReadOnlyList<string> PreloadClipKeys => _preloadClipKeys;

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取指定通道的默认音量
        /// </summary>
        public float GetDefaultVolume(AudioChannel channel)
        {
            return channel switch
            {
                AudioChannel.Master => _defaultMasterVolume,
                AudioChannel.BGM => _defaultBGMVolume,
                AudioChannel.SFX => _defaultSFXVolume,
                AudioChannel.UI => _defaultUIVolume,
                AudioChannel.Voice => _defaultVoiceVolume,
                _ => 1f
            };
        }

        #endregion

        #region IValidatableConfig

        public bool Validate(out List<string> errors)
        {
            errors = new List<string>();

            if (_maxSimultaneousSFX < 1)
            {
                errors.Add("最大同时音效数必须大于 0");
            }

            if (_audioSourcePoolSize < 1)
            {
                errors.Add("AudioSource 对象池大小必须大于 0");
            }

            if (_bgmFadeInDuration < 0)
            {
                errors.Add("BGM 淡入时长不能为负");
            }

            if (_bgmFadeOutDuration < 0)
            {
                errors.Add("BGM 淡出时长不能为负");
            }

            if (_fadeCurve == null || _fadeCurve.length < 2)
            {
                errors.Add("淡入淡出曲线至少需要 2 个关键帧");
            }

            return errors.Count == 0;
        }

        #endregion
    }
}

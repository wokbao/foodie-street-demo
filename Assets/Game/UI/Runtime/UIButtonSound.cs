using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Audio.Runtime;
using Game.Audio.Runtime.Abstractions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Runtime
{
    /// <summary>
    /// UI 按钮音效组件
    /// 
    /// <para><b>用途</b>：</para>
    /// <list type="bullet">
    ///   <item>挂载到 Button 上自动播放点击音效</item>
    ///   <item>无需在 Presenter 中手动调用</item>
    ///   <item>支持自定义音效 Key</item>
    /// </list>
    /// 
    /// <para><b>使用方式</b>：</para>
    /// <list type="number">
    ///   <item>将此组件挂载到任意带有 Button 的 GameObject</item>
    ///   <item>可选：在 Inspector 中自定义 Click Sound Key</item>
    ///   <item>运行时自动绑定点击事件并播放音效</item>
    /// </list>
    /// 
    /// <para><b>实现原理</b>：通过 <see cref="AudioManager.Instance"/> 访问音频服务，无需 DI 注入</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class UIButtonSound : MonoBehaviour
    {
        [Header("音效配置")]
        [Tooltip("点击时播放的音效 Key（留空使用默认点击音效）")]
        [SerializeField] private string _clickSoundKey = "";

        [Tooltip("音效音量")]
        [Range(0f, 1f)]
        [SerializeField] private float _volume = 1f;

        [Tooltip("是否启用音效")]
        [SerializeField] private bool _enabled = true;

        private Button _button;
        private CancellationTokenSource _cts;

        /// <summary>
        /// 实际使用的音效 Key
        /// </summary>
        private string EffectiveSoundKey =>
            string.IsNullOrEmpty(_clickSoundKey) ? AudioKeys.UI.ButtonClick : _clickSoundKey;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _cts = new CancellationTokenSource();
        }

        private void Start()
        {
            if (_button != null)
            {
                _button.onClick.AddListener(OnButtonClick);
            }
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            if (_button != null)
            {
                _button.onClick.RemoveListener(OnButtonClick);
            }
        }

        private void OnButtonClick()
        {
            if (!_enabled) return;

            // 通过静态访问器获取音频服务（每次调用都获取最新实例，避免 Scope 切换后引用过期）
            var audioService = AudioManager.Instance;
            if (audioService == null)
            {
                Debug.LogWarning("[UIButtonSound] AudioService 未就绪，跳过音效播放");
                return;
            }

            PlayClickSoundAsync(audioService).Forget();
        }

        private async UniTaskVoid PlayClickSoundAsync(IAudioService audioService)
        {
            try
            {
                await audioService.PlaySFXAsync(EffectiveSoundKey, _volume, AudioChannel.UI, _cts.Token);
            }
            catch (System.OperationCanceledException)
            {
                // 正常取消，忽略
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (GetComponent<Button>() == null)
            {
                Debug.LogWarning($"[UIButtonSound] {gameObject.name} 上没有 Button 组件", this);
            }
        }
#endif
    }
}

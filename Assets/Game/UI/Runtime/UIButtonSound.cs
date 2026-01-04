using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Audio.Runtime;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

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
        private IAudioService _audioService;
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
            // 尝试从场景中解析 IAudioService
            TryResolveAudioService();

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
            if (!_enabled || _audioService == null) return;

            PlayClickSoundAsync().Forget();
        }

        private async UniTaskVoid PlayClickSoundAsync()
        {
            try
            {
                await _audioService.PlaySFXAsync(EffectiveSoundKey, _volume, AudioChannel.UI, _cts.Token);
            }
            catch (System.OperationCanceledException)
            {
                // 正常取消，忽略
            }
        }

        /// <summary>
        /// 手动注入 AudioService（用于 VContainer 场景）
        /// </summary>
        [Inject]
        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        private void TryResolveAudioService()
        {
            if (_audioService != null) return;

            // 尝试从父级 LifetimeScope 解析
            var scope = GetComponentInParent<VContainer.Unity.LifetimeScope>();
            if (scope != null && scope.Container != null)
            {
                _audioService = scope.Container.Resolve<IAudioService>();
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

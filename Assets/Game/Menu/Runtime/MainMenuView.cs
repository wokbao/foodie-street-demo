using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Menu.Runtime.Abstractions;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Menu.Runtime
{
    /// <summary>
    /// 主菜单 UI 视图脚本。
    /// 放到场景的主菜单根节点上，并在 Inspector 绑定按钮与可选的加载指示器。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MainMenuView : MonoBehaviour, IMainMenuView
    {
        [Header("Buttons")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

        [Header("Loading")]
        [SerializeField] private GameObject _loadingIndicator;

        [Header("Config")]
        [Tooltip("开始游戏时要加载的场景 Key（Addressables 或 Build Settings 名称）。")]
        [SerializeField] private string _startSceneKey = "Gameplay";

        [Tooltip("进入游戏时是否显示加载界面。")]
        [SerializeField] private bool _useLoadingScreen = true;

        public event Action PlayClicked;
        public event Action SettingsClicked;
        public event Action QuitClicked;

        public string StartSceneKey => _startSceneKey;
        public bool UseLoadingScreen => _useLoadingScreen;
        public CancellationToken DestroyCancellationToken => this.GetCancellationTokenOnDestroy();

        private void Awake()
        {
            if (_playButton != null)
            {
                _playButton.onClick.AddListener(() => PlayClicked?.Invoke());
            }

            if (_settingsButton != null)
            {
                _settingsButton.onClick.AddListener(() => SettingsClicked?.Invoke());
            }

            if (_quitButton != null)
            {
                _quitButton.onClick.AddListener(() => QuitClicked?.Invoke());
            }

            ShowLoadingIndicator(false);
        }

        private void OnDestroy()
        {
            _playButton?.onClick.RemoveAllListeners();
            _settingsButton?.onClick.RemoveAllListeners();
            _quitButton?.onClick.RemoveAllListeners();
        }

        public void SetInteractable(bool interactable)
        {
            if (_playButton != null) _playButton.interactable = interactable;
            if (_settingsButton != null) _settingsButton.interactable = interactable;
            if (_quitButton != null) _quitButton.interactable = interactable;
        }

        public void ShowLoadingIndicator(bool visible)
        {
            if (_loadingIndicator != null)
            {
                _loadingIndicator.SetActive(visible);
            }
        }
    }
}

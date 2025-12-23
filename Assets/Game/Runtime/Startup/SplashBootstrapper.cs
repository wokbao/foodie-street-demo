using System;
using Core.Feature.Loading.Abstractions;
using Core.Feature.Loading.Runtime;
using Core.Runtime.Configuration;
using Cysharp.Threading.Tasks;
using Game.Runtime.Configs;
using Game.Runtime.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Runtime.Startup
{
    /// <summary>
    /// Splash 场景启动器，负责异步预加载所有配置。
    /// 这是游戏启动的第一个场景，完成配置加载后跳转到主菜单。
    /// 
    /// <para><b>架构说明</b>：</para>
    /// <list type="bullet">
    ///   <item>位于 Game 层，因为需要加载 Game 配置（GameConfigManifest）</item>
    ///   <item>加载 Core 配置和 Game 配置，存入 ConfigCache</item>
    ///   <item>Core/Game LifetimeScope 从缓存读取，避免同步阻塞</item>
    /// </list>
    /// </summary>
    public sealed class SplashBootstrapper : MonoBehaviour
    {
        [Header("配置清单")]
        [SerializeField]
        [Tooltip("核心配置清单")]
        private ConfigManifest _coreConfigManifest;

        [SerializeField]
        [Tooltip("游戏配置清单")]
        private GameConfigManifest _gameConfigManifest;

        [Header("场景设置")]
        [SerializeField]
        [Tooltip("配置加载完成后跳转的场景名称")]
        private string _nextSceneName = "Menu";

        [Header("可选配置")]
        [SerializeField]
        [Tooltip("可选：LoadingHud 配置，为空则使用默认配置")]
        private LoadingHudConfig _loadingHudConfig;

        private async void Start()
        {
            // 创建临时 LoadingService（用于显示配置加载进度）
            var loadingService = new LoadingService();

            // 创建临时 LoadingHud
            var hudGo = new GameObject("LoadingHud_Temp");
            DontDestroyOnLoad(hudGo); // 确保场景切换时不被销毁
            var hud = hudGo.AddComponent<LoadingHud>();
            hud.Initialize(loadingService);

            try
            {
                var ct = this.GetCancellationTokenOnDestroy();

                //  阶段 1: 异步加载配置
                // ========================================

                var coreResult = await ConfigLoader.LoadFromManifestAsync(
                    _coreConfigManifest,
                    loadingService,
                    ct
                );
                ConfigCache.SetCoreConfigs(coreResult);

                var gameResult = await ConfigLoader.LoadFromManifestAsync(
                    _gameConfigManifest,
                    loadingService,
                    ct
                );
                ConfigCache.SetGameConfigs(gameResult);

                // ========================================
                // 阶段 2: 手动触发 CoreLifetimeScope 构建
                // ========================================

                // 找到场景中的 CoreLifetimeScope（autoRun 已设置为 false）
                var coreScope = FindObjectOfType<Core.Bootstrap.CoreLifetimeScope>();
                if (coreScope == null)
                {
                    throw new Exception("未在 Splash 场景中找到 CoreLifetimeScope！");
                }

                // 现在配置已加载完成，可以安全地构建 DI 容器
                coreScope.Build(); // 手动触发构建

                // ========================================
                // 阶段 3: 短暂延迟，确保用户看到加载完成
                // ========================================

                // CoreLifetimeScope 会在场景启动时自动初始化（VContainer机制）
                // 它设置了 autoRun=true，会自动从 ConfigCache 读取配置
                // 我们只需确保配置已加载完成即可
                loadingService.ReportProgress(1f, "加载完成");
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: ct);

                //  阶段 3: 跳转到主菜单
                // ========================================

                SceneManager.LoadScene(_nextSceneName);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("[SplashBootstrapper] 配置加载被取消（应用程序退出）");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SplashBootstrapper] 配置加载失败: {ex.Message}");
                Debug.LogException(ex);

                // TODO: 显示错误对话框或重试逻辑
#if UNITY_EDITOR
                var retry = UnityEditor.EditorUtility.DisplayDialog(
                    "配置加载失败",
                    $"无法加载游戏配置，请检查控制台日志。\n\n错误信息: {ex.Message}\n\n是否重试？",
                    "重试",
                    "退出"
                );

                if (retry)
                {
                    // 重新加载 Splash 场景
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                else
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                }
#else
                // 运行时：显示错误信息后退出
                Application.Quit();
#endif
            }
            finally
            {
                // 清理临时对象
                loadingService?.Dispose();
                if (hudGo != null)
                {
                    Destroy(hudGo);
                }
            }
        }

        private void OnValidate()
        {
            // 验证配置清单是否设置
            if (_coreConfigManifest == null)
            {
                Debug.LogWarning("[SplashBootstrapper] 核心配置清单未设置，启动时会失败", this);
            }

            if (_gameConfigManifest == null)
            {
                Debug.LogWarning("[SplashBootstrapper] 游戏配置清单未设置，启动时会失败", this);
            }

            if (string.IsNullOrEmpty(_nextSceneName))
            {
                Debug.LogWarning("[SplashBootstrapper] 下一个场景名称为空，启动时会失败", this);
            }
        }
    }
}

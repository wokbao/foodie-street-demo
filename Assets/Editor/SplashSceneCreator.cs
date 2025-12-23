using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;

namespace Game.Editor
{
    /// <summary>
    /// Splash 场景自动生成工具
    /// </summary>
    public static class SplashSceneCreator
    {
        private const string ScenePath = "Assets/Game/Scenes/Game_Splash.unity";
        private const string SceneFolder = "Assets/Game/Scenes";

        [MenuItem("Tools/Loading System/Create Splash Scene", priority = 100)]
        public static void CreateSplashScene()
        {
            // 确保文件夹存在
            if (!Directory.Exists(SceneFolder))
            {
                Directory.CreateDirectory(SceneFolder);
                AssetDatabase.Refresh();
                Debug.Log($"[SplashSceneCreator] 创建文件夹: {SceneFolder}");
            }

            // 检查场景是否已存在
            if (File.Exists(ScenePath))
            {
                var overwrite = EditorUtility.DisplayDialog(
                    "场景已存在",
                    $"Splash 场景已存在：{ScenePath}\n\n是否覆盖？",
                    "覆盖",
                    "取消"
                );

                if (!overwrite)
                {
                    Debug.Log("[SplashSceneCreator] 用户取消操作");
                    return;
                }
            }

            // 创建新场景
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Debug.Log("[SplashSceneCreator] 创建空场景");

            // ========================================
            // 1. 创建 CoreLifetimeScope（DontDestroyOnLoad）
            // ========================================
            var coreGo = new GameObject("CoreLifetimeScope");
            var coreScope = coreGo.AddComponent<Core.Bootstrap.CoreLifetimeScope>();
            Debug.Log("[SplashSceneCreator] 添加 CoreLifetimeScope 组件（DontDestroyOnLoad）");

            // 尝试自动配置 Core ConfigManifest
            TryAutoConfigureCoreManifest(coreScope);

            // ========================================
            // 2. 创建 SplashBootstrapper
            // ========================================
            var bootstrapperGo = new GameObject("SplashBootstrapper");
            var bootstrapper = bootstrapperGo.AddComponent<Game.Runtime.Startup.SplashBootstrapper>();
            Debug.Log("[SplashSceneCreator] 添加 SplashBootstrapper 组件");

            // 尝试自动配置 Game ConfigManifest
            TryAutoConfigureGameManifest(bootstrapper);

            // ========================================
            // 3. 创建 EventSystem
            // ========================================
            var eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("[SplashSceneCreator] 添加 EventSystem");

            // ========================================
            // 4. （可选）创建 Main Camera
            // ========================================
            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            var camera = cameraGo.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            camera.orthographic = true;
            cameraGo.AddComponent<AudioListener>();
            Debug.Log("[SplashSceneCreator] 添加 Main Camera");

            // 保存场景
            EditorSceneManager.SaveScene(newScene, ScenePath);
            AssetDatabase.Refresh();
            Debug.Log($"[SplashSceneCreator] 场景已保存: {ScenePath}");

            // 添加到 Build Settings
            AddSceneToBuildSettings();

            // 显示完成提示
            EditorUtility.DisplayDialog(
                "Splash 场景创建成功",
                $"场景已创建：{ScenePath}\n\n" +
                "✅ 已添加 CoreLifetimeScope（DontDestroyOnLoad）\n" +
                "✅ 已添加 SplashBootstrapper\n" +
                "✅ 已添加 EventSystem\n" +
                "✅ 已添加到 Build Settings（Index 0）\n\n" +
                "⚠️ 请检查 Inspector 中的配置清单是否正确",
                "确定"
            );

            // 选中 SplashBootstrapper GameObject
            Selection.activeGameObject = bootstrapperGo;
            EditorGUIUtility.PingObject(bootstrapperGo);
        }

        private static void TryAutoConfigureCoreManifest(Core.Bootstrap.CoreLifetimeScope coreScope)
        {
            // 查找 ConfigManifest
            var coreManifests = AssetDatabase.FindAssets("t:ConfigManifest")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Core.Runtime.Configuration.ConfigManifest>)
                .Where(m => m != null)
                .ToArray();

            if (coreManifests.Length > 0)
            {
                var serializedObject = new SerializedObject(coreScope);
                var coreManifestProp = serializedObject.FindProperty("_coreConfigManifest");
                coreManifestProp.objectReferenceValue = coreManifests[0];
                serializedObject.ApplyModifiedProperties();
                Debug.Log($"[SplashSceneCreator] 自动配置核心配置清单: {coreManifests[0].name}");
            }
            else
            {
                Debug.LogWarning("[SplashSceneCreator] 未找到 ConfigManifest，请手动配置 CoreLifetimeScope");
            }
        }

        private static void TryAutoConfigureGameManifest(Game.Runtime.Startup.SplashBootstrapper bootstrapper)
        {
            // 查找 ConfigManifest（用于 SplashBootstrapper 的 Core 配置）
            var coreManifests = AssetDatabase.FindAssets("t:ConfigManifest")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Core.Runtime.Configuration.ConfigManifest>)
                .Where(m => m != null)
                .ToArray();

            if (coreManifests.Length > 0)
            {
                var serializedObject = new SerializedObject(bootstrapper);
                var coreManifestProp = serializedObject.FindProperty("_coreConfigManifest");
                coreManifestProp.objectReferenceValue = coreManifests[0];
                serializedObject.ApplyModifiedProperties();
                Debug.Log($"[SplashSceneCreator] 自动配置 SplashBootstrapper 核心配置清单: {coreManifests[0].name}");
            }
            else
            {
                Debug.LogWarning("[SplashSceneCreator] 未找到 ConfigManifest，请手动配置 SplashBootstrapper");
            }

            // 查找 GameConfigManifest
            var gameManifests = AssetDatabase.FindAssets("t:GameConfigManifest")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Game.Runtime.Configs.GameConfigManifest>)
                .Where(m => m != null)
                .ToArray();

            if (gameManifests.Length > 0)
            {
                var serializedObject = new SerializedObject(bootstrapper);
                var gameManifestProp = serializedObject.FindProperty("_gameConfigManifest");
                gameManifestProp.objectReferenceValue = gameManifests[0];
                serializedObject.ApplyModifiedProperties();
                Debug.Log($"[SplashSceneCreator] 自动配置游戏配置清单: {gameManifests[0].name}");
            }
            else
            {
                Debug.LogWarning("[SplashSceneCreator] 未找到 GameConfigManifest，请手动配置");
            }

            // 设置下一个场景名称
            var serializedObj = new SerializedObject(bootstrapper);
            var nextSceneProp = serializedObj.FindProperty("_nextSceneName");
            nextSceneProp.stringValue = "Game_Menu_Main";
            serializedObj.ApplyModifiedProperties();
            Debug.Log("[SplashSceneCreator] 设置下一个场景: Game_Menu_Main");
        }

        private static void AddSceneToBuildSettings()
        {
            var scenes = EditorBuildSettings.scenes.ToList();

            // 检查是否已存在
            var existingScene = scenes.FirstOrDefault(s => s.path == ScenePath);
            if (existingScene != null)
            {
                scenes.Remove(existingScene);
                Debug.Log("[SplashSceneCreator] 移除旧的 Splash 场景引用");
            }

            // 添加到第一位
            scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"[SplashSceneCreator] 已添加到 Build Settings (Index 0)");
        }

        [MenuItem("Tools/Loading System/Open Splash Scene", priority = 101)]
        public static void OpenSplashScene()
        {
            if (!File.Exists(ScenePath))
            {
                var create = EditorUtility.DisplayDialog(
                    "场景不存在",
                    "Splash 场景尚未创建。\n\n是否立即创建？",
                    "创建",
                    "取消"
                );

                if (create)
                {
                    CreateSplashScene();
                }
                return;
            }

            EditorSceneManager.OpenScene(ScenePath);
            Debug.Log($"[SplashSceneCreator] 已打开场景: {ScenePath}");
        }

        [MenuItem("Tools/Loading System/Validate Splash Scene", priority = 102)]
        public static void ValidateSplashScene()
        {
            if (!File.Exists(ScenePath))
            {
                EditorUtility.DisplayDialog(
                    "验证失败",
                    "❌ Splash 场景不存在\n\n请先创建场景：Tools → Loading System → Create Splash Scene",
                    "确定"
                );
                return;
            }

            var scenes = EditorBuildSettings.scenes;
            var splashScene = scenes.FirstOrDefault(s => s.path == ScenePath);

            if (splashScene == null)
            {
                EditorUtility.DisplayDialog(
                    "验证失败",
                    "❌ Splash 场景未添加到 Build Settings\n\n请重新创建场景",
                    "确定"
                );
                return;
            }

            if (scenes[0].path != ScenePath)
            {
                EditorUtility.DisplayDialog(
                    "验证警告",
                    $"⚠️ Splash 场景不是第一个场景\n\n当前位置：Index {System.Array.IndexOf(scenes, splashScene)}\n应该位置：Index 0",
                    "确定"
                );
                return;
            }

            EditorUtility.DisplayDialog(
                "验证成功",
                "✅ Splash 场景配置正确\n\n" +
                "- 场景文件存在\n" +
                "- 已添加到 Build Settings\n" +
                "- 位于第一位（Index 0）",
                "确定"
            );
        }
    }
}

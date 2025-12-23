# Loading 系统美术对接文档

## 📋 概述

本文档说明如何将精美的美术资源集成到 Loading 系统中。  
当前实现提供了完整的技术架构和扩展接口，开发者可以轻松替换为美术定制的版本。

---

## 🎨 扩展点一览

### 1. **Splash Logo 动画**
- **当前实现**: 无（直接显示 LoadingHud）
- **扩展位置**: `SplashBootstrapper.cs`
- **扩展说明**:

```csharp
// 在 SplashBootstrap.Start() 方法开头添加：
private async void Start()
{
    // 播放 Logo 动画
    await PlayCompanyLogoAnimation();
    await PlayGameLogoAnimation();
    
    // ... 原有的配置加载代码
}

private async UniTask PlayCompanyLogoAnimation()
{
    // 实例化 Logo Prefab
    var logoPrefab = await Addressables.LoadAssetAsync<GameObject>("Logo_Company").Task;
    var logoInstance = Instantiate(logoPrefab);
    
    // 播放动画（假设有 Animator 组件）
    var animator = logoInstance.GetComponent<Animator>();
    animator.Play("FadeIn");
    
    // 等待动画播放完成
    await UniTask.Delay(TimeSpan.FromSeconds(3f));
    
    // 销毁
    Destroy(logoInstance);
}
```

**建议美术资源**:
- 公司 Logo 动画预制体（带 Animator）
- 游戏 Logo 动画预制体
- 音效：Logo 音效（Ding~）

---

### 2. **LoadingHud 美术版本**
- **当前实现**: `LoadingHud.cs` - 纯代码生成UI
- **扩展方式**: 创建预制体版本，实现 `ILoadingView` 接口

#### 步骤：

**Step 1: 创建美术预制体**
- 使用 Unity UI 或 UI Toolkit 创建精美的 Loading 界面
- 包含元素：
  - 背景图片（可以是多张，随机显示）
  - 进度条（Slider 或自定义动画）
  - 加载描述文字（TextMeshPro）
  - Spinner / 旋转圈（粒子特效或动画）  - Tips 文字区域（可选）

**Step 2: 创建脚本实现接口**

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Runtime.Loading.Abstractions;

namespace Game.Runtime.Loading
{
    /// <summary>
    /// 美术定制的 LoadingView 预制体版本
    /// </summary>
    public class LoadingViewPrefab : MonoBehaviour, ILoadingView
    {
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private GameObject _spinner;
        [SerializeField] private CanvasGroup _canvasGroup;

        public bool IsVisible => _canvasGroup.alpha > 0f;

        public void SetProgress(float progress)
        {
            _progressBar.value = Mathf.Clamp01(progress);
        }

        public void SetDescription(string description)
        {
            _descriptionText.text = description;
        }

        public void Show()
        {
            _canvasGroup.alpha = 1f;
            _spinner.SetActive(true);
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0f;
            _spinner.SetActive(false);
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}
```

**Step 3: 在 LoadingHudEntryPoint 中使用**

修改 `LoadingHudEntryPoint.cs`:
```csharp
public class LoadingHudEntryPoint : IStartable, IDisposable
{
    private ILoadingView _loadingView;
    private readonly ILoadingService _loadingService;
    private readonly IAssetProvider _assetProvider;

    public async void Start()
    {
        // 加载美术版预制体
        var prefab = await _assetProvider.LoadAsync<GameObject>("LoadingView_Prefab");
        var instance = UnityEngine.Object.Instantiate(prefab);
        _loadingView = instance.GetComponent<ILoadingView>();
        
        // 初始化（订阅 LoadingService 事件）
        _loadingService.OnStateChanged += OnLoadingStateChanged;
    }
}
```

**建议美术资源**:
- Loading 背景图（1920x1080，3-5张随机）
- 进度条 Sprite（背景 + 填充）
- Spinner 粒子特效或动画
- 字体：推荐使用 TextMeshPro

---

### 3. **Tips 系统**
- **当前实现**: 无
- **扩展位置**: `LoadingViewPrefab.cs`

**实现示例**:

```csharp
[SerializeField] private TextMeshProUGUI _tipsText;
[SerializeField] private float _tipsChangeInterval = 5f;

private static readonly string[] LoadingTips = {
    "按住 Shift 可以加速移动",
    "完成每日任务可以获得额外奖励",
    "试试使用不同的食材组合，发现新菜谱",
    // ... 更多 Tips
};

private async void Start()
{
    while (true)
    {
        var randomTip = LoadingTips[Random.Range(0, LoadingTips.Length)];
        _tipsText.text = randomTip;
        await UniTask.Delay(TimeSpan.FromSeconds(_tipsChangeInterval));
    }
}
```

**建议准备**:
- 50-100 条游戏提示文字
- 本地化文本（如果需要多语言）

---

### 4. **场景过渡动画**
- **当前实现**: `SceneFadeTransition`、`SceneCinematicTransition` 等
- **扩展方式**: 实现 `ISceneTransition` 接口

**已有的过渡效果**:
- Fade（淡入淡出）
- Cinematic（电影条过渡）
- Shutter（百叶窗）
- Noise（噪声）

**自定义过渡效果**:

```csharp
public class CustomTransition : ISelectableSceneTransition
{
    public TransitionName Name => TransitionName.Custom;

    public async UniTask PlayOutAsync(string fromScene, string toScene, string description)
    {
        // 播放离开动画
        // 例如：径向模糊、像素化、漩涡等
    }

    public async UniTask PlayInAsync(string toScene, string description)
    {
        // 播放进入动画
    }
}
```

在 `CoreLifetimeScope.cs` 中注册：
```csharp
builder.Register<CustomTransition>(Lifetime.Singleton)
    .As<ISelectableSceneTransition>();
```

**建议美术资源**:
- Shader / Material（径向模糊、像素化等）
- 过渡音效

---

### 5. **音频反馈**
- **当前实现**: 无
- **扩展位置**: 各个关键节点

**建议添加音效的位置**:

```csharp
// 1. Logo 音效
private async UniTask PlayCompanyLogoAnimation()
{
    PlaySound("Audio/Logo_Ding");
    // ...
}

// 2. 加载开始音效
loadingService.OnLoadingStarted += () => {
    PlaySound("Audio/Loading_Start");
};

// 3. 加载完成音效
loadingService.OnLoadingCompleted += () => {
    PlaySound("Audio/Loading_Complete");
};

// 4. 进度条移动音效（可选）
private void OnProgressChanged(float progress)
{
    if (progress >= 0.99f)
    {
        PlaySound("Audio/Progress_Complete");
    }
}
```

**建议美术资源**:
- Logo 音效（1-2秒）
- Loading 环境音/BGM（循环）
- 进度完成音效
- UI 点击音效

---

## 🔧 配置文件

### LoadingHudConfig
当前配置支持的参数（`LoadingHudConfig.cs`）:

```csharp
[Header("动画配置")]
public float ShowDelaySeconds = 2f;       // 延迟显示时间
public float FadeInDuration = 0.3f;       // 淡入时长
public float FadeOutDuration = 0.2f;      // 淡出时长

[Header("视觉配置")]
public Color OverlayColor;                // 遮罩颜色
public Color ProgressBarBackgroundColor;  // 进度条背景色
public Color ProgressBarFillColor;        // 进度条填充色
public Color DescriptionTextColor;        // 文字颜色
public Color SpinnerColor;                // Spinner 颜色

[Header("布局配置")]
public Vector2 SpinnerAnchor;             // Spinner 锚点
public Vector2 SpinnerSize;               // Spinner 大小
public Vector2 ProgressBarAnchorMin;      // 进度条最小锚点
public Vector2 ProgressBarAnchorMax;      // 进度条最大锚点
public Vector2 DescriptionAnchorMin;      // 描述文字最小锚点
public Vector2 DescriptionAnchorMax;      // 描述文字最大锚点

[Header("其他")]
public int DescriptionFontSize = 20;      // 文字大小
public float SpinnerRotationSpeed = 180f; // 旋转速度
```

**美术可调整**:
- 颜色方案
- 布局位置
- 动画时长
- 字体大小

---

## 📦 资源组织建议

```
Assets/
├─ Art/
│  ├─ UI/
│  │  ├─ Loading/
│  │  │  ├─ Backgrounds/        # 背景图（3-5张）
│  │  │  ├─ ProgressBar/        # 进度条 Sprite
│  │  │  ├─ Spinner/            # 旋转动画 / 粒子特效
│  │  │  ├─ Logo/               # Logo 预制体
│  │  │  └─ Font/               # 字体文件
│  │  └─ Prefabs/
│  │     └─ LoadingView_Prefab.prefab
│  └─ Audio/
│     ├─ Logo_Ding.wav
│     ├─ Loading_Start.wav
│     ├─ Loading_Complete.wav
│     └─ Loading_BGM.wav
```

---

## ✅ 集成检查清单

- [ ] 准备 Logo 动画预制体并集成到 `SplashBootstrapper`
- [ ] 创建 LoadingView 美术预制体，实现 `ILoadingView` 接口
- [ ] 准备 3-5 张 Loading 背景图
- [ ] 编写 50-100 条 Tips 文字
- [ ] 准备音效（Logo、Loading 开始/完成）
- [ ] 调整 `LoadingHudConfig` 配置参数
- [ ] 测试所有过渡动画是否流畅
- [ ] 验证不同分辨率下的 UI 表现

---

## 🎓 技术支持

如有疑问或需要技术支持，请参考：
- `ILoadingView.cs` - Loading UI 接口定义
- `LoadingHud.cs` - 参考实现
- `SplashBootstrapper.cs` - Splash 启动流程
- `SceneService.cs` - 场景加载细粒度阶段追踪

---

**祝对接顺利！🎉**

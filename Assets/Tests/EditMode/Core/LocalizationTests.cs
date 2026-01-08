using NUnit.Framework;
using Core.Feature.Localization.Abstractions;

namespace Tests.EditMode.Core
{
    /// <summary>
    /// Localization 模块单元测试
    /// </summary>
    /// <remarks>
    /// <para><b>测试范围</b>：</para>
    /// <list type="bullet">
    ///   <item>DefaultLocalizationOptions - Null Object Pattern 实现</item>
    ///   <item>SupportedLocale - 枚举与代码映射</item>
    ///   <item>LocaleExtensions - 扩展方法</item>
    /// </list>
    /// <para><b>注意</b>：LocalizationService 依赖 Unity Localization Package，需要 PlayMode 测试。</para>
    /// </remarks>
    public class LocalizationTests
    {
        // ==============================================
        // DefaultLocalizationOptions 测试
        // ==============================================

        [Test]
        public void DefaultLocalizationOptions_Instance_IsNotNull()
        {
            // Arrange & Act
            var instance = DefaultLocalizationOptions.Instance;

            // Assert
            Assert.IsNotNull(instance, "DefaultLocalizationOptions.Instance 不应为 null");
        }

        [Test]
        public void DefaultLocalizationOptions_DefaultTableName_IsSharedStrings()
        {
            // Arrange
            var options = DefaultLocalizationOptions.Instance;

            // Act
            var tableName = options.DefaultTableName;

            // Assert
            Assert.AreEqual(DefaultLocalizationOptions.DefaultTable, tableName);
            Assert.AreEqual("SharedStrings", tableName);
        }

        [Test]
        public void DefaultLocalizationOptions_FallbackLocaleCode_IsEnglish()
        {
            // Arrange
            var options = DefaultLocalizationOptions.Instance;

            // Act
            var fallbackCode = options.FallbackLocaleCode;

            // Assert
            Assert.AreEqual("en", fallbackCode);
        }

        [Test]
        public void DefaultLocalizationOptions_InitializeOnStartup_IsTrue()
        {
            // Arrange
            var options = DefaultLocalizationOptions.Instance;

            // Assert
            Assert.IsTrue(options.InitializeOnStartup);
        }

        [Test]
        public void DefaultLocalizationOptions_RememberUserSelection_IsTrue()
        {
            // Arrange
            var options = DefaultLocalizationOptions.Instance;

            // Assert
            Assert.IsTrue(options.RememberUserSelection);
        }

        // ==============================================
        // SupportedLocale 枚举测试
        // ==============================================

        [Test]
        public void SupportedLocale_HasExpectedValues()
        {
            // Assert - 验证枚举值存在
            Assert.IsTrue(System.Enum.IsDefined(typeof(SupportedLocale), SupportedLocale.English));
            Assert.IsTrue(System.Enum.IsDefined(typeof(SupportedLocale), SupportedLocale.ChineseSimplified));
            Assert.IsTrue(System.Enum.IsDefined(typeof(SupportedLocale), SupportedLocale.ChineseTraditional));
        }

        [Test]
        public void SupportedLocale_DefaultValue_IsEnglish()
        {
            // Arrange
            var defaultLocale = default(SupportedLocale);

            // Assert - 默认值应该是 English (0)
            Assert.AreEqual(SupportedLocale.English, defaultLocale);
        }

        // ==============================================
        // LocaleExtensions 扩展方法测试
        // ==============================================

        [Test]
        public void LocaleExtensions_ToCode_English_ReturnsEn()
        {
            // Arrange
            var locale = SupportedLocale.English;

            // Act
            var code = locale.ToCode();

            // Assert
            Assert.AreEqual("en", code);
        }

        [Test]
        public void LocaleExtensions_ToCode_ChineseSimplified_ReturnsZhHans()
        {
            // Arrange
            var locale = SupportedLocale.ChineseSimplified;

            // Act
            var code = locale.ToCode();

            // Assert
            Assert.AreEqual("zh-Hans", code);
        }

        [Test]
        public void LocaleExtensions_ToCode_ChineseTraditional_ReturnsZhHant()
        {
            // Arrange
            var locale = SupportedLocale.ChineseTraditional;

            // Act
            var code = locale.ToCode();

            // Assert
            Assert.AreEqual("zh-Hant", code);
        }

        [Test]
        public void LocaleExtensions_FromCode_En_ReturnsEnglish()
        {
            // Act
            var locale = LocaleExtensions.FromCode("en");

            // Assert
            Assert.AreEqual(SupportedLocale.English, locale);
        }

        [Test]
        public void LocaleExtensions_FromCode_ZhHans_ReturnsChineseSimplified()
        {
            // Act
            var locale = LocaleExtensions.FromCode("zh-Hans");

            // Assert
            Assert.AreEqual(SupportedLocale.ChineseSimplified, locale);
        }

        [Test]
        public void LocaleExtensions_FromCode_ZhHant_ReturnsChineseTraditional()
        {
            // Act
            var locale = LocaleExtensions.FromCode("zh-Hant");

            // Assert
            Assert.AreEqual(SupportedLocale.ChineseTraditional, locale);
        }

        [Test]
        public void LocaleExtensions_FromCode_InvalidCode_ReturnsEnglishFallback()
        {
            // Act
            var locale = LocaleExtensions.FromCode("invalid-code");

            // Assert - 无效代码应回退到英语
            Assert.AreEqual(SupportedLocale.English, locale);
        }

        [Test]
        public void LocaleExtensions_FromCode_EmptyString_ReturnsEnglishFallback()
        {
            // Act
            var locale = LocaleExtensions.FromCode("");

            // Assert
            Assert.AreEqual(SupportedLocale.English, locale);
        }

        [Test]
        public void LocaleExtensions_FromCode_Null_ReturnsEnglishFallback()
        {
            // Act
            var locale = LocaleExtensions.FromCode(null);

            // Assert
            Assert.AreEqual(SupportedLocale.English, locale);
        }

        // ==============================================
        // ILocalizationOptions 接口契约测试
        // ==============================================

        [Test]
        public void ILocalizationOptions_DefaultTableName_IsNotNullOrEmpty()
        {
            // Arrange
            ILocalizationOptions options = DefaultLocalizationOptions.Instance;

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(options.DefaultTableName), "DefaultTableName 不应为空");
        }

        [Test]
        public void ILocalizationOptions_FallbackLocaleCode_IsNotNullOrEmpty()
        {
            // Arrange
            ILocalizationOptions options = DefaultLocalizationOptions.Instance;

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(options.FallbackLocaleCode), "FallbackLocaleCode 不应为空");
        }

        // ==============================================
        // 双向映射一致性测试
        // ==============================================

        [Test]
        public void LocaleExtensions_ToCodeAndFromCode_AreConsistent()
        {
            // 测试所有枚举值的双向映射一致性
            foreach (SupportedLocale locale in System.Enum.GetValues(typeof(SupportedLocale)))
            {
                // Act
                var code = locale.ToCode();
                var backToLocale = LocaleExtensions.FromCode(code);

                // Assert
                Assert.AreEqual(locale, backToLocale, $"双向映射不一致：{locale} -> {code} -> {backToLocale}");
            }
        }
    }
}

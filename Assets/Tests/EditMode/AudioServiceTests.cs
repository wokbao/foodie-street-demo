using System.Collections.Generic;
using NUnit.Framework;
using Game.Audio.Runtime;

namespace Tests.EditMode
{
    /// <summary>
    /// AudioService 单元测试
    /// 
    /// <para><b>测试范围</b>：</para>
    /// <list type="bullet">
    ///   <item>AudioConfig 配置验证</item>
    ///   <item>AudioChannel 枚举值</item>
    /// </list>
    /// 
    /// <para><b>注意</b>：</para>
    /// AudioService 的播放功能需要 Unity 运行时环境，
    /// 这里只测试不依赖运行时的纯逻辑部分。
    /// </summary>
    public class AudioServiceTests
    {
        #region AudioConfig 验证测试

        [Test]
        public void AudioConfig_Validate_WithValidConfig_ReturnsTrue()
        {
            // Arrange
            var config = UnityEngine.ScriptableObject.CreateInstance<AudioConfig>();

            // Act
            bool isValid = config.Validate(out List<string> errors);

            // Assert
            Assert.IsTrue(isValid, "默认配置应该是有效的");
            Assert.IsEmpty(errors, "不应该有错误消息");

            // Cleanup
            UnityEngine.Object.DestroyImmediate(config);
        }

        [Test]
        public void AudioConfig_GetDefaultVolume_ReturnsCorrectValues()
        {
            // Arrange
            var config = UnityEngine.ScriptableObject.CreateInstance<AudioConfig>();

            // Act & Assert
            Assert.AreEqual(1f, config.DefaultMasterVolume, "主音量默认应为 1");
            Assert.AreEqual(0.7f, config.DefaultBGMVolume, "BGM 音量默认应为 0.7");
            Assert.AreEqual(0.8f, config.DefaultSFXVolume, "SFX 音量默认应为 0.8");

            // Cleanup
            UnityEngine.Object.DestroyImmediate(config);
        }

        [Test]
        public void AudioConfig_GetDefaultVolume_ByChannel_ReturnsCorrectValue()
        {
            // Arrange
            var config = UnityEngine.ScriptableObject.CreateInstance<AudioConfig>();

            // Act & Assert
            Assert.AreEqual(config.DefaultMasterVolume, config.GetDefaultVolume(AudioChannel.Master));
            Assert.AreEqual(config.DefaultBGMVolume, config.GetDefaultVolume(AudioChannel.BGM));
            Assert.AreEqual(config.DefaultSFXVolume, config.GetDefaultVolume(AudioChannel.SFX));
            Assert.AreEqual(config.DefaultUIVolume, config.GetDefaultVolume(AudioChannel.UI));
            Assert.AreEqual(config.DefaultVoiceVolume, config.GetDefaultVolume(AudioChannel.Voice));

            // Cleanup
            UnityEngine.Object.DestroyImmediate(config);
        }

        #endregion

        #region AudioChannel 枚举测试

        [Test]
        public void AudioChannel_HasExpectedValues()
        {
            // Assert
            Assert.AreEqual(0, (int)AudioChannel.Master);
            Assert.AreEqual(1, (int)AudioChannel.BGM);
            Assert.AreEqual(2, (int)AudioChannel.SFX);
            Assert.AreEqual(3, (int)AudioChannel.UI);
            Assert.AreEqual(4, (int)AudioChannel.Voice);
        }

        [Test]
        public void AudioChannel_HasFiveValues()
        {
            // Act
            var values = System.Enum.GetValues(typeof(AudioChannel));

            // Assert
            Assert.AreEqual(5, values.Length, "AudioChannel 应该有 5 个值");
        }

        #endregion
    }
}

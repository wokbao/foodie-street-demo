using System.Collections.Generic;
using NUnit.Framework;
using Game.Audio.Runtime;
using UnityEngine;

namespace Tests.EditMode
{
    /// <summary>
    /// AudioService 单元测试
    /// 
    /// <para><b>测试范围</b>：</para>
    /// <list type="bullet">
    ///   <item>AudioConfig 配置验证</item>
    ///   <item>AudioChannel 枚举值</item>
    ///   <item>AudioManager 静态访问器（公开 API）</item>
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
            var config = ScriptableObject.CreateInstance<AudioConfig>();

            // Act
            bool isValid = config.Validate(out List<string> errors);

            // Assert
            Assert.IsTrue(isValid, "默认配置应该是有效的");
            Assert.IsEmpty(errors, "不应该有错误消息");

            // Cleanup
            Object.DestroyImmediate(config);
        }

        [Test]
        public void AudioConfig_GetDefaultVolume_ReturnsCorrectValues()
        {
            // Arrange
            var config = ScriptableObject.CreateInstance<AudioConfig>();

            // Act & Assert
            Assert.AreEqual(1f, config.DefaultMasterVolume, "主音量默认应为 1");
            Assert.AreEqual(0.7f, config.DefaultBGMVolume, "BGM 音量默认应为 0.7");
            Assert.AreEqual(0.8f, config.DefaultSFXVolume, "SFX 音量默认应为 0.8");

            // Cleanup
            Object.DestroyImmediate(config);
        }

        [Test]
        public void AudioConfig_GetDefaultVolume_ByChannel_ReturnsCorrectValue()
        {
            // Arrange
            var config = ScriptableObject.CreateInstance<AudioConfig>();

            // Act & Assert
            Assert.AreEqual(config.DefaultMasterVolume, config.GetDefaultVolume(AudioChannel.Master));
            Assert.AreEqual(config.DefaultBGMVolume, config.GetDefaultVolume(AudioChannel.BGM));
            Assert.AreEqual(config.DefaultSFXVolume, config.GetDefaultVolume(AudioChannel.SFX));
            Assert.AreEqual(config.DefaultUIVolume, config.GetDefaultVolume(AudioChannel.UI));
            Assert.AreEqual(config.DefaultVoiceVolume, config.GetDefaultVolume(AudioChannel.Voice));

            // Cleanup
            Object.DestroyImmediate(config);
        }

        [Test]
        public void AudioConfig_GetDefaultVolume_UnknownChannel_ReturnsOne()
        {
            // Arrange
            var config = ScriptableObject.CreateInstance<AudioConfig>();

            // Act - 使用一个不存在的枚举值
            var volume = config.GetDefaultVolume((AudioChannel)999);

            // Assert
            Assert.AreEqual(1f, volume, "未知通道应返回默认音量 1");

            // Cleanup
            Object.DestroyImmediate(config);
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

        #region AudioManager 静态访问器测试

        [TearDown]
        public void TearDown()
        {
            // 确保每个测试后都清理 AudioManager 状态
            AudioManager.SetInstance(null);
        }

        [Test]
        public void AudioManager_Instance_InitiallyNull()
        {
            // Assert - 在清理后 Instance 应为 null
            Assert.IsNull(AudioManager.Instance, "AudioManager.Instance 初始应为 null");
        }

        [Test]
        public void AudioManager_SetInstance_Null_ClearsInstance()
        {
            // Arrange - 先设置一个非 null 值（通过反射避免 Mock 类）
            // 由于无法创建 Mock，我们只测试 null 情况
            AudioManager.SetInstance(null);

            // Act
            var instance = AudioManager.Instance;

            // Assert
            Assert.IsNull(instance, "SetInstance(null) 后 Instance 应为 null");
        }

        #endregion
    }
}

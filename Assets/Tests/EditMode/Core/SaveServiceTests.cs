using System;
using NUnit.Framework;
using Core.Feature.Save.Abstractions;

namespace Tests.EditMode.Core
{
    /// <summary>
    /// SaveService 单元测试
    /// 注意：SaveService 依赖 Application.persistentDataPath，需要 PlayMode
    /// 这里只测试 SaveOptions 和 SaveSlotInfo 结构
    /// </summary>
    public class SaveServiceTests
    {
        // ==============================================
        // 测试 1：验证 SaveOptions 默认值
        // ==============================================
        [Test]
        public void SaveOptions_Default_HasCorrectValues()
        {
            // Act
            var options = SaveOptions.Default;

            // Assert
            Assert.IsFalse(options.Encrypt);
            Assert.IsTrue(options.PrettyPrint);
            Assert.AreEqual(".sav", options.FileExtension);
        }

        // ==============================================
        // 测试 2：验证 SaveOptions 可配置
        // ==============================================
        [Test]
        public void SaveOptions_CanBeConfigured()
        {
            // Arrange & Act
            var options = new SaveOptions
            {
                Encrypt = true,
                EncryptionKey = "TestKey123",
                PrettyPrint = false,
                FileExtension = ".json"
            };

            // Assert
            Assert.IsTrue(options.Encrypt);
            Assert.AreEqual("TestKey123", options.EncryptionKey);
            Assert.IsFalse(options.PrettyPrint);
            Assert.AreEqual(".json", options.FileExtension);
        }

        // ==============================================
        // 测试 3：验证 SaveSlotInfo 结构
        // ==============================================
        [Test]
        public void SaveSlotInfo_CanBeCreated()
        {
            // Arrange & Act
            var slotInfo = new SaveSlotInfo
            {
                SlotId = "slot1",
                DisplayName = "存档 1",
                LastSaveTime = new DateTime(2026, 1, 5, 12, 0, 0),
                FileSizeBytes = 1024
            };

            // Assert
            Assert.AreEqual("slot1", slotInfo.SlotId);
            Assert.AreEqual("存档 1", slotInfo.DisplayName);
            Assert.AreEqual(2026, slotInfo.LastSaveTime.Year);
            Assert.AreEqual(1024, slotInfo.FileSizeBytes);
        }

        // ==============================================
        // 测试 4：验证 SaveSlotInfo 元数据
        // ==============================================
        [Test]
        public void SaveSlotInfo_Metadata_DefaultsToEmptyDictionary()
        {
            // Arrange
            var slotInfo = new SaveSlotInfo();

            // Assert
            Assert.IsNotNull(slotInfo.Metadata);
            Assert.AreEqual(0, slotInfo.Metadata.Count);
        }

        [Test]
        public void SaveSlotInfo_Metadata_CanStoreCustomData()
        {
            // Arrange
            var slotInfo = new SaveSlotInfo();

            // Act
            slotInfo.Metadata["Level"] = "5";
            slotInfo.Metadata["PlayTime"] = "3600";

            // Assert
            Assert.AreEqual(2, slotInfo.Metadata.Count);
            Assert.AreEqual("5", slotInfo.Metadata["Level"]);
            Assert.AreEqual("3600", slotInfo.Metadata["PlayTime"]);
        }
    }
}

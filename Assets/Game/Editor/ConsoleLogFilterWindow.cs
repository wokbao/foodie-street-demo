using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// Console 日志过滤和复制工具
    /// 用于快速复制关键日志信息，自动过滤堆栈跟踪和无关内容
    /// </summary>
    public class ConsoleLogFilterWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private string _filterTag = "";
        private bool _excludeStackTrace = true;
        private bool _excludeTimestamps = true;
        private bool _onlyErrors = false;
        private bool _onlyWarnings = false;
        private string _filteredLogs = "";
        private int _logCount = 0;

        [MenuItem("Tools/Console Log Filter")]
        public static void ShowWindow()
        {
            var window = GetWindow<ConsoleLogFilterWindow>("Console Filter");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Console 日志过滤器", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // 过滤选项
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("过滤选项", EditorStyles.boldLabel);

            _filterTag = EditorGUILayout.TextField("标签过滤 (留空=全部)", _filterTag);
            EditorGUILayout.HelpBox("例如: [LoadingHud] 或 [ConfigLoader]", MessageType.Info);

            _excludeStackTrace = EditorGUILayout.Toggle("排除堆栈跟踪", _excludeStackTrace);
            _excludeTimestamps = EditorGUILayout.Toggle("排除时间戳", _excludeTimestamps);

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("日志类型", EditorStyles.boldLabel);
            _onlyErrors = EditorGUILayout.Toggle("仅错误 (Error)", _onlyErrors);
            _onlyWarnings = EditorGUILayout.Toggle("仅警告 (Warning)", _onlyWarnings);

            GUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // 操作按钮
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("🔍 提取日志", GUILayout.Height(30)))
            {
                ExtractLogs();
            }

            if (GUILayout.Button("📋 复制到剪贴板", GUILayout.Height(30)))
            {
                CopyToClipboard();
            }

            if (GUILayout.Button("🗑 清空", GUILayout.Height(30)))
            {
                _filteredLogs = "";
                _logCount = 0;
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"已提取日志: {_logCount} 条", EditorStyles.miniLabel);

            // 显示过滤后的日志
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("过滤结果", EditorStyles.boldLabel);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            EditorGUILayout.TextArea(_filteredLogs, EditorStyles.wordWrappedLabel, GUILayout.ExpandHeight(true));

            EditorGUILayout.EndScrollView();
        }

        private void ExtractLogs()
        {
            try
            {
                var logs = GetConsoleLogs();
                var filtered = FilterLogs(logs);

                var sb = new StringBuilder();
                foreach (var log in filtered)
                {
                    sb.AppendLine(log);
                    sb.AppendLine(); // 空行分隔
                }

                _filteredLogs = sb.ToString();
                _logCount = filtered.Count;

                Debug.Log($"✅ 已提取 {_logCount} 条日志");
            }
            catch (Exception ex)
            {
                Debug.LogError($"提取日志失败: {ex.Message}");
                _filteredLogs = $"错误: {ex.Message}\n{ex.StackTrace}";
            }
        }

        private List<string> GetConsoleLogs()
        {
            var logs = new List<string>();

            // 使用反射访问 Unity Console 的内部 API
            var logEntriesType = Type.GetType("UnityEditor.LogEntries,UnityEditor");
            if (logEntriesType == null)
            {
                throw new Exception("无法找到 LogEntries 类型");
            }

            // 先清除折叠状态，确保获取所有日志
            var setCon​soleFlag = logEntriesType.GetMethod("SetConsoleFlag", BindingFlags.Static | BindingFlags.Public);
            if (setConsoleFlag != null)
            {
                // 1 = kClearOnPlay, 2 = kErrorPause, 4 = kCollapse, 8 = kClearOnBuild, 16 = kStopForError
                // 我们暂时禁用折叠（清除bit 4）
                setConsoleFlag.Invoke(null, new object[] { 4, false });
            }

            // 获取日志数量
            var getCountMethod = logEntriesType.GetMethod("GetCount", BindingFlags.Static | BindingFlags.Public);
            if (getCountMethod == null)
            {
                throw new Exception("无法找到 GetCount 方法");
            }

            int count = (int)getCountMethod.Invoke(null, null);
            Debug.Log($"[ConsoleFilter] Unity Console 总共有 {count} 条日志");

            // 获取每条日志
            var startGettingEntriesMethod = logEntriesType.GetMethod("StartGettingEntries", BindingFlags.Static | BindingFlags.Public);
            var endGettingEntriesMethod = logEntriesType.GetMethod("EndGettingEntries", BindingFlags.Static | BindingFlags.Public);
            var getEntryInternalMethod = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public);

            if (startGettingEntriesMethod != null)
            {
                startGettingEntriesMethod.Invoke(null, null);
            }

            try
            {
                var logEntryType = Type.GetType("UnityEditor.LogEntry,UnityEditor");

                for (int i = 0; i < count; i++)
                {
                    var logEntry = Activator.CreateInstance(logEntryType);
                    var args = new object[] { i, logEntry };

                    bool success = (bool)getEntryInternalMethod.Invoke(null, args);

                    if (!success)
                    {
                        Debug.LogWarning($"[ConsoleFilter] 无法获取日志条目 {i}");
                        continue;
                    }

                    // 获取日志信息
                    var messageField = logEntryType.GetField("message");
                    var modeField = logEntryType.GetField("mode");

                    if (messageField != null && modeField != null)
                    {
                        string message = (string)messageField.GetValue(logEntry);
                        int mode = (int)modeField.GetValue(logEntry);

                        // mode: 0=Log, 1=Warning, 2=Error (实际是 LogType 枚举)
                        // LogType.Error = 0, Warning = 1, Log = 2, Assert = 3, Exception = 4
                        // 但 mode 字段存储的值可能不同，我们根据实际值判断
                        string prefix = "";
                        if (mode == 0)  // Error/Exception
                        {
                            prefix = "[ERROR] ";
                        }
                        else if (mode == 1)  // Warning
                        {
                            prefix = "[WARNING] ";
                        }

                        logs.Add(prefix + message);
                    }
                }

                Debug.Log($"[ConsoleFilter] 成功提取 {logs.Count} 条原始日志");
            }
            finally
            {
                if (endGettingEntriesMethod != null)
                {
                    endGettingEntriesMethod.Invoke(null, null);
                }
            }

            return logs;
        }

        private List<string> FilterLogs(List<string> logs)
        {
            var filtered = new List<string>();

            foreach (var log in logs)
            {
                // 过滤日志类型
                if (_onlyErrors && !log.StartsWith("[ERROR]"))
                {
                    continue;
                }

                if (_onlyWarnings && !log.StartsWith("[WARNING]"))
                {
                    continue;
                }

                // 过滤标签
                if (!string.IsNullOrWhiteSpace(_filterTag))
                {
                    if (!log.Contains(_filterTag))
                    {
                        continue;
                    }
                }

                // 处理日志内容
                string processedLog = log;

                // 排除堆栈跟踪
                if (_excludeStackTrace)
                {
                    var lines = processedLog.Split('\n');
                    var mainMessage = new StringBuilder();

                    foreach (var line in lines)
                    {
                        var trimmed = line.TrimStart();

                        // 跳过堆栈跟踪行，识别多种格式：
                        // 1. Unity 标准格式: "at ..."
                        // 2. Unity 堆栈: "UnityEngine.XXX:方法名 (at Assets/...)"
                        // 3. UniTask 堆栈: "Cysharp.Threading.Tasks.XXX:方法名 (at ./Library/...)"
                        // 4. System 堆栈: "System.Runtime.XXX:方法名"
                        // 5. 包含文件路径: "(at XXX.cs:行号)"
                        // 6. 命名空间.类名:方法名 格式

                        bool isStackTrace =
                           trimmed.StartsWith("at ") ||                                    // Unity 标准
                           trimmed.Contains("(at ") ||                                     // 包含文件路径
                           trimmed.Contains(".cs:") ||                                     // 源文件引用
                           trimmed.StartsWith("UnityEngine.") ||                           // Unity 引擎
                           trimmed.StartsWith("UnityEditor.") ||                           // Unity 编辑器
                           trimmed.StartsWith("VContainer.") ||                            // VContainer
                           trimmed.StartsWith("Cysharp.Threading.Tasks.") ||               // UniTask
                           trimmed.StartsWith("System.Runtime.") ||                        // System Runtime
                           (trimmed.StartsWith("Core.") && !trimmed.StartsWith("[") &&    // 项目堆栈（排除日志）
                            trimmed.Contains(":") && trimmed.Contains("(")) ||
                           (trimmed.StartsWith("Game.") && !trimmed.StartsWith("[") &&    // 项目堆栈（排除日志）
                            trimmed.Contains(":") && trimmed.Contains("("));

                        if (isStackTrace)
                        {
                            continue;
                        }

                        mainMessage.AppendLine(line);
                    }

                    processedLog = mainMessage.ToString().TrimEnd();
                }

                // 排除时间戳（Unity日志格式）
                if (_excludeTimestamps)
                {
                    // 移除形如 "HH:MM:SS" 的时间戳
                    processedLog = System.Text.RegularExpressions.Regex.Replace(
                        processedLog,
                        @"^\d{2}:\d{2}:\d{2}\.?\d*\s*",
                        ""
                    );
                }

                if (!string.IsNullOrWhiteSpace(processedLog))
                {
                    filtered.Add(processedLog.Trim());
                }
            }

            return filtered;
        }

        private void CopyToClipboard()
        {
            if (string.IsNullOrWhiteSpace(_filteredLogs))
            {
                Debug.LogWarning("⚠️ 没有日志可复制，请先点击「提取日志」");
                return;
            }

            EditorGUIUtility.systemCopyBuffer = _filteredLogs;
            Debug.Log($"✅ 已复制 {_logCount} 条日志到剪贴板");
        }
    }
}

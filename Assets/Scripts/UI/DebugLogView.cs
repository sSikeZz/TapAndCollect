using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TapAndCollect.UI
{
    public class DebugLogView : MonoBehaviour
    {
        [SerializeField] private Text logText;
        [SerializeField] private int maxLines = 4;

        private readonly Queue<string> lines = new Queue<string>();

        private void Awake()
        {
            if (logText == null)
            {
                logText = GetComponent<Text>();
            }
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string condition, string stackTrace, LogType type)
        {
            AddLine($"{type}: {condition}");
        }

        private void AddLine(string line)
        {
            lines.Enqueue(line);
            while (lines.Count > maxLines)
            {
                lines.Dequeue();
            }

            if (logText != null)
            {
                logText.text = string.Join("\n", lines);
            }
        }
    }
}

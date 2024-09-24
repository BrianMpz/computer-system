using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    public TMP_Text consoleText;
    public ScrollRect scrollRect;
    
    private List<string> logEntries = new List<string>();
    public bool isCollapsed = false;

    private void Awake()
    {
        // Redirect debug logs to our custom method
        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string logText, string stackTrace, LogType logType)
    {
        // Append the new log to the consoleText
        consoleText.text += logText + "\n" + "\n";
    }

    private void UpdateConsoleText()
    {
        // Clear the existing text
        consoleText.text = "";

        // Display the log entries based on the collapsed state
        if (isCollapsed)
        {
            // Display a summary when collapsed
            consoleText.text = logEntries.Count + " log entries (Click to expand)\n";
        }
        else
        {
            // Display all log entries when expanded
            foreach (string logEntry in logEntries)
            {
                consoleText.text += logEntry + "\n" + "\n";
            }
        }
    }

    // Toggle between collapsed and expanded states when the consoleText is clicked
    public void ToggleCollapse()
    {
        isCollapsed = !isCollapsed;
        UpdateConsoleText();
    }
}

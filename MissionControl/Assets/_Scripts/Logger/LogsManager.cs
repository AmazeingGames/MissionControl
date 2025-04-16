using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;

// Provides convenient access to toggle logging from various scripts
public class LogsManager : MonoBehaviour
{
    [SerializeField] List<Logger> loggers = new List<Logger>();

    static LogsManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
        => instance = this;

    public static void Log(GameObject loggingObject, string message, UnityEngine.Object context = null)
        => Log(isWarning: false, loggingObject, message, context);

    public static void LogWarning(GameObject loggingObject, string message, UnityEngine.Object context = null)
        => Log(isWarning: true, loggingObject, message, context);

    static void Log(bool isWarning, GameObject loggingObject, string message, UnityEngine.Object context = null)
    {
        Assert.IsNotNull(instance, "Instance of logger should not be null");

        foreach (Logger _logger in instance.loggers)
            Assert.IsNotNull(_logger.LoggingObject, "Logging object should not be null");

        Logger logger = instance.loggers.Find(l => l.LoggingObject == loggingObject);
        Assert.IsNotNull(logger, $"Logging object has not properly been set up in the LogsManager. {loggingObject.name} is not contained added as a parameter in the logger");

        if (logger.MyTextColor != Logger.RichTextColor.none)
            message = $"<color={logger.MyTextColor}>{message}</color>";

        if (isWarning)
        {
            if (context == null)
                Debug.LogWarning(message);
            else
                Debug.LogWarning(message, context);
        }
        else if (logger.ShouldLog)
        {
            if (context == null)
                Debug.Log(message);
            else
                Debug.Log(message, context);
        }
    }
}

[Serializable]
class Logger
{
    public enum RichTextColor { none, aqua, black, blue, brown, cyan, darkblue, fuchsia, green, grey, lightblue, lime, magenta, maroon, navy, olive, orange, purple, red, silver, teal, white, yellow}
    [field: SerializeField] public GameObject LoggingObject { get; private set; }
    [field: SerializeField] public RichTextColor MyTextColor { get; private set; }
    [field: SerializeField] public bool ShouldLog { get; private set; }
}
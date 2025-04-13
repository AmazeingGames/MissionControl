using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class MonobehaviourExtension
{
    public static void Log(this MonoBehaviour monoBehaviour, string message, Object context = null)
    {
        Assert.IsNotNull(monoBehaviour, "Monobehaviour should not be null");

        LogsManager.Log(monoBehaviour, message, context);
    }

    public static void LogWarning(this MonoBehaviour monoBehaviour, string message, Object context = null)
    {
        Assert.IsNotNull(monoBehaviour, "Monobehaviour should not be null");

        LogsManager.LogWarning(monoBehaviour, message, context);
    }
}

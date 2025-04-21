using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class MonobehaviourExtension
{
    public static void Log(this MonoBehaviour monoBehaviour, string message, Object context = null)
    {
        Assert.IsNotNull(monoBehaviour, "Monobehaviour class should not be null");
        Assert.IsNotNull(monoBehaviour.gameObject, "Gameobject should not be null");

        LogsManager.Log(monoBehaviour.gameObject, message, context);
    }

    public static void LogWarning(this MonoBehaviour monoBehaviour, string message, Object context = null)
    {
        Assert.IsNotNull(monoBehaviour, "MonoBehaviour class should not be null");
        Assert.IsNotNull(monoBehaviour.gameObject, "GameObject should not be null");

        LogsManager.LogWarning(monoBehaviour.gameObject, message, context);
    }
}

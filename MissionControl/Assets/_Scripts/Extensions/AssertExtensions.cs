using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static UIManager;

public static class AssertExtensions
{
    public static void DictionaryMatchesEnumLength<key, value, enumType>(Dictionary<key, value> dictionary)
    {
        Assert.IsNotNull(dictionary, "Dictionary cannot be null");
        Assert.AreEqual(dictionary.Count, Enum.GetNames(typeof(enumType)).Length);
    }
}

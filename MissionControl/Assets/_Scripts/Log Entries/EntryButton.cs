using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntryButton : MonoBehaviour, IPointerClickHandler, IPageButton
{
    [SerializeField] TextMeshProUGUI display_TMP;
    EntryData entryData;

    public static EventHandler<ClickEntryEventArgs> ClickEntryEventHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickEntryEventHandler?.Invoke(this, new(entryData));
    }

    public void Initialize<T>(T data) where T : IPageData
    {
        entryData = data as EntryData;

        if (entryData != null)
        {
            gameObject.SetActive(true);
            display_TMP.text = entryData.displayName;
            LogsManager.Log(LogsManager.Instance.PageButtonLoggingObject, $"Initialized entry select button | Short Text: {this.entryData.displayName}");
        }
        else
            gameObject.SetActive(false);
    }
}

[Serializable]
public class EntryData : IPageData
{
    [SerializeField] public string displayName;
    [SerializeField] public TextAsset entryText;
}

public class ClickEntryEventArgs
{
    public readonly EntryData entryData;
    public ClickEntryEventArgs(EntryData entryData)
    {
        this.entryData = entryData;
    }
}

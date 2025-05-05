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
            display_TMP.text = entryData.DisplayName;
            LogsManager.Log(LogsManager.Instance.PageButtonLoggingObject, $"Initialized entry select button | Short Text: {this.entryData.DisplayName}");
        }
        else
            gameObject.SetActive(false);
    }
}

[Serializable]
public class EntryData : IPageData
{
    public string DisplayName { get; private set; }
    public string DisplayText { get; private set; }
    [SerializeField] TextAsset entryText;

    public void Initialize()
    {
        string[] lines = entryText ? entryText.text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries) : null;

        DisplayName = lines[0];

        for (int i = 1; i < lines.Length; i++)
            DisplayText += lines[i];
    }
}

public class ClickEntryEventArgs
{
    public readonly EntryData entryData;
    public ClickEntryEventArgs(EntryData entryData)
    {
        this.entryData = entryData;
    }
}

using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntryManager : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] int spacesAtEntryStart;
    [SerializeField] List<UnlockableLog> unlockableLogs;

    [Header("Text Tween")]
    [SerializeField] float entryEaseDuration;
    [SerializeField] ScrambleMode textScrambleMode;
    [SerializeField] Ease textEase;

    [Header("Components")]
    [SerializeField] TMPro.TextMeshProUGUI entry_TMP;
    [SerializeField] Transform entryButtonsParent;


    PageDisplayer pageDisplayer;
    List<EntryData> unlockedLogEntries = new();

    public static EventHandler<UnlockEntryEventArgs> UnlockEntryEventHandler;

    private void OnEnable()
    {
        EntryButton.ClickEntryEventHandler += HandleClickEntry;
        Window.SetWindowEventHandler += HandleSetWindow;
        RoomItem.ClickItemEventHandler += HandleClickItem;
        CameraUnlockButton.UnlockCameraEventHandler += HandleUnlockCamera;
    }

    private void OnDisable()
    {
        EntryButton.ClickEntryEventHandler -= HandleClickEntry;
        Window.SetWindowEventHandler -= HandleSetWindow;
        RoomItem.ClickItemEventHandler -= HandleClickItem;
        CameraUnlockButton.UnlockCameraEventHandler -= HandleUnlockCamera;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<EntryButton> entryButtons = new List<EntryButton>();
        for (int i = 0; i < entryButtonsParent.childCount; i++)
            entryButtons.Add(entryButtonsParent.GetChild(i).GetComponent<EntryButton>());

        pageDisplayer = new(entryButtons.ToList<IPageButton>());
    }

    void HandleUnlockCamera(object sender, UnlockCameraEventArgs e)
    {
        for (int i = 0; i < unlockableLogs.Count; i++)
        {
            if (!unlockableLogs[i].OnRoomUnlock.Contains(e.ipInformation.myRoom))
                continue;

            Debug.Log("unlocked entry by camera");
            OnUnlockEntry(unlockableLogs[i].EntryData);
        }
    }

    void HandleClickItem(object sender, ClickItemEventArgs e)
    {
        OnUnlockEntry(e.entryData);
    }

    void OnUnlockEntry(EntryData entryData)
    {
        if (unlockedLogEntries.Contains(entryData) || entryData == null || entryData.EntryText == null)
            return;
        unlockedLogEntries.Add(entryData);

        pageDisplayer.DisplayPageButtons<EntryData, EntryButton>(0, unlockedLogEntries, true);

        UnlockEntryEventHandler?.Invoke(this, new(entryData));
    }    

    void HandleSetWindow(object sender, SetWindowEventArgs e)
    {
        if (e.myWindowType != Window.WindowType.Logs)
            return;

        if (!e.isOpening)
            return;
        
        pageDisplayer.DisplayPageButtons<EntryData, EntryButton>(0, unlockedLogEntries, true);
    }

    void HandleClickEntry(object sender, ClickEntryEventArgs e)
    {
        string targetText = "";

        for (int i = 0; i < spacesAtEntryStart; i++)
            targetText += " ";

        targetText += e.entryData.DisplayText;

        entry_TMP.DOText(targetText, entryEaseDuration, true, textScrambleMode, null).SetEase(textEase);
    }
}

[Serializable] 
public class UnlockableLog
{
    [field: SerializeField] public EntryData EntryData { get; private set; }

    [field: SerializeField] public List<IPInformation.Room> OnRoomUnlock { get; private set; } = new();
}

public class UnlockEntryEventArgs : EventArgs
{
    public readonly EntryData entryData;

    public UnlockEntryEventArgs(EntryData entryData)
    {
        this.entryData = entryData;
    }
}
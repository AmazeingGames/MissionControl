using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntryManager : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] int spacesAtEntryStart;

    [Header("Text Tween")]
    [SerializeField] float entryEaseDuration;
    [SerializeField] ScrambleMode textScrambleMode;
    [SerializeField] Ease textEase;

    [Header("Components")]
    [SerializeField] TMPro.TextMeshProUGUI entry_TMP;
    [SerializeField] Transform entryButtonsParent;

    PageDisplayer pageDisplayer;
    List<EntryData> unlockedLogEntries = new();


    private void OnEnable()
    {
        EntryButton.ClickEntryEventHandler += HandleClickEntry;
        Window.SetWindowEventHandler += HandleSetWindow;
        RoomItem.UnlockLogsEventHandler += HandleUnlockLogs;
    }

    private void OnDisable()
    {
        EntryButton.ClickEntryEventHandler -= HandleClickEntry;
        Window.SetWindowEventHandler -= HandleSetWindow;
        RoomItem.UnlockLogsEventHandler -= HandleUnlockLogs;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<EntryButton> entryButtons = new List<EntryButton>();
        for (int i = 0; i < entryButtonsParent.childCount; i++) 
            entryButtons.Add(entryButtonsParent.GetChild(i).GetComponent<EntryButton>());

        pageDisplayer = new(entryButtons.ToList<IPageButton>());
    }

    void HandleUnlockLogs(object sender, UnlockLogsEventArgs e) 
    {
        unlockedLogEntries.Add(e.entryData);

        pageDisplayer.DisplayPageButtons<EntryData, EntryButton>(0, unlockedLogEntries, true);
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

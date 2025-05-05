using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntryManager : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] List<EntryData> logEntries;
    [SerializeField] int spacesAtEntryStart;

    [Header("Text Tween")]
    [SerializeField] float entryEaseDuration;
    [SerializeField] ScrambleMode textScrambleMode;
    [SerializeField] Ease textEase;

    [Header("Components")]
    [SerializeField] TMPro.TextMeshProUGUI entry_TMP;
    [SerializeField] Transform entryButtonsParent;

    PageDisplayer pageDisplayer;

    private void OnEnable()
    {
        EntryButton.ClickEntryEventHandler += HandleClickEntry;
        Window.SetWindowEventHandler += HandleSetWindow;
    }

    private void OnDisable()
    {
        EntryButton.ClickEntryEventHandler -= HandleClickEntry;
        Window.SetWindowEventHandler -= HandleSetWindow;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<EntryButton> entryButtons = new List<EntryButton>();
        for (int i = 0; i < entryButtonsParent.childCount; i++) 
            entryButtons.Add(entryButtonsParent.GetChild(i).GetComponent<EntryButton>());

        pageDisplayer = new(entryButtons.ToList<IPageButton>());
    }

    void HandleSetWindow(object sender, SetWindowEventArgs e)
    {
        if (e.myWindowType != Window.WindowType.Logs)
            return;

        if (!e.isOpening)
            return;
        
        int pageToDisplay = pageDisplayer.LastViewedPage(logEntries);
        pageDisplayer.DisplayPageButtons<EntryData, EntryButton>(pageToDisplay, logEntries, true);
    }

    void HandleClickEntry(object sender, ClickEntryEventArgs e)
    {
        string targetText = "";

        for (int i = 0; i < spacesAtEntryStart; i++)
            targetText += " ";

        targetText += e.entryData.entryText.text;

        entry_TMP.DOText(targetText, entryEaseDuration, true, textScrambleMode, null).SetEase(textEase);

    }
}

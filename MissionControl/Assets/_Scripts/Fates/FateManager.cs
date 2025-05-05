using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class FateManager : MonoBehaviour, ISelectFate
{
    [Header("Settings")]
    [SerializeField] bool shouldRememberPage;

    [Header("Buttons")]
    [SerializeField] Transform fateButtonsParent;

    [Header("All Fates")]
    [SerializeField] FateData unkownFate;
    [SerializeField] List<FateData> allFates;

    [Header("Correct Fates")]
    [SerializeField] FateData LynnsFate;
    [SerializeField] FateData AlvarosFate;
    [SerializeField] FateData LizsFate;
    [SerializeField] FateData MelsFate;
    [SerializeField] FateData BlakesFate;

    [Header("Display Text")]
    [SerializeField] TextMeshProUGUI crewMateFate_TMP;
    [SerializeField] TextMeshProUGUI selectFate_TMP;

    List<FateData> viewingFates = null;

    public static EventHandler SelectFateEventHandler;

    static List<MemberFateData> membersFateData;

    CrewData crewData;

    // Describes the path of the page data we've selected in order to accomodate for subfates (i.e. Suicide -> Gun)
    readonly List<FateData> selectedFates = new();
    readonly Dictionary<List<FateData>, int> FatesToPageNumber = new();

    PageDisplayer pageDisplayer;

    private void OnEnable()
    {
        NotesTab.ClickTabEventHandler += HandleClickTab;
        PopupsManager.TogglePopupEventHandler += HandleTogglePopup;
    }

    private void OnDisable()
    {
        NotesTab.ClickTabEventHandler -= HandleClickTab;
        PopupsManager.TogglePopupEventHandler -= HandleTogglePopup;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<FateSelectButton> fateSelectButtons = new();
        for (int i = 0; i < fateButtonsParent.childCount; i++)
            fateSelectButtons.Add(fateButtonsParent.GetChild(i).GetComponent<FateSelectButton>());

        pageDisplayer = new PageDisplayer(fateSelectButtons.ToList<IPageButton>());

        FateSelectButton.selectFateHandler = this;

        membersFateData = new()
        {
            { new(CrewData.Name.Socha,  LynnsFate,   unkownFate) },
            { new(CrewData.Name.Zen, AlvarosFate, unkownFate) },
            { new(CrewData.Name.Mel,    MelsFate,    unkownFate) },
            { new(CrewData.Name.Blake,  BlakesFate,  unkownFate) },
            { new(CrewData.Name.Ethena,    LizsFate,    unkownFate) },
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (PopupsManager.OpenPopups.Contains(PopupsManager.Popup.Fate))
        {
            if (Input.GetKeyDown(KeyCode.D))
                pageDisplayer.DisplayNext<FateData, FateSelectButton>(true);
            else if (Input.GetKeyDown(KeyCode.A))
                pageDisplayer.DisplayPrevious<FateData, FateSelectButton>(true);
        }
    }

    public static FateData GetGuessedFate(CrewData.Name name)
        => membersFateData.First(membersFateData => membersFateData.myName == name).GuessedFate;

    MemberFateData GetMemberFate(CrewData.Name name)
        => membersFateData.First(d => d.myName == name);

    public void HandleSelectFate(FateArguments fateArguments)
    {
        var fateData = fateArguments.fate;

        if (selectedFates == null)
            this.Log("Selected fate should not be null");

        if (selectedFates.Count > 0)
        {
            FateData lastSelectedFate = selectedFates[^1];

            if (!lastSelectedFate.SubFates.Contains(fateData))
            {
                this.Log("Fate not contained as a subfate in previously selected fate");
                selectedFates.Clear();
                selectFate_TMP.text = "";
            }
        }
        else
            selectFate_TMP.text = "";
        selectedFates.Add(fateData);

        this.Log($"Selected fates count: {selectedFates.Count}");

        Assert.IsNotNull(crewData, "Crew data should not be null");
        Assert.IsNotNull(fateData, "Fate data should not be null");

        crewMateFate_TMP.text = $"{crewData.MyName} {fateData.FullDisplay}";
        selectFate_TMP.text += $"{fateData.FullDisplay}{(fateData.FullDisplay != "" ? " " : "")}";

        if (fateData.SubFates.Count == 0)
        {
            GetMemberFate(crewData.MyName).SetGuessedFate(fateData);

            for (int i = 0; i < membersFateData.Count; i++)
            {
                if (membersFateData[i].correctFate == membersFateData[i].GuessedFate)
                    this.Log($"Correctly guessed the fate of {membersFateData[i].myName}");

                SelectFateEventHandler?.Invoke(this, new());
            }
        }
        else
        {
            int lastViewedSubPage = pageDisplayer.LastViewedPage(fateData.SubFates);
            pageDisplayer.DisplayPageButtons<FateData, FateSelectButton>(lastViewedSubPage, fateData.SubFates, true);
        }
    }

    void HandleTogglePopup(object sender, TogglePopupsEventArgs e)
    {
        switch (e.popup)
        {
            case PopupsManager.Popup.SelectAttacker:
            case PopupsManager.Popup.Fate:
                break;

            case PopupsManager.Popup.SelectFate:
                if (e.isOpening)
                {
                    int pageNumber = shouldRememberPage ? pageDisplayer.LastViewedPage(allFates) : 0;
                    this.Log($"Opened fates at page: {pageNumber}");
                    pageDisplayer.DisplayPageButtons<FateData, FateSelectButton>(pageNumber, allFates, true);
                }
                break;
        }
    }

    void HandleClickTab(object sender, ClickTabEventArgs e)
    {
        crewData = e.crewData;

        selectFate_TMP.text = $"{GetGuessedFate(e.crewData.MyName).FullDisplay}";
    }
}

class MemberFateData
{
    public readonly CrewData.Name myName;
    public readonly FateData correctFate;
    public FateData GuessedFate { get; private set; }

    public void SetGuessedFate(FateData fateData)
        => GuessedFate = fateData;

    public MemberFateData(CrewData.Name myName, FateData correctFate, FateData guessedFate)
    {
        this.myName = myName;
        this.correctFate = correctFate;
        GuessedFate = guessedFate;
    }
}
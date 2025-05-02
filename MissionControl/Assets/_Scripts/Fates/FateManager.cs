using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
    readonly List<FateSelectButton> fateSelectButtons = new();

    List<FateData> viewingFates = null;

    public static EventHandler SelectFateEventHandler;

    static List<MemberFateData> membersFateData;

    CrewData crewData;

    readonly List<FateData> selectedFates = new();
    readonly Dictionary<List<FateData>, int> FatesToPageNumber = new();

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
        FateSelectButton.selectFateHandler = this;

        membersFateData = new()
        {
            { new(CrewData.Name.Socha,  LynnsFate,   unkownFate) },
            { new(CrewData.Name.Alvaro, AlvarosFate, unkownFate) },
            { new(CrewData.Name.Mel,    MelsFate,    unkownFate) },
            { new(CrewData.Name.Blake,  BlakesFate,  unkownFate) },
            { new(CrewData.Name.Liz,    LizsFate,    unkownFate) },
        };

        for (int i = 0; i < fateButtonsParent.childCount; i++)
            fateSelectButtons.Add(fateButtonsParent.GetChild(i).GetComponent<FateSelectButton>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            DisplayFates(GetLastViewedPage(viewingFates) + 1, viewingFates);
        else if (Input.GetKeyDown(KeyCode.A))
            DisplayFates(GetLastViewedPage(viewingFates) - 1, viewingFates);
    }

    int GetLastViewedPage(List<FateData> fatesToView)
    {
        if (fatesToView == null)
        {
            this.LogWarning("Trying to view a null list of fates");
            return -1;
        }    

        if (FatesToPageNumber.TryGetValue(fatesToView, out int pageNumber))
            return pageNumber;
        else
            return 0;
    }

    public static FateData GetGuessedFate(CrewData.Name name)
        => membersFateData.First(membersFateData => membersFateData.myName == name).GuessedFate;

    MemberFateData GetMemberFate(CrewData.Name name)
        => membersFateData.First(d => d.myName == name);

    public void HandleSelectFate(FateArguments fateArguments)
    {
        var fateData = fateArguments.fate;

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
            DisplayFates(GetLastViewedPage(fateData.SubFates), fateData.SubFates);
    }

    void HandleTogglePopup(object sender, TogglePopupsArgs e)
    {
        switch (e.popup)
        {
            case PopupsManager.Popup.SelectAttacker:
            case PopupsManager.Popup.Fate:
                break;

            case PopupsManager.Popup.SelectFate:
                if (e.isOpening)
                {
                    int pageToOpen = shouldRememberPage ? GetLastViewedPage(allFates) : 0;
                    this.Log($"Opened fates at page: {pageToOpen}");
                    DisplayFates(pageToOpen, allFates);
                }
                break;
        }
    }

    void HandleClickTab(object sender, ClickTabEventArgs e)
    {
        crewData = e.crewData;

        selectFate_TMP.text = $"{GetGuessedFate(e.crewData.MyName).FullDisplay}";
    }

    void DisplayFates(int pageNumber, List<FateData> fatesToDisplay)
    {
        if (fatesToDisplay == null)
        {
            this.LogWarning("Fates to display are null");
            return;
        }

        if (pageNumber < 0 || pageNumber * fateSelectButtons.Count > fatesToDisplay.Count)
        {
            this.LogWarning("Page number is out of list range");
            return;
        }

        this.Log($"Displayed page {pageNumber} for fates list {fatesToDisplay}");
        if (FatesToPageNumber.TryGetValue(fatesToDisplay, out _))
            FatesToPageNumber[fatesToDisplay] = pageNumber;
        else
            FatesToPageNumber.Add(fatesToDisplay, pageNumber);

        this.Log($"fatesToPageNumber dictionary count: {FatesToPageNumber.Count}");
        this.Log($"Selected Fates Count on Display Fates: {selectedFates.Count}");
        
        viewingFates = fatesToDisplay;

        for (int i = 0; i < fateSelectButtons.Count; i++)
        {
            FateSelectButton button = fateSelectButtons[i];

            int fateToSelect = i + (pageNumber * fateSelectButtons.Count);
            if (fateToSelect < fatesToDisplay.Count)
            {
                var fate = fatesToDisplay[fateToSelect];
                button.InitializeFate(fate);
            }
            else
                button.InitializeFate(null);
        }
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
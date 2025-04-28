using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FateManager : MonoBehaviour, ISelectFate
{
    [Header("Settings")]
    [SerializeField] bool rememberPageNumber;

    [Header("Buttons")]
    [SerializeField] Transform fateButtonsParent;

    [Header("All Fates")]
    [SerializeField] FateData unkownFate;
    [SerializeField] List<FateData> fates;

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
    int currentPage = -1;

    readonly List<FateData> viewingFates = null;

    public static EventHandler SelectFateEventHandler;

    static List<MemberFateData> membersFateData;

    CrewData crewData;

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
            DisplayFates(currentPage + 1, viewingFates);
        else if (Input.GetKeyDown(KeyCode.A))
            DisplayFates(currentPage - 1, viewingFates);
    }

    public static FateData GetGuessedFate(CrewData.Name name)
        => membersFateData.First(d => d.myName == name).GuessedFate;

    MemberFateData GetMemberFate(CrewData.Name name)
        => membersFateData.First(d => d.myName == name);

    public void HandleSelectFate(FateArguments fateArguments)
    {
        if (fateArguments.fate.SubFates.Count == 0)
        {
            GetMemberFate(crewData.MyName).SetGuessedFate(fateArguments.fate);

            crewMateFate_TMP.text = $"{crewData.MyName} {fateArguments.fate.FullDisplay}";
            selectFate_TMP.text = $"{fateArguments.fate.FullDisplay}";

            for (int i = 0; i < membersFateData.Count; i++)
            {
                if (membersFateData[i].correctFate == membersFateData[i].GuessedFate)
                    this.Log($"Correctly guessed the fate of {membersFateData[i].myName}");

                SelectFateEventHandler?.Invoke(this, new());
            }
        }
        else
            DisplayFates(0, fateArguments.fate.SubFates);
    }

    void HandleTogglePopup(object sender, TogglePopupsArgs e)
    {
        switch (e.popup)
        {
            case PopupsManager.Popup.SelectAttacker:
            case PopupsManager.Popup.Fate:
                break;

            case PopupsManager.Popup.SelectFate:
                int pageToOpen = rememberPageNumber ? currentPage : 0;
                pageToOpen = pageToOpen == -1 ? 0 : pageToOpen;

                DisplayFates(pageToOpen, fates);                    
                break;
        }
    }

    void HandleClickTab(object sender, ClickTabEventArgs e)
    {
        crewData = e.crewData;

        selectFate_TMP.text = $"{GetGuessedFate(e.crewData.MyName).FullDisplay}";
    }

    void DisplayFates(int pageNumber, List<FateData> fates)
    {
        if (fates == null)
            return;

        if (pageNumber < 0 || pageNumber * fateSelectButtons.Count > fates.Count)
            return;

        currentPage = pageNumber;

        for (int i = 0; i < fateSelectButtons.Count; i++)
        {
            FateSelectButton button = fateSelectButtons[i];

            int fateToSelect = i + (pageNumber * fateSelectButtons.Count);
            if (fateToSelect < fates.Count)
            {
                var fate = fates[fateToSelect];
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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FateManager : MonoBehaviour, ISelectFate
{
    [Header("Settings")]
    [SerializeField] bool rememberPageNumber;

    [Header("Buttons")]
    [SerializeField] Transform fateButtonsParent;

    [Header("All Fates")]
    [SerializeField] List<FateData> fates;

    [Header("Correct Fates")]
    [SerializeField] FateData LynnsFate;
    [SerializeField] FateData AlvarosFate;
    [SerializeField] FateData LizsFate;
    [SerializeField] FateData MelsFate;
    [SerializeField] FateData BlakesFate;

    Dictionary<FateData, FateData> correctFateToGuessedFate;
    Dictionary<FateData, CrewData.Name> correctFateToName;
    Dictionary<CrewData.Name, FateData> nameToGuessedFate;
    readonly List<FateSelectButton> fateSelectButtons = new();
    int currentPage = -1;

    private void OnEnable()
    {
        PopupsManager.TogglePopupEventHandler += HandleTogglePopup;
    }

    private void OnDisable()
    {
        PopupsManager.TogglePopupEventHandler -= HandleTogglePopup;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FateSelectButton.selectFateHandler = this;
        correctFateToGuessedFate = new Dictionary<FateData, FateData>()
        {
            { LynnsFate,    null },
            { AlvarosFate,  null },
            { BlakesFate,   null },
            { MelsFate,     null },
            { LizsFate,     null },
        };

        correctFateToName = new Dictionary<FateData, CrewData.Name>()
        {
            { LynnsFate,    CrewData.Name.Socha  },
            { AlvarosFate,  CrewData.Name.Alvaro },
            { BlakesFate,   CrewData.Name.Blake  },
            { MelsFate,     CrewData.Name.Mel    },
            { LizsFate,     CrewData.Name.Liz    },
        };

        nameToGuessedFate = new Dictionary<CrewData.Name, FateData>()
        {
            { CrewData.Name.Socha,  null },
            { CrewData.Name.Alvaro, null },
            { CrewData.Name.Blake,  null },
            { CrewData.Name.Mel,    null },
            { CrewData.Name.Liz,    null },
        };

        for (int i = 0; i < fateButtonsParent.childCount; i++)
            fateSelectButtons.Add(fateButtonsParent.GetChild(i).GetComponent<FateSelectButton>());
    }

    // Update is called once per frame
    void Update()
    {
        if (PopupsManager.IsPopupOpen(PopupsManager.Popup.SelectFate))
        {
            if (Input.GetKeyDown(KeyCode.D))
                DisplayPage(currentPage + 1);
            else if (Input.GetKeyDown(KeyCode.A))
                DisplayPage(currentPage - 1);
        }
    }

    void DisplayPage(int pageNumber)
    {
        if (pageNumber < 0 || pageNumber * fateSelectButtons.Count > fates.Count)
            return;
        
        currentPage = pageNumber;

        for (int i = 0; i < fateSelectButtons.Count; i++)
        {
            FateSelectButton button = fateSelectButtons[i];

            int fateToSelect = i + (pageNumber * fateSelectButtons.Count);
            if (fateToSelect < fates.Count)
            {
                FateData fate = fates[fateToSelect];
                button.Initialize(fate);
            }
            else
                button.Initialize(null);
        }
    }

    public void HandleSelectFate(FateArguments fateArguments)
    {
        nameToGuessedFate[NotesManager.CrewData.MyName] = fateArguments.fate;

        for (int i = 0; i < correctFateToGuessedFate.Count; i++)
        {
            List<FateData> correctFates = correctFateToGuessedFate.Keys.ToList();

            if (correctFateToGuessedFate[correctFates[i]] == correctFates[i])
                this.Log($"Correctly guessed the fate of {correctFateToName[correctFates[i]]}");
        }
    }

    void HandleTogglePopup(object sender, TogglePopupsEventArgs e)
    {
        switch (e.popup)
        {
            case PopupsManager.Popup.Fate:
                break;
            case PopupsManager.Popup.SelectFate:

                int pageToOpen = rememberPageNumber ? currentPage : 0;
                pageToOpen = pageToOpen == -1 ? 0 : pageToOpen;

                DisplayPage(pageToOpen);
                break;
            case PopupsManager.Popup.SelectAttacker:
                break;
        }
    }
}
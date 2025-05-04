using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static PopupsManager;
using UnityEngine.Assertions;

public class PopupsManager : MonoBehaviour, IClickPopup
{
    public enum Popup { None, Fate, Time, Location, Notes, SelectFate, SelectAttacker }

    [Header("Popups")]
    [SerializeField] Window FatePopup;
    [SerializeField] Window LocationPopup;
    [SerializeField] Window TimePopup;
    [SerializeField] Window NotesPopup;
    [SerializeField] Window selectFatePopup;
    [SerializeField] GameObject popupParent;
    Dictionary<Popup, Window> PopupsToWindow;

    [Header("Tween")]
    [Range(0f, 1f)]
    [SerializeField] float backgroundAlpha = 126;
    [SerializeField] Image popupBackground;
    [SerializeField] float toggleDuration;
    [SerializeField] Ease backgroundFadeEase;

    [Header("Fate Popup")]
    [SerializeField] TMPro.TextMeshProUGUI name_TMP;
    [SerializeField] Image photo;

    List<Window> popups = new();
    static readonly List<Window> openPopups = new();

    public static bool IsAPopupOpen => openPopups.Count > 0;

    public static EventHandler<TogglePopupsArgs> TogglePopupEventHandler;

    void OnEnable()
    {
        GameStateManager.ChangePlayStateEventHandler += HandleChangePlayState;
        FateManager.SelectFateEventHandler += HandleSelectFate;
    }

    void OnDisable()
    {
        GameStateManager.ChangePlayStateEventHandler -= HandleChangePlayState;
        FateManager.SelectFateEventHandler -= HandleSelectFate;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        popupParent.SetActive(false);

        for (int i = 0; i < popupParent.transform.childCount; i++)
            popupParent.transform.GetChild(i).gameObject.SetActive(false);

        popupBackground.gameObject.SetActive(false);

        PopupButton.clickPopupHandler = this;

        PopupsToWindow = new Dictionary<Popup, Window>()
        {
            { Popup.Fate,       FatePopup },
            { Popup.Location,   LocationPopup },
            { Popup.Time,       TimePopup },
            { Popup.Notes,      NotesPopup },
            { Popup.SelectFate, selectFatePopup },
        };

        popups = PopupsToWindow.Values.ToList();

        foreach (var popup in popups)
            TogglePopup(popup, false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameStateManager.MyPlayState)
        {
            case GameStateManager.PlayState.None:
                break;
            case GameStateManager.PlayState.Station:
                break;
            case GameStateManager.PlayState.Notes:
                if (Input.GetMouseButtonDown(1) && IsAPopupOpen)
                    TogglePopup(openPopups[^1], false);
                break;
        }

        Assert.IsNotNull(this);
    }

    void HandleSelectFate(object sender, EventArgs e)
    {
        TogglePopup(selectFatePopup, false);
    }

    void HandleChangePlayState(object sender, ChangePlayStateEventArgs e)
    {
        switch (e.myPlayState)
        {
            case GameStateManager.PlayState.None:
                FadeBackground(false);
            break;

            case GameStateManager.PlayState.Station:
                FadeBackground(false);
            break;

            case GameStateManager.PlayState.Notes:
                FadeBackground(true);
            break;
        }
    }

    public void OnClickPopupButton(PopupsManager.Popup myPopup, bool isOpening)
        => TogglePopup(PopupsToWindow[myPopup], isOpening);

    void TogglePopup(Window popupWindow, bool isOpening)
    {
        Action onComplete = null;

        if (isOpening)
        {
            InitializePopups();

            openPopups.Add(popupWindow);
            popupParent.SetActive(true);
            popupBackground.gameObject.SetActive(true);

            bool isOnlyPopupOpen = openPopups.Count == 1;

            if (isOnlyPopupOpen)
                FadeBackground(true);
        }
        else
        {
            openPopups.Remove(popupWindow);
            onComplete = OnCompleteClose;

            bool areAllPopupsClosed = openPopups.Count == 0;
            if (areAllPopupsClosed)
                FadeBackground(false);
        }
  
        bool isAPopupOpen = openPopups.Count != 0;
        popupWindow.SetWindow(isOpening, onComplete, toggleDuration);

        if (this == null)
        {
            Debug.LogWarning("I don't understand how Popups Manager is null at all");
            return;
        }
        Assert.IsNotNull(this, "Popups Manager should not be null");
        Assert.IsTrue(PopupsToWindow.Count > 0, "Popups to Window should contain at least 1 value");

        Popup myPopup = PopupsToWindow.FirstOrDefault(x => x.Value == popupWindow).Key;

        TogglePopupEventHandler?.Invoke(this, new(myPopup, isOpening, openPopups.Count));
    }


    void FadeBackground(bool fadeIn)
    {
        var originalColor = popupBackground.color;
        originalColor.a = backgroundAlpha;

        var transparentColor = popupBackground.color;
        transparentColor.a = 0;

        if (fadeIn)
        {
            popupBackground.color = transparentColor;
            popupBackground.DOColor(originalColor, toggleDuration).SetEase(backgroundFadeEase);
        }
        else
            popupBackground.DOColor(transparentColor, toggleDuration).SetEase(backgroundFadeEase);
    }

    void OnCompleteClose()
    {
        if (openPopups.Count == 0)
        {
            popupParent.SetActive(false);
            popupBackground.gameObject.SetActive(false);
        }
    }

    void InitializePopups()
    {
        var crewData = NotesManager.CrewData;
        
        name_TMP.text = $"{crewData.MyName}";
        photo.sprite = crewData.Picture;
    }
}

public class TogglePopupsArgs : EventArgs
{
    public readonly PopupsManager.Popup popup;
    public readonly bool isOpening;
    public readonly int countAfterToggled;

    public TogglePopupsArgs(Popup popup, bool isOpening, int countAfterToggled)
    {
        this.popup = popup;
        this.isOpening = isOpening;
        this.countAfterToggled = countAfterToggled;
    }
}
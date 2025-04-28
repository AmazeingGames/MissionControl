using DG.Tweening;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static PopupsManager;

public class PopupsManager : MonoBehaviour, IClickPopup, IOpenSubfate
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
    readonly List<Window> openPopups = new();
    static PopupsManager instance;

    public static EventHandler<TogglePopupsEventArgs> TogglePopupEventHandler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        popupParent.SetActive(false);
        popupBackground.gameObject.SetActive(false);

        PopupButton.ClickPopupHandler = this;

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
                break;
            case GameStateManager.PlayState.Popups:
                if (Input.GetMouseButtonDown(1))
                    TogglePopup(openPopups[^1], false);
                break;
        }
        
    }

    void HandleToggleNotes(object sender, ToggleNotesEventArgs e)
    {
        FadeBackground(e.isOpening);
    }

    public void HandleOpenFateSelect(FateArguments fateArguments)
        => TogglePopup(selectFatePopup, true);

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
        popupWindow.ToggleWindow(isOpening, onComplete, toggleDuration);

        Popup myKey = PopupsToWindow.FirstOrDefault(x => x.Value == popupWindow).Key;
        OnTogglePopup(myKey, isOpening, openPopups.Count);
    }

    void OnTogglePopup(Popup myPopup, bool isOpening, int newCount)
        => TogglePopupEventHandler?.Invoke(this, new(myPopup, isOpening, newCount));

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
        
        // Fate
        name_TMP.text = $"{crewData.MyName}";
        photo.sprite = crewData.Picture;
    }

    public static bool IsPopupOpen(Popup popup)
        => instance.PopupsToWindow[popup].gameObject.activeSelf;
}

public class TogglePopupsEventArgs : EventArgs
{
    public readonly PopupsManager.Popup popup;
    public readonly bool isOpening;
    public readonly int countAfterToggled;

    public TogglePopupsEventArgs(Popup popup, bool isOpening, int countAfterToggled)
    {
        this.popup = popup;
        this.isOpening = isOpening;
        this.countAfterToggled = countAfterToggled;
    }
}
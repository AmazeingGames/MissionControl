using DG.Tweening;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PopupsManager;

public class PopupsManager : MonoBehaviour, IClickPopup
{
    public enum Popup { None, Fate, Time, Location, Notes }

    [Header("Popups")]
    [SerializeField] Window FatePopup;
    [SerializeField] Window LocationPopup;
    [SerializeField] Window TimePopup;
    [SerializeField] Window NotesPopup;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        popupParent.SetActive(false);
        popupBackground.gameObject.SetActive(false);

        PopupButton.ClickPopupHandler = this;

        PopupsToWindow = new Dictionary<Popup, Window>()
        {
            { Popup.Fate,     FatePopup },
            { Popup.Location, LocationPopup },
            { Popup.Time,     TimePopup },
            { Popup.Notes,    NotesPopup },
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickPopup(PopupsManager.Popup myPopup, bool isOpening)
    {
        Action onComplete = null;
        
        var originalColor = popupBackground.color;
        originalColor.a = backgroundAlpha;

        var transparentColor = popupBackground.color;
        transparentColor.a = 0;

        if (isOpening)
        {
            InitializePopups();

            popupParent.SetActive(true);
            popupBackground.gameObject.SetActive(true);

            popupBackground.color = transparentColor;
            popupBackground.DOColor(originalColor, toggleDuration).SetEase(backgroundFadeEase);
        }
        else
        {
            popupBackground.DOColor(transparentColor, toggleDuration).SetEase(backgroundFadeEase);
            onComplete = OnCompleteClose;
        }

        PopupsToWindow[myPopup].ToggleWindow(isOpening, onComplete, toggleDuration);
    }

    void OnCompleteClose()
    {
        popupParent.SetActive(false);
        popupBackground.gameObject.SetActive(false);
    }

    void InitializePopups()
    {
        var crewData = NotesManager.CrewData;
        
        // Fate
        name_TMP.text = crewData.Name;
        photo.sprite = crewData.Picture;
    }
}
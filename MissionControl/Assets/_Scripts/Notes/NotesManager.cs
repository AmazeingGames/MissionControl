using DG.Tweening;
using JetBrains.Annotations;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotesManager : MonoBehaviour
{
    [Header("Notebook Toggle Tween")]
    [SerializeField] float moveInDuration;
    [SerializeField] float moveOutDuration;
    [SerializeField] float inPosition;
    [SerializeField] float outPosition;
    [SerializeField] Ease moveEase;

    [Header("Text Tween")]
    [SerializeField] float titleEaseDuration;
    [SerializeField] float notesEaseDuration;
    [SerializeField] string textScrambleChars = null;
    [SerializeField] ScrambleMode textScrambleMode;
    [SerializeField] Ease textEase;

    [Header("Page Turn Tween")]
    [SerializeField] float defaultPageTurnSpeed;
    [SerializeField] float fastPageTurnSpeed;

    [Header("Button Appear")]
    [SerializeField] Ease buttonAppearEase;
    [SerializeField] float buttonAppearDuration;
    [SerializeField] bool shouldShake;
    [SerializeField] float buttonTextAppearDuration;
    [SerializeField] ScrambleMode buttonTextScrambleMode;
    [SerializeField] Ease buttonTextEase;

    [Header("Button Shake")]
    [SerializeField] float buttonShakeDuration;
    [SerializeField] float buttonShakeStrength;
    [SerializeField] float buttonShakeVibrato;
    [SerializeField] float buttonShakeRandomness;
    [SerializeField] bool buttonShakeFadeOut;
    [SerializeField] ShakeRandomnessMode buttonShakeMode;

    [Header("Button Punch")]
    [SerializeField] Vector3 buttonPunch;
    [SerializeField] float buttonPunchDuration;
    [SerializeField] float buttonPunchElasticity;
    [SerializeField] int buttonPunchVibrato;

    [Header("Canvas")]
    [SerializeField] Canvas notesCanvas;
    [SerializeField] RectTransform notes;

    [Header("Pages")]
    [SerializeField] List<Transform> pages;
    [SerializeField] int fateSelectPage;
    
    [Header("Page Elements")]
    [SerializeField] TMPro.TextMeshProUGUI role_TMP;
    [SerializeField] TMPro.TextMeshProUGUI name_TMP;
    [SerializeField] TMPro.TextMeshProUGUI notes_TMP;
    [SerializeField] List<Button> fateSelectButtons;
    [SerializeField] TMPro.TextMeshProUGUI fate_TMP;
    [SerializeField] TMPro.TextMeshProUGUI logs_TMP;
    [SerializeField] Image crewMatePicture;

    Sequence notebookSequence;
    public static IToggleNotes ToggleNotesHandler;

    public static CrewData CrewData { get; private set; }

    int pageIndex = -1;
    bool isTurningPage = false;
    bool hasEnabledButtons;

    private IEnumerator Start()
    {
        InitializePages();

        yield return null;

        foreach (var button in fateSelectButtons)
        {
            button.GetComponent<PageElement>().enabled = false;
            button.gameObject.SetActive(false);
            button.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        GameStateManager.PerformGameActionEventHandler += HandlePerformGameAction;
        NotesTab.ClickTabEventHandler += HandleClickTab;
    }

    void OnDisable()
    {
        GameStateManager.PerformGameActionEventHandler -= HandlePerformGameAction;
        NotesTab.ClickTabEventHandler -= HandleClickTab;
    }

    bool areNotesOpen;
    bool wasOpenOnPause;

    void HandlePerformGameAction(object sender, PerformGameActionEventArgs e)
    {
        switch (e.myGameAction)
        {
            case GameStateManager.GameAction.None:
                break;

            case GameStateManager.GameAction.EnterMainMenu:
            case GameStateManager.GameAction.StartGame:
                ToggleNotes(false);
                break;

            case GameStateManager.GameAction.PauseGame:
                wasOpenOnPause = areNotesOpen;
                ToggleNotes(false);
                break;

            case GameStateManager.GameAction.ResumeGame:
                ToggleNotes(wasOpenOnPause);
                break;
            case GameStateManager.GameAction.BeatGame:
                break;
        }
    }

    void Update()
    {
        // Maybe I should consider following the state machine pattern more strictly 
        if (GameStateManager.MyGameState != GameStateManager.GameState.Running)
            return;

        switch (GameStateManager.MyPlayState)
        {
            case GameStateManager.PlayState.None:
                break;

            case GameStateManager.PlayState.Notes:
                if (Input.GetButtonDown("Notes"))
                    ToggleNotes(isOpening: false);

                if (!PopupsManager.IsAPopupOpen)
                {
                    if (Input.GetKeyDown(KeyCode.D))
                        TurnNextPage();
                    if (Input.GetKeyDown(KeyCode.A))
                        TurnPreviousPage();
                }
                
                break;

            case GameStateManager.PlayState.Station:
                if (Input.GetButtonDown("Notes"))
                    ToggleNotes(isOpening: true);
                break;
        }
    }

    void InitializePages()
    {
        foreach (var page in pages)
        {
            page.gameObject.SetActive(true);

            for (int i = 0; i < page.childCount; i++)
            {
                page.GetChild(i).gameObject.SetActive(true);
            }

            page.transform.rotation = Quaternion.identity;
            page.SetAsFirstSibling();
        }
    }

    void ToggleNotes(bool isOpening)
    {
        areNotesOpen = isOpening;
        ToggleNotesHandler?.HandleToggleNotes(new(isOpening));

        notebookSequence?.Kill();
        notebookSequence = DOTween.Sequence();

        if (isOpening)
        {
            notesCanvas.gameObject.SetActive(isOpening);
            notebookSequence.Append(notes.DOLocalMoveY(inPosition, moveInDuration).SetEase(moveEase));
        }
        else
        { 
            notebookSequence.Append(notes.DOLocalMoveY(outPosition, moveOutDuration).SetEase(moveEase));
            notebookSequence.OnComplete(() => notesCanvas.gameObject.SetActive(isOpening));
        }
    }

    public void TurnNextPage(bool useDefaultSpeed = true)
    {
        StartCoroutine(TurnPageCoroutine(true, 180, useDefaultSpeed));
    }

    public void TurnPreviousPage(bool useDefaultSpeed = true)
    {
        StartCoroutine(TurnPageCoroutine(false, 0, useDefaultSpeed));
    }

    IEnumerator TurnPageCoroutine(bool isTurningForward, float angle, bool useDefaultSpeed)
    {
        if (isTurningPage)
            yield break;

        if (isTurningForward && pageIndex >= pages.Count - 1)
            yield break;

        else if (!isTurningForward && pageIndex < 0)
            yield break;

        if (isTurningForward)
            pageIndex++;

        pages[pageIndex].SetAsLastSibling();

        float time = 0;

        while (true)
        {
            isTurningPage = true;

            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);

            float turnSpeed = useDefaultSpeed ? defaultPageTurnSpeed : fastPageTurnSpeed;
            time += Time.deltaTime * turnSpeed;

            pages[pageIndex].rotation = Quaternion.Slerp(pages[pageIndex].rotation, targetRotation, time);
            float differenceBetweenAngles = Quaternion.Angle(pages[pageIndex].rotation, targetRotation);

            this.Log($"Time: {time}");

            if (differenceBetweenAngles < .1f)
            {
                if (!isTurningForward)
                    pageIndex--;
                
                isTurningPage = false;
                break;
            }
            yield return null;
        }
    }


    public void HandleClickTab(object sender, ClickTabEventArgs e)
    {
        CrewData = e.crewData;

        role_TMP.DOText($"{e.crewData.MyRole}", titleEaseDuration, true, textScrambleMode, textScrambleChars).SetEase(textEase);
        name_TMP.DOText($"- {e.crewData.MyName} -", titleEaseDuration, true, textScrambleMode, textScrambleChars).SetEase(textEase);

        string targetText = "";
        StartCoroutine(TurnToPageCoroutine(fateSelectPage));

        fate_TMP.DOText($"{e.crewData.MyName} {FateManager.GetGuessedFate(e.crewData.MyName).FullDisplay}", buttonTextAppearDuration, true, buttonTextScrambleMode, null).SetEase(buttonTextEase);
        logs_TMP.DOText($"Logs Logs Logs", buttonTextAppearDuration, true, buttonTextScrambleMode, null).SetEase(buttonTextEase);
        crewMatePicture.sprite = e.crewData.Picture;

        foreach (var button in fateSelectButtons)
        {
            if (hasEnabledButtons)
                break;

            var buttonText = button.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            var buttonImage = button.GetComponent<Image>();

            var originalbuttonColor = buttonImage.color;
            var transparentButtonColor = buttonImage.color;
            transparentButtonColor.a = 0;
            buttonImage.color = transparentButtonColor;

            var originalTextColor = buttonText.color;
            var transparentTextColor = buttonText.color;
            transparentTextColor.a = 0;
            buttonText.color = transparentTextColor;


            button.GetComponent<PageElement>().enabled = true;
            button.gameObject.SetActive(true);
            buttonText.gameObject.SetActive(true);

            // Note: I could instead use DOFade
            buttonText.DOColor(originalTextColor, buttonAppearDuration).SetEase(buttonAppearEase);
            buttonImage.DOColor(originalbuttonColor, buttonAppearDuration).SetEase(buttonAppearEase);
        }
        hasEnabledButtons = true;

        foreach (var button in fateSelectButtons)
        {
            button.transform.DOKill();

            if (shouldShake)
                button.transform.DOShakeRotation(buttonShakeDuration, fadeOut: buttonShakeFadeOut, strength: new Vector3(0, 0, buttonShakeStrength), randomnessMode: buttonShakeMode);
            else
                button.transform.DOPunchRotation(buttonPunch, buttonPunchDuration, buttonPunchVibrato, buttonPunchElasticity).OnComplete(() => button.transform.DOLocalRotate(new(0, 0, 0), .5f));
        }
    }

    IEnumerator TurnToPageCoroutine(int pageIndex)
    {
        while (pageIndex != this.pageIndex)
        {
            if (pageIndex > this.pageIndex)
                TurnNextPage(false);
            else if (pageIndex < this.pageIndex)
                TurnPreviousPage(false);
            else
                yield break;

            yield return null;
        }
    }
}

public interface IToggleNotes
{
    public void HandleToggleNotes(ToggleNotesArgs e);
}

public class ToggleNotesArgs
{
    public readonly bool isOpening;
    public ToggleNotesArgs(bool isOpening)
    { 
        this.isOpening = isOpening; 
    }
}

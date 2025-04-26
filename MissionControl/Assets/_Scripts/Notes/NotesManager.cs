using DG.Tweening;
using JetBrains.Annotations;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour, IClickTab
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
    [SerializeField] float pageTurnSpeed;
    [SerializeField] RotateMode pageTurnRotateMode;

    [Header("Canvas")]
    [SerializeField] Canvas notesCanvas;
    [SerializeField] RectTransform notes;

    [Header("Pages")]
    [SerializeField] List<Transform> pages;

    [Header("Text Fields")]
    [SerializeField] TMPro.TextMeshProUGUI role_TMP;
    [SerializeField] TMPro.TextMeshProUGUI name_TMP;
    [SerializeField] TMPro.TextMeshProUGUI notes_TMP;

    Sequence notebookSequence;
    public static EventHandler<OpenNotesEventArgs> OpenNotesEventHandler;

    int pageIndex = -1;
    bool isTurningPage = false;

    List<Action> RotateSequence = new();

    private void Start()
    {
        NotesTab.clickTabHandler = this as IClickTab;
    }

    void OnEnable()
    {
        GameStateManager.PerformGameActionEventHandler += HandlePerformGameAction;
    }

    void OnDisable()
    {
        GameStateManager.PerformGameActionEventHandler -= HandlePerformGameAction;
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
            case GameStateManager.GameAction.LoseGame:
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
            break;

            case GameStateManager.PlayState.Station:
                if (Input.GetButtonDown("Notes"))
                    ToggleNotes(isOpening: true);
            break;
        }

        if (areNotesOpen)
        {
            if (Input.GetKeyDown(KeyCode.D))
                TurnNextPage();
            if (Input.GetKeyDown(KeyCode.A))
                TurnPreviousPage();
        }
    }

    void ToggleNotes(bool isOpening)
    {
        areNotesOpen = isOpening;
        OpenNotesEventHandler?.Invoke(this, new(isOpening));

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

    public void TurnNextPage()
    {
        StartCoroutine(TurnPageCoroutine(true, 180));
    }

    public void TurnPreviousPage()
    {
        StartCoroutine(TurnPageCoroutine(false, 0));
    }

    IEnumerator TurnPageCoroutine(bool isTurningForward, float angle)
    {
        if (isTurningPage)
            yield break;

        if (isTurningForward && pageIndex >= pages.Count - 1)
            yield break;

        else if (!isTurningForward && pageIndex <= 0)
            yield break;

        if (isTurningForward)
            pageIndex++;


        pages[pageIndex].SetAsLastSibling();

        float time = 0;

        while (true)
        {
            isTurningPage = true;

            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            time += Time.deltaTime * pageTurnSpeed;

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


    public void OnClickTab(CrewData crewData)
    {
        role_TMP.DOText($"{crewData.MyRole}", titleEaseDuration, true, textScrambleMode, textScrambleChars).SetEase(textEase);
        name_TMP.DOText($"- {crewData.Name} -", titleEaseDuration, true, textScrambleMode, textScrambleChars).SetEase(textEase);

        string targetText = "";
        foreach (string note in crewData.LogNotes)
            targetText += $"> {note}\n";

        notes_TMP.DOText(targetText, notesEaseDuration, true, textScrambleMode, textScrambleChars).SetEase(textEase);
        notes_TMP.font = crewData.NotesFont;
    }
}

public class OpenNotesEventArgs : EventArgs
{
    public readonly bool isOpening;

    public OpenNotesEventArgs(bool isOpening)
    {
        this.isOpening = isOpening;
    }
}

using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// One of the few class responsible for conveying important changes in game state directly to the game manager
public class UIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [field: Header("Button")]
    [field: SerializeField] public float RegularScale { get; private set; } = 1.0f;
    [field: SerializeField] public float HoverScale { get; private set; } = 1.1f;
    [field: Range(0, 1)][field: SerializeField] public float RegularOpacity { get; private set; } = .66f;
    [field: Range(0, 1)][field: SerializeField] public float HoverOpacity { get; private set; } = 1;

    [field: Header("Button Lerp")]
    [field: SerializeField] public float ButtonLerpSpeed { get; private set; } = 8;
    [field: SerializeField] public float UnderlineLerpSpeed { get; private set; } = 8;
    [field: SerializeField] public AnimationCurve ButtonLerpCurve { get; private set; }
    [field: SerializeField] public AnimationCurve UnderlineLerpCurve { get; private set; }


    [Header("Button Type")]
    [SerializeField] ButtonType myButtonType;

    // I have no clue what I was trying to say when I wrote this comment:
        // Turn this into a class value, which inherits from the same type
        // Change the class based on the button type, and then serialize the class values
        // The script would have to run in the editor for this to work properly
    [Header("UI Button Type")]
    [SerializeField] UIManager.MenuType myMenuToOpen;

    [Header("Game State Button Type")]
    [SerializeField] GameStateManager.GameAction myGameActionToPerform;

    [Header("Components")]
    [SerializeField] TextMeshProUGUI text_TMP;
    [SerializeField] Image underline;

    public enum ButtonType { None, UI, Gamestate, Gameplay }
    public enum InteractionTypes { Enter, Click, Up, Exit }

    // We're using Interfaces instead of C# events to ensure only one class can subscribe to each event
    public static IChangeMenuHandler ChangeMenuHandler;
    public static IGameStateActionHandler GameStateActionHandler;
    public static IGameplayActionHandler GameplayActionHandler;

    Coroutine lerpButtonCoroutine = null;
    Coroutine lerpUnderlineCoroutine = null;

    void Start()
    {
        text_TMP.alpha = RegularOpacity;
        text_TMP.gameObject.SetActive(true);

        var regularScale = RegularScale;
        var rectTransform = transform as RectTransform;

        text_TMP.transform.localScale = new Vector3(regularScale, regularScale, text_TMP.transform.localScale.z);
        // underline.rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, underline.rectTransform.sizeDelta.y);
        // underline.fillAmount = 0;

        StartCoroutine(SetUnderlineLength());
    }

    void OnDisable()
    {
        text_TMP.transform.localScale = new(RegularScale, RegularScale);
        text_TMP.alpha = RegularOpacity;
        // underline.fillAmount = 0;
    }

    IEnumerator SetUnderlineLength()
    {
        // underline.rectTransform.sizeDelta = new Vector2(0, underline.rectTransform.sizeDelta.y);

        while (true)
        {
            var rect = transform as RectTransform;
            // underline.rectTransform.sizeDelta = new Vector2(rect.sizeDelta.x, underline.rectTransform.sizeDelta.y);
            yield return new WaitForSeconds(.1f);
        }
    }

    /// <summary> Moves the last held paper to the correct position over time. </summary>
    IEnumerator LerpButton(bool isSelected)
    {
        float time = 0;

        float startingScale = text_TMP.transform.localScale.x;
        float targetScale = isSelected ? HoverScale : RegularScale;

        float startingOpacity = text_TMP.alpha;
        float targetOpacity = isSelected ? HoverOpacity : RegularOpacity;

        while (time < 1)
        {
            var lerpCurve = ButtonLerpCurve;

            float newScale = Mathf.Lerp(startingScale, targetScale, lerpCurve.Evaluate(time));
            text_TMP.transform.localScale = new Vector3(newScale, newScale, text_TMP.transform.localScale.z);

            float newOpacity = Mathf.Lerp(startingOpacity, targetOpacity, lerpCurve.Evaluate(time));
            text_TMP.alpha = newOpacity;

            time += Time.deltaTime * ButtonLerpSpeed;
            yield return null;
        }
    }

    IEnumerator LerpUnderline(bool isSelected)
    {
        float time = 0;

        // float startingFill = underline.fillAmount;
        float targetFill = isSelected ? 1 : 0;

        while (time < 1)
        {
            var lerpCurve = UnderlineLerpCurve;

            // float newFillAmount = Mathf.Lerp(startingFill, targetFill, lerpCurve.Evaluate(time));
            // underline.fillAmount = newFillAmount;

            time += Time.deltaTime * UnderlineLerpSpeed;
            yield return null;
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        OnUIInteract(pointerEventData, InteractionTypes.Click);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        OnUIInteract(pointerEventData, InteractionTypes.Enter);

        HighlightButton(true);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        OnUIInteract(pointerEventData, InteractionTypes.Exit);

        HighlightButton(false);
    }

    void HighlightButton(bool isSelected)
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (lerpButtonCoroutine != null)
            StopCoroutine(lerpButtonCoroutine);

        lerpButtonCoroutine = StartCoroutine(LerpButton(isSelected));

        if (lerpUnderlineCoroutine != null)
            StopCoroutine(lerpUnderlineCoroutine);

        lerpUnderlineCoroutine = StartCoroutine(LerpUnderline(isSelected));
    }

    public void OnPointerUp(PointerEventData pointerEventData)
        => OnUIInteract(pointerEventData, InteractionTypes.Up);

    public virtual void OnUIInteract(PointerEventData pointerEventData, InteractionTypes myInteractionType)
    {
        if (myInteractionType != InteractionTypes.Click)
            return;

        switch (myButtonType)
        {
            case ButtonType.None:
                break;

            case ButtonType.UI:
                ChangeMenuHandler?.OnClick(myMenuToOpen);
                break;

            case ButtonType.Gamestate:
                GameStateActionHandler?.OnClick(myGameActionToPerform);
                break;
        }
    }

    private void OnValidate()
    {
        switch (myButtonType)
        {
            case ButtonType.None:
                break;

            case ButtonType.UI:
                switch (myMenuToOpen)
                {
                    case UIManager.MenuType.None:
                        throw new DataMisalignedException("A menu type of none will cause nothing to happen.");
                    
                    case UIManager.MenuType.Pause:
                        throw new DataMisalignedException("Pausing the game is a game action, not a UI action.");
                    
                    case UIManager.MenuType.Empty:
                        throw new DataMisalignedException("Closing all menus should be done by updating the game state.");

                    case UIManager.MenuType.Settings:
                    case UIManager.MenuType.Previous:
                    case UIManager.MenuType.MainMenu:
                    case UIManager.MenuType.Credits:
                        break;
                }
                break;

            case ButtonType.Gamestate:
                switch (myGameActionToPerform)
                {
                    case GameStateManager.GameAction.None:
                            throw new DataMisalignedException("A game state of none will cause nothing to happen.");

                    case GameStateManager.GameAction.EnterMainMenu:
                    case GameStateManager.GameAction.StartGame:
                    case GameStateManager.GameAction.PauseGame:
                    case GameStateManager.GameAction.ResumeGame:
                    case GameStateManager.GameAction.LoseGame:
                        break;
                }
                break;
        }
    }
}

public interface IChangeMenuHandler
{
    public void OnClick(UIManager.MenuType myMenuType);
}

public interface IGameStateActionHandler
{
    public void OnClick(GameStateManager.GameAction myGameAction);
}

public interface IGameplayActionHandler
{
    public void OnClick(GameStateManager.PlayState myPlayState);
}
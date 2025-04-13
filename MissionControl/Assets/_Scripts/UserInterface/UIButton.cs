using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// One of the few class responsible for conveying important changes in game state directly to the game manager
public class UIButton : MonoBehaviour
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

    public enum ButtonType { None, UI, GameAction, DayAction, Setting }
    public enum UIInteractionTypes { Enter, Click, Up, Exit }

    public static EventHandler<UIInteractEventArgs> UIInteractEventHandler;

    Coroutine lerpButtonCoroutine = null;
    Coroutine lerpUnderlineCoroutine = null;

    void Start()
    {
        text_TMP.alpha = RegularOpacity;
        text_TMP.gameObject.SetActive(true);

        var regularScale = RegularScale;
        var rectTransform = transform as RectTransform;

        text_TMP.transform.localScale = new Vector3(regularScale, regularScale, text_TMP.transform.localScale.z);
        underline.rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, underline.rectTransform.sizeDelta.y);
        underline.fillAmount = 0;

        StartCoroutine(SetUnderlineLength());
    }

    void OnEnable()
    {
        // if (!= null)
            // StartCoroutine(LerpButton(false));
    }

    void OnDisable()
    {
        // if (== null)
            // return;

        text_TMP.transform.localScale = new(RegularScale, RegularScale);
        text_TMP.alpha = RegularOpacity;
        underline.fillAmount = 0;
    }

    IEnumerator SetUnderlineLength()
    {
        underline.rectTransform.sizeDelta = new Vector2(0, underline.rectTransform.sizeDelta.y);

        while (true)
        {
            var rect = transform as RectTransform;
            underline.rectTransform.sizeDelta = new Vector2(rect.sizeDelta.x, underline.rectTransform.sizeDelta.y);
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

        float startingFill = underline.fillAmount;
        float targetFill = isSelected ? 1 : 0;

        while (time < 1)
        {
            var lerpCurve = UnderlineLerpCurve;

            float newFillAmount = Mathf.Lerp(startingFill, targetFill, lerpCurve.Evaluate(time));
            underline.fillAmount = newFillAmount;

            time += Time.deltaTime * UnderlineLerpSpeed;
            yield return null;
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        OnUIInteract(pointerEventData, UIInteractionTypes.Click);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        OnUIInteract(pointerEventData, UIInteractionTypes.Enter);

        HighlightButton(true);
    }
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        OnUIInteract(pointerEventData, UIInteractionTypes.Exit);

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
        => OnUIInteract(pointerEventData, UIInteractionTypes.Up);

    public virtual void OnUIInteract(PointerEventData pointerEventData, UIInteractionTypes buttonInteract)
        => UIInteractEventHandler?.Invoke(this, new(this, myButtonType, pointerEventData, buttonInteract));

    public class UIInteractEventArgs : EventArgs
    {
        public readonly ButtonType myButtonType;
        public readonly UIInteractionTypes myInteractionType;
        public readonly PointerEventData pointerEventData;

        public readonly UIManager.MenuType myMenuToOpen = UIManager.MenuType.None;
        public readonly GameStateManager.GameAction myActionToPerform = GameStateManager.GameAction.None;
        public UIInteractEventArgs(UIButton button, ButtonType buttonType, PointerEventData pointerEventData, UIInteractionTypes uiInteractionType)
        {
            this.myButtonType = buttonType;
            this.pointerEventData = pointerEventData;
            this.myInteractionType = uiInteractionType;

            if (uiInteractionType == UIInteractionTypes.Enter || uiInteractionType == UIInteractionTypes.Exit)
                return;

            switch (buttonType)
            {
                case ButtonType.UI:
                    myMenuToOpen = button.myMenuToOpen;

                    switch (myMenuToOpen)
                    {
                        case UIManager.MenuType.None:
                            throw new InvalidOperationException("A menu type of none will cause nothing to happen.");
                        case UIManager.MenuType.Pause:
                            throw new InvalidOperationException("Pausing the game should be done by updating the game state, not through changing UI menus.");
                        case UIManager.MenuType.Empty:
                            throw new InvalidOperationException("Closing all menus should be done by updating the game to the proper game state, not through changing UI menus.");
                    }
                    break;

                case ButtonType.GameAction:
                    myActionToPerform = button.myGameActionToPerform;

                    if (myActionToPerform == GameStateManager.GameAction.None)
                        throw new InvalidOperationException("A game state of none will cause nothing to happen.");
                break;
            }
        }
    }
}

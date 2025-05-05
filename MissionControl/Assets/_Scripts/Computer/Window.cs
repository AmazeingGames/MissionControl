using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Window : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] WindowType myWindowType;

    [Header("Animation")]
    [SerializeField] float originalSize = 1f;
    [SerializeField] float duration = .2f;
    [SerializeField] Ease ease = Ease.InSine;
    
    RectTransform rectTransform;

    public enum WindowType { None, LockScreen, HomeScreen, Logs, CameraManual, WingdingsDecoder, CameraChart, PersonnelLogs, FileExplorer, FatePopup, MapPopup, NotesPopup, TimePopup, SelectPopup, WaveLink, LogsAccess }

    RectTransform RectTransform
    {
        get
        {
            if (rectTransform == null) 
                rectTransform = GetComponent<RectTransform>();
            return rectTransform;
        }
    }

    Sequence sequence;
    bool hasOpened;

    public static EventHandler<SetWindowEventArgs> SetWindowEventHandler;

    private void Start()
    {
        switch (myWindowType)
        {
            case WindowType.None:
            case WindowType.LockScreen:
            case WindowType.HomeScreen:
                throw new DataMisalignedException($"Window type should not be set to {myWindowType} on {gameObject.name}");
        }
    }

    public void SetWindow(bool isOpening, Action onComplete = null, float overrideDuration = -1)
    {
        float duration = overrideDuration == -1 ? this.duration : overrideDuration;

        RectTransform.SetAsLastSibling();

        LogsManager.Log(LogsManager.Instance.WindowsLoggingObject, $"Started {(isOpening ? "open" : "close")}");

        if (isOpening)
            gameObject.SetActive(true);

        sequence?.Kill();
        sequence = DOTween.Sequence();

        if (!hasOpened && !isOpening)
        {
            LogsManager.Log(LogsManager.Instance.WindowsLoggingObject, "Attempting to close window before it should even be open.");
            return;
        }

        if (!hasOpened)
        {
            LogsManager.Log(LogsManager.Instance.WindowsLoggingObject, $"Is {(isOpening ? "opening" : "closing")} for the first time");
            sequence.Append(RectTransform.DOScale(0, 0));
        }

        float targetScale = isOpening ? originalSize : 0f;
        sequence.Append(RectTransform.DOScale(targetScale, duration)).SetEase(ease);

        // Disables the window after the animation sequence has finished
        if (!isOpening)
            sequence.OnComplete(() => { gameObject.SetActive(false); LogsManager.Log(LogsManager.Instance.WindowsLoggingObject, "finished closing"); onComplete?.Invoke(); });
        else
            sequence.OnComplete(() => { LogsManager.Log(LogsManager.Instance.WindowsLoggingObject, "finished opening"); onComplete?.Invoke(); });

        hasOpened = true;

        SetWindowEventHandler?.Invoke(this, new(isOpening, myWindowType));
    }

    public void SetWindow(bool isOpening)
        => SetWindow(isOpening, null, -1);

    /// <summary>
    ///     Closes a window if it was the last window interacted with, sets it as the last window interacted with.
    /// </summary>
    public void ToggleWindow()
    {
        bool isOpening = !gameObject.activeInHierarchy;

        if (isOpening)
            SetWindow(true);
        else
        {
            var siblingIndex = RectTransform.GetSiblingIndex();
            RectTransform.SetAsLastSibling();

            if (siblingIndex == RectTransform.GetSiblingIndex() || IsOnlyWindowActive(RectTransform))
                SetWindow(false);
        }
    }

    bool IsOnlyWindowActive(RectTransform rectTransform)
    {
        for (int i = 0; i < rectTransform.parent.childCount; i++)
        {
            RectTransform window = rectTransform.parent.GetChild(i) as RectTransform;

            if (window == rectTransform)
                continue;

            if (window.gameObject.activeInHierarchy)
                return false;
        }
        return true;
    }
}

public class SetWindowEventArgs : EventArgs
{
    public readonly bool isOpening;
    public readonly Window.WindowType myWindowType;

    public SetWindowEventArgs(bool isOpening, Window.WindowType myWindowType)
    {
        this.isOpening = isOpening;
        this.myWindowType = myWindowType;
    }
}

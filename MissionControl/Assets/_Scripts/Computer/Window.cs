using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Window : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] float originalSize = 1f;
    [SerializeField] float duration = .2f;
    [SerializeField] Ease ease = Ease.InSine;
    RectTransform rectTransform;

    Sequence sequence;
    bool hasOpened;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleWindow(bool isOpening, Action onComplete = null, float overrideDuration = -1)
    {
        float duration = overrideDuration == -1 ? this.duration : overrideDuration;

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        rectTransform.SetAsLastSibling();

        LogsManager.Log(LogsManager.Instance.WindowsLogger, $"Started {(isOpening ? "open" : "close")}");

        if (isOpening)
            gameObject.SetActive(true);

        sequence?.Kill();
        sequence = DOTween.Sequence();

        if (!hasOpened && !isOpening)
        {
            Debug.LogWarning("Attempting to close window before it should even be open.");
            return;
        }

        if (!hasOpened)
        {
            LogsManager.Log(LogsManager.Instance.WindowsLogger, $"Is {(isOpening ? "opening" : "closing")} for the first time");
            sequence.Append(rectTransform.DOScale(0, 0));
        }

        float targetScale = isOpening ? originalSize : 0f;
        sequence.Append(rectTransform.DOScale(targetScale, duration)).SetEase(ease);

        if (!isOpening)
            sequence.OnComplete(() => { gameObject.SetActive(false); LogsManager.Log(LogsManager.Instance.WindowsLogger, "finished closing"); onComplete?.Invoke(); });
        else
            sequence.OnComplete(() => { LogsManager.Log(LogsManager.Instance.WindowsLogger, "finished opening"); onComplete?.Invoke(); });

        hasOpened = true;
    }

    public void ToggleWindow(bool isOpening)
    {
        ToggleWindow(isOpening, null, -1);
    }
}

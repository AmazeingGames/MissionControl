using DG.Tweening;
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

    public void ToggleWindow(bool isOpening)
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        rectTransform.SetAsLastSibling();

        string toggleText = isOpening ? "open" : "close";
        string togglingText = isOpening ? "opening" : "closing";

        LogsManager.Log(LogsManager.Instance.WindowsLogger, $"Started {toggleText}");

        if (isOpening)
        {
            gameObject.SetActive(true);
        }
        
        sequence?.Kill();
        sequence = DOTween.Sequence();

        bool isClosingBeforeBeingOpened = !hasOpened && !isOpening;
        if (isClosingBeforeBeingOpened)
        {
            Debug.LogWarning("Closing window before it should even be open.");
            return;
        }

        if (!hasOpened)
        {
            LogsManager.Log(LogsManager.Instance.WindowsLogger, $"Is {togglingText} for the first time");
            sequence.Append(rectTransform.DOScale(0, 0));
        }

        float targetScale = isOpening ? originalSize : 0f;
        sequence.Append(rectTransform.DOScale(targetScale, duration)).SetEase(ease);
        
        if (!isOpening)
            sequence.OnComplete(() => { gameObject.SetActive(false); LogsManager.Log(LogsManager.Instance.WindowsLogger, "finished closing"); });
        else 
            sequence.OnComplete(() => { LogsManager.Log(LogsManager.Instance.WindowsLogger, "finished opening"); });

        hasOpened = true;
    }
}

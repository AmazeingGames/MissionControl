using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour, IPointerClickHandler
{
    [field: SerializeField] public IPInformation.Room myRoom { get; private set; }
    [SerializeField] GameObject roomDisplay;

    [Header("Shake Tween")]
    [SerializeField] float shakeDuration = .2f;
    [SerializeField] float shakeAmount = 1;
    [SerializeField] Ease shakeEase = Ease.Linear;

    [Header("Color Tween")]
    [SerializeField] Color wrongColor = Color.red;
    [SerializeField] float turnColorDuration = .25f;
    [SerializeField] float relaxColorDuration = 1f;
    [SerializeField] Ease colorEase = Ease.InOutSine;

    [Header("Components")]
    [SerializeField] public Image lockImage;
    [SerializeField] Image boxImage;

    public bool isUnlocked = false;

    Sequence shakeSequence;
    Sequence lockColorSequence;
    Sequence boxColorSequence;

    Color lockDefaultColor;
    Color boxDefaultColor;

    public static EventHandler<ToggleCameraEventArgs> ToggleCameraEventHandler;

    void Start()
    {

        if (lockImage != null)
        {
            lockDefaultColor = lockImage.color;
            boxDefaultColor = boxImage.color;
            boxImage.gameObject.SetActive(true);
            lockImage.gameObject.SetActive(true);
        }
        
        if (roomDisplay != null)
        {
            roomDisplay.SetActive(false);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (myRoom == IPInformation.Room.None)
        {
            ToggleCameraEventHandler?.Invoke(this, new(myRoom, isOpening: false));
            return;
        }

        if (isUnlocked)
        {
            roomDisplay.SetActive(true);
            ToggleCameraEventHandler?.Invoke(this, new(myRoom, isOpening: true));
        }
        else
        {
            shakeSequence?.Kill();

            shakeSequence = DOTween.Sequence();
            float xPosition = transform.localPosition.x;
            shakeSequence.Append(transform.DOLocalMoveX(xPosition - shakeAmount, shakeDuration / 3)).SetEase(shakeEase);
            shakeSequence.Append(transform.DOLocalMoveX(xPosition + shakeAmount, shakeDuration / 3)).SetEase(shakeEase);
            shakeSequence.Append(transform.DOLocalMoveX(xPosition, shakeDuration / 3)).SetEase(shakeEase);

            lockColorSequence?.Kill();
            lockColorSequence = DOTween.Sequence();
            lockColorSequence.Append(lockImage.DOColor(wrongColor, turnColorDuration).SetEase(colorEase));
            lockColorSequence.Append(lockImage.DOColor(lockDefaultColor, relaxColorDuration).SetEase(colorEase));

            boxColorSequence?.Kill();
            boxColorSequence = DOTween.Sequence();
            boxColorSequence.Append(boxImage.DOColor(wrongColor, turnColorDuration).SetEase(colorEase));
            boxColorSequence.Append(boxImage.DOColor(boxDefaultColor, relaxColorDuration).SetEase(colorEase));
        }
    }
}

public class ToggleCameraEventArgs : EventArgs
{
    public readonly IPInformation.Room myRoom;
    public readonly bool isOpening;

    public ToggleCameraEventArgs(IPInformation.Room myRoom, bool isOpening)
    {
        this.myRoom = myRoom;
        this.isOpening = isOpening;
    }
}
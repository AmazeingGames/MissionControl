using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] IPInformation.Room myRoom;

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
    [SerializeField] Image lockImage;
    [SerializeField] Image boxImage;

    bool isUnlocked = false;

    Sequence shakeSequence;
    Sequence lockColorSequence;
    Sequence boxColorSequence;

    Color lockDefaultColor;
    Color boxDefaultColor;

    public static EventHandler<OpenCameraEventArgs> OpenCameraEventHandler;

    private void OnEnable()
    {
        CameraUnlockButton.UnlockCameraEventHandler += HandleUnlockCamera;
    }

    private void OnDisable()
    {
        CameraUnlockButton.UnlockCameraEventHandler -= HandleUnlockCamera;
    }

    void Start()
    {
        Assert.IsTrue(myRoom != IPInformation.Room.None);

        lockDefaultColor = lockImage.color;
        boxDefaultColor = boxImage.color;
        lockImage.gameObject.SetActive(true);
    }

    void HandleUnlockCamera(object sender, UnlockCameraEventArgs e)
    {
        if (e.ipInformation.myRoom != myRoom)
            return;

        e.ipInformation.HandleCameraUnlock();
        lockImage.gameObject.SetActive(false);
        isUnlocked = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUnlocked)
        {

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

public class OpenCameraEventArgs : EventArgs
{
    public IPInformation.Room myRoomToOpen;


}
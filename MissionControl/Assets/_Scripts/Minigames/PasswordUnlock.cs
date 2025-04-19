using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordUnlock : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] GameObject lockScreen;
    [SerializeField] GameObject unlockScreen;
    [SerializeField] TMP_InputField inputField;

    [Header("Animation")]
    [SerializeField] float duration;
    [SerializeField] float shakeAmount;
    [SerializeField] Ease ease;

    [Header("Password")]
    [SerializeField] List<string> password;


    Sequence shakeSequence;
    public void ReadInput(string input)
    {
        if (password.Contains(input))
        {
            lockScreen.SetActive(false);
            unlockScreen.SetActive(true);
        }
        else if (inputField.text != "")
        {
            inputField.text = "";
            //inputField.Select();
            inputField.ActivateInputField();
            
            shakeSequence?.Kill();

            shakeSequence = DOTween.Sequence();
            float xPosition = transform.localPosition.x;
            shakeSequence.Append(transform.DOLocalMoveX(xPosition - shakeAmount, duration / 3)).SetEase(ease);
            shakeSequence.Append(transform.DOLocalMoveX(xPosition + shakeAmount, duration / 3)).SetEase(ease);
            shakeSequence.Append(transform.DOLocalMoveX(xPosition, duration / 3)).SetEase(ease);
        }
    }

}

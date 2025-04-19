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
    [SerializeField] float duration = .2f;
    [SerializeField] float shakeAmount = 1;
    [SerializeField] Ease ease = Ease.Linear;

    [Header("Password")]
    [SerializeField] List<string> password;


    Sequence shakeSequence;
    Window unlockWindow;
    Window lockWindow;
    private void Start()
    {
        lockWindow = lockScreen.GetComponent<Window>();
        unlockWindow = unlockScreen.GetComponent<Window>();
    }
    public void ReadInput(string input)
    {
        Debug.Log("reading password input");
        input = new string(input.ToLower());
        if (password.Contains(input))
        {
            inputField.text = "";

            if (lockWindow != null)
                lockWindow.ToggleWindow(false);
            else
                lockScreen.SetActive(false);
            
            if (unlockWindow != null)
                unlockWindow.ToggleWindow(true);
            else
                unlockScreen.SetActive(true);
        }
        else if (inputField != null && inputField.text != "")
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
        if (inputField == null)
            Debug.LogWarning("Input field should not be null");
    }

}

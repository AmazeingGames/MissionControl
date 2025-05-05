using DG.Tweening;
using System;
using System.Collections;
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

    [Header("Properties")]
    [SerializeField] Window.WindowType myScreenToOpen;
    [SerializeField] List<string> password;

    Sequence shakeSequence;
    Window unlockWindow;
    Window lockWindow;

    public static EventHandler<EnterPasswordEventArgs> EnterPasswordEventHandler;

    bool wasFocused;

    private void Start()
    {
        lockWindow = lockScreen.GetComponent<Window>();
        unlockWindow = unlockScreen.GetComponent<Window>();
    }

    private void Update()
    {
        if (!inputField.isFocused)
            StartCoroutine(SetWasFocused_CO());
        else
            wasFocused = inputField.isFocused;

        if (wasFocused && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Return)))
            ReadInput(inputField.text);
    }

    IEnumerator SetWasFocused_CO()
    {
        yield return new WaitForSeconds(.1f);

        wasFocused = inputField.isFocused;
    }

    public void ReadInput(string input)
    {
        Debug.Log("reading password input");
        input = new string(input.ToLower());

        if (password.Contains(input))
        {
            inputField.text = "";

            if (lockWindow != null)
                lockWindow.SetWindow(false);
            else
                lockScreen.SetActive(false);
            
            if (unlockWindow != null)
                unlockWindow.SetWindow(true);
            else
                unlockScreen.SetActive(true);

            EnterPasswordEventHandler?.Invoke(this, new(myScreenToOpen));
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

public class EnterPasswordEventArgs : EventArgs
{
    public Window.WindowType myScreenToOpen;

    public EnterPasswordEventArgs(Window.WindowType myScreenToOpen)
    {
        this.myScreenToOpen = myScreenToOpen;
    }
}

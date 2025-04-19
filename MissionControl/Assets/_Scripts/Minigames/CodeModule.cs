using UnityEngine;
using System;
using System.Collections;

public class CodeModule : MonoBehaviour
{
    public enum CodeTypes { numbers, colors }

    [SerializeField] string codeDigit;
    [SerializeField] CodeTypes codeType; 

    public static Action<SendCodeDigitEventArgs> OnSendCodeDigit; //string1 is codeType, string 2 is the digit that you want to send as part of the secret code

    SpriteRenderer spriteRenderer;
    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        StartCoroutine(ResetButtonColor());
        OnSendCodeDigit?.Invoke(new SendCodeDigitEventArgs(codeType, codeDigit));
    }

    IEnumerator ResetButtonColor()
    {
        while (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            yield return null;
        }

        Color startingColor = spriteRenderer.color;
        spriteRenderer.color = Color.black;
        yield return new WaitForSeconds(.1f);
        spriteRenderer.color = startingColor;
    }
}


public class SendCodeDigitEventArgs
{
    public readonly CodeModule.CodeTypes myCodeType;
    public readonly string digit;

    public SendCodeDigitEventArgs(CodeModule.CodeTypes myCodeType, string digit)
    {
        this.myCodeType = myCodeType;
        this.digit = digit;
    }
}

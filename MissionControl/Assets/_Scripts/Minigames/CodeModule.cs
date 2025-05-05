using UnityEngine;
using System;
using System.Collections;

public class CodeModule : MonoBehaviour
{
    [SerializeField] char character;
    [SerializeField] Color pressedColor;

    public static Action<SendCodeDigitEventArgs> SendCharacterEventHandler; 

    SpriteRenderer spriteRenderer;

    Color startingColor;

    private void OnMouseDown()
    {
        StartCoroutine(ResetButtonColor());
        SendCharacterEventHandler?.Invoke(new SendCodeDigitEventArgs(character));
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingColor = spriteRenderer.color;
    }

    IEnumerator ResetButtonColor()
    {
        spriteRenderer.color = pressedColor;
        yield return new WaitForSeconds(.1f);
        spriteRenderer.color = startingColor;
    }
}


public class SendCodeDigitEventArgs
{
    public readonly char digit;

    public SendCodeDigitEventArgs(char digit)
    {
        this.digit = digit;
    }
}

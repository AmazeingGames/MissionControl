using UnityEngine;
using System;
using System.Collections;

public class CodeModule : MonoBehaviour
{
    [SerializeField] char character;

    public static Action<SendCodeDigitEventArgs> SendCharacterEventHandler; 

    SpriteRenderer spriteRenderer;

    private void OnMouseDown()
    {
        StartCoroutine(ResetButtonColor());
        SendCharacterEventHandler?.Invoke(new SendCodeDigitEventArgs(character));
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
    public readonly char digit;

    public SendCodeDigitEventArgs(char digit)
    {
        this.digit = digit;
    }
}

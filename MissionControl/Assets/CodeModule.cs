using UnityEngine;
using System;
using System.Collections;

public class CodeModule : MonoBehaviour
{
    public enum CodeTypes { numbers, colors }

    [SerializeField] string codeDigit;
    [SerializeField] CodeTypes codeType; 

    public static Action<CodeTypes, string> OnSendCodeDigit; //string1 is codeType, string 2 is the digit that you want to send as part of the secret code

    private void OnMouseDown()
    {
        StartCoroutine(ResetButtonColor());
        OnSendCodeDigit?.Invoke(codeType, codeDigit);
        
    }

    IEnumerator ResetButtonColor()
    {
        Color startingColor = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = Color.black;
        yield return new WaitForSeconds(.1f);
        gameObject.GetComponent<SpriteRenderer>().color = startingColor;
    }
}

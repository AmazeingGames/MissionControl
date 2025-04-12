using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CameraConnectApp : MonoBehaviour
{
    [SerializeField] GameObject accessCameraButton;
    [SerializeField] TMPro.TMP_Text lastCodeText;

    //Which IP addresses belong with which color codes, and number codes
    List<(string a, string b, string c)> ipCodes = new List<(string a, string b, string c)> // a is IPs, b is color codes, c is number codes
    {
        ("192.168.0.1", "BGRYY", "2314"),
        ("10.0.0.1", "RPRPG", "1323"),
        ("172.16.01", "OBPOB", "6301"),
        ("255.100.42.7", "BBBYR", "6301"),
    };

    string lastInputIpAddress;
    string lastInputColorCode = "";
    string lastInputNumberCode = "";

    [SerializeField] int colorCodeLength;
    [SerializeField] int numberCodeLength;

    //Takes players input for the IP
    public void ReadIpInput(string input) =>
        lastInputIpAddress = input;

    //Called when a code value is clicked
    void HandleCodeDigit(CodeModule.CodeTypes codeType, string codeDigit)
    {
        if (codeType == CodeModule.CodeTypes.colors)
            HandleColorCode(codeDigit);
        else if (codeType == CodeModule.CodeTypes.numbers)
            HandleNumberCode(codeDigit);
    }

    void HandleColorCode(string codeDigit)
    {
        lastInputNumberCode = "";
        if (lastInputColorCode.Length + 1 > colorCodeLength)
            lastInputColorCode = "";

        lastInputColorCode += codeDigit;
        lastCodeText.text = lastInputColorCode;
        foreach (var pair in ipCodes)
        {
            if (pair.a == lastInputIpAddress && pair.b == lastInputColorCode)
            {
                accessCameraButton.SetActive(true);
                lastInputColorCode = "";
                return;
            }
        }
    }

    void HandleNumberCode(string codeDigit)
    {
        lastInputColorCode = "";
        if (lastInputNumberCode.Length + 1 > numberCodeLength)
            lastInputNumberCode = "";

        lastInputNumberCode += codeDigit;
        lastCodeText.text = lastInputNumberCode;
        foreach (var pair in ipCodes)
        {
            if (pair.a == lastInputIpAddress && pair.c == lastInputNumberCode)
            {
                accessCameraButton.SetActive(true);
                lastInputNumberCode = "";
                return;
            }
        }
    }

    void OnEnable() =>
        CodeModule.OnSendCodeDigit += HandleCodeDigit;

    void OnDisable() =>
        CodeModule.OnSendCodeDigit -= HandleCodeDigit;
}

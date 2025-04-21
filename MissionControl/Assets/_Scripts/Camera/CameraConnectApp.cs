using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CameraConnectApp : MonoBehaviour
{
    [SerializeField] GameObject accessCameraButton;
    [SerializeField] TMPro.TMP_Text lastCodeText;

    //Which IP addresses belong with which color codes, and number codes
    readonly List<IPInformation> ipCodes = new List<IPInformation> // a is IPs, b is color codes, c is number codes
    {
        {new IPInformation("192.168.0.1", "BGRYY", "2314")},
        {new IPInformation ("10.0.0.1", "RPRPG", "1323")},
        {new IPInformation("172.16.01", "OBPOB", "6301")},
        {new IPInformation("255.100.42.7", "BBBYR", "6301")}
    };

    public string lastInputIpAddress;
    string lastInputColorCode = "";
    string lastInputNumberCode = "";

    [SerializeField] int colorCodeLength;
    [SerializeField] int numberCodeLength;

    // Takes players input for the IP
    public void ReadIpInput(string input) =>
        lastInputIpAddress = input;

    //Called when a code value is clicked
    void HandleCodeDigit(SendCodeDigitEventArgs sendCodeDigit)
    {
        if (sendCodeDigit.myCodeType == CodeModule.CodeTypes.colors)
            HandleColorCode(sendCodeDigit.digit);
        else if (sendCodeDigit.myCodeType == CodeModule.CodeTypes.numbers)
            HandleNumberCode(sendCodeDigit.digit);
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
            if (pair.ipAddress == lastInputIpAddress && pair.colorCode == lastInputColorCode)
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
            if (pair.ipAddress == lastInputIpAddress && pair.numberCode == lastInputNumberCode)
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



public class IPInformation
{
    public readonly string ipAddress;
    public readonly string colorCode;
    public readonly string numberCode;

    public IPInformation(string ipAddress, string colorCode, string numberCode)
    {
        this.ipAddress = ipAddress;
        this.colorCode = colorCode;
        this.numberCode = numberCode;
    }
}

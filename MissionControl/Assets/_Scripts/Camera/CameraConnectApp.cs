using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static IPInformation;
using static CameraUnlockButton;

public class CameraConnectApp : MonoBehaviour
{
    [SerializeField] CameraUnlockButton accessCameraButton;
    [SerializeField] TMPro.TMP_Text colorCode_TMP;
    [SerializeField] TMPro.TMP_InputField ipAddress_Input;

    // Which IP addresses belong with which color codes, and number codes
    List<IPInformation> ipsInformation = new()
    {
        { new IPInformation("192.168.0.1",  "RBRYY", Room.CaptainsQuarters) },
        { new IPInformation("10.0.0.1",     "RPRPG", Room.DwellingA1) },
        { new IPInformation("255.100.42.7", "BBBYR", Room.DwellingA2) },
        // { new IPInformation("172.16.01",    "OBPOB", Room.None) },
    };

    string colorCode = "";
    string ColorCode
    {
        get => colorCode;
        set
        {
            colorCode = value;
            colorCode_TMP.text = colorCode;
        }
    }

    [SerializeField] int colorCodeLength;
    [SerializeField] int numberCodeLength;

    void OnEnable()
    {
        CameraUnlockButton.UnlockCameraEventHandler += HandleCameraUnlock;
        CodeModule.SendCharacterEventHandler += HandleSendCharacter;
    }
    void OnDisable()
    {
        CameraUnlockButton.UnlockCameraEventHandler -= HandleCameraUnlock;
        CodeModule.SendCharacterEventHandler -= HandleSendCharacter;
    }

    private void Start()
    {
        accessCameraButton.Initialize(null);
    }

    private void Update()
    {
        for (int i = 0; i < ipsInformation.Count; i++)
        {
            IPInformation ipInformation = ipsInformation[i];

            bool foundMatch = ipInformation.ipAddress == ipAddress_Input.text && ipInformation.colorCode == ColorCode;

            if (foundMatch)
            {
                accessCameraButton.Initialize(ipInformation);
                return;
            }
            else if (i == ipsInformation.Count - 1)
                accessCameraButton.Initialize(null);
        }
    }

    void HandleCameraUnlock(object sender, UnlockCameraEventArgs e)
    {
        ColorCode = "";
        ipAddress_Input.text = "";

        accessCameraButton.Initialize(null);
    }

    //Called when a code value is clicked
    void HandleSendCharacter(SendCodeDigitEventArgs sendCodeDigit)
    {
        if (ColorCode.Length + 1 > colorCodeLength)
        {
            var shiftedOver = "";

            for (int i = 0; i < ColorCode.Length - 1; i++)
                shiftedOver += ColorCode[i + 1];
            ColorCode = shiftedOver;
        }

        ColorCode += sendCodeDigit.digit;
    }
}

public class IPInformation
{
    public enum Room { None, CaptainsQuarters, DwellingA1, DwellingA2 }

    public readonly string ipAddress;
    public readonly string colorCode;
    public readonly Room myRoom;

    public bool HasUnlockedCamera { get; set; } = false;

    public IPInformation(string ipAddress, string colorCode, Room myRoom)
    {
        this.ipAddress = ipAddress;
        this.colorCode = colorCode;
        this.myRoom = myRoom;
    }

    public void HandleCameraUnlock()
    {
        HasUnlockedCamera = true;
    }
}

using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class CameraUnlockButton : MonoBehaviour, IPointerClickHandler
{
    IPInformation ipInformation;

    public static EventHandler<UnlockCameraEventArgs> UnlockCameraEventHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        Assert.IsNotNull(ipInformation, "IP information should not be null while the button is enabled");
        UnlockCameraEventHandler?.Invoke(this, new(ipInformation));
    }

    public void Initialize(IPInformation ipInformation)
    {
        this.ipInformation = ipInformation;
       
        gameObject.SetActive(ipInformation != null);
    }   
}

public class UnlockCameraEventArgs : EventArgs
{
    public readonly IPInformation ipInformation;
    public UnlockCameraEventArgs(IPInformation ipInformation)
    {
        this.ipInformation = ipInformation;
    }
}

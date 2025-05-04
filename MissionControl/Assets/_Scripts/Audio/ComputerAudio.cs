using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class ComputerAudio : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    //I know this in not the way you'll want this to look but it's an easy solution for now lol
    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.inst.PlayOneShot(AudioHelper.inst.MouseDown);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AudioManager.inst.PlayOneShot(AudioHelper.inst.MouseUp);
    }
}

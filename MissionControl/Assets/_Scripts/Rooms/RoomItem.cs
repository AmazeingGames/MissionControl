using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] EntryData entryData;
    [SerializeField] IPInformation.Room roomOrigin;

    public static EventHandler<ClickItemEventArgs> ClickItemEventHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickItemEventHandler?.Invoke(this, new(entryData, roomOrigin));
    }
}

public class ClickItemEventArgs : EventArgs
{
    public readonly IPInformation.Room roomOrigin;
    public readonly EntryData entryData;
    public ClickItemEventArgs(EntryData entryData, IPInformation.Room roomOrigin)
    {
        this.entryData = entryData;
        this.roomOrigin = roomOrigin;
    }
}

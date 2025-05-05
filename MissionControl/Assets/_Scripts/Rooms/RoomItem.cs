using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] EntryData entryData;

    public static EventHandler<ClickItemEventArgs> ClickItemEventHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickItemEventHandler?.Invoke(this, new(entryData));
    }
}

public class ClickItemEventArgs : EventArgs
{
    public readonly EntryData entryData;
    public ClickItemEventArgs(EntryData entryData)
    {
        this.entryData = entryData;
    }
}

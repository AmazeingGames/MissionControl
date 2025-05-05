using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] EntryData entryData;

    public static EventHandler<UnlockLogsEventArgs> UnlockLogsEventHandler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        entryData.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UnlockLogsEventHandler?.Invoke(this, new(entryData));
    }
}

public class UnlockLogsEventArgs : EventArgs
{
    public readonly EntryData entryData;
    public UnlockLogsEventArgs(EntryData entryData)
    {
        this.entryData = entryData;
    }
}

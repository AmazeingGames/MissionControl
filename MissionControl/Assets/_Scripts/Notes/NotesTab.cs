using System;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class NotesTab : MonoBehaviour, IPointerClickHandler
{
    [Header("Properties")]
    [SerializeField] CrewData crewData;

    [Header("Components")]
    [SerializeField] Image image;

    public static EventHandler<ClickTabEventArgs> ClickTabEventHandler;

    private void Start()
    {
        if (Application.isEditor)
            return;
        DataMatch();
    }
    private void Update()
    {
        if (image != null && crewData != null)
            DataMatch();
    }

    void DataMatch()
    {
        image.sprite = crewData.Icon;
        image.color = crewData.IconColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked tab");
        ClickTabEventHandler?.Invoke(this, new(crewData));
    }
}

public class ClickTabEventArgs : EventArgs
{
    public readonly CrewData crewData;

    public ClickTabEventArgs(CrewData crewData)
    {
        this.crewData = crewData;
    }
}
using System;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NotesTab : MonoBehaviour, IPointerClickHandler
{
    [Header("Properties")]
    [SerializeField] CrewData crewData;

    [Header("Components")]
    [SerializeField] Image image;
    static IClickTab clickTabHandler;
    public static IClickTab ClickTabHandler 
    { 
        get 
            => clickTabHandler;
        set
        {
            if (clickTabHandler != null)
                Debug.LogWarning($"Trying to set handler to {value}, when it's already set to {clickTabHandler}");
            clickTabHandler = value;
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Quit");
    }

    private void OnValidate()
    {
        if (image != null && crewData != null)
            DataMatch();
    }

    private void Start()
    {      
        DataMatch();
    }   

    void DataMatch()
    {
        image.sprite = crewData.Icon;
        image.color = crewData.IconColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickTabHandler?.OnClickTab(crewData);
    }
}

public interface IClickTab
{
    void OnClickTab(CrewData crewData);
}
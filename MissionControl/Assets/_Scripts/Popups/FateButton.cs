using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class FateButton : MonoBehaviour, IPointerClickHandler
{
    
    static IClickFate clickFateHandler;
    public static IClickFate ClickFateHandler
    {
        get
            => clickFateHandler;
        set
        {
            if (clickFateHandler != null)
                Debug.LogWarning($"Trying to set handler to {value}, when it's already set to {clickFateHandler}");
            clickFateHandler = value;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickFateHandler?.OnClickFate();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public interface IClickFate
{
    void OnClickFate();
}

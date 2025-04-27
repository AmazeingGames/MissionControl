using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] PopupsManager.Popup myPopup;
    [SerializeField] bool isOpening;
    static IClickPopup clickPopupHandler;
    public static IClickPopup ClickPopupHandler
    {
        get
            => clickPopupHandler;
        set
        {
            if (clickPopupHandler != null)
                Debug.LogWarning($"Trying to set handler to {value}, when it's already set to {clickPopupHandler}");
            clickPopupHandler = value;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickPopupHandler?.OnClickPopup(myPopup, isOpening);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (myPopup == PopupsManager.Popup.None)
        {
            Transform lastParent = transform;
            while (true)
            {
                if (lastParent.parent != null)
                    lastParent = lastParent.parent;
                else
                    break;
            }

            string hierarchyPath = $"{lastParent.name}";

            for (int i = 0; i < lastParent.childCount; i++)
            {
                hierarchyPath += $" -> {lastParent.GetChild(0).name}";
            }

            throw new DataException($"Popup on {gameObject.name} should not be set to none. Hierarchy is as follows: {hierarchyPath}");
        }
    }
}

public interface IClickPopup
{
    void OnClickPopup(PopupsManager.Popup myPopup, bool isOpening);
}

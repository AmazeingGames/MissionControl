using System.Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class PopupButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] PopupsManager.Popup myPopup;
    [SerializeField] bool isOpening;
    public static IClickPopup clickPopupHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        clickPopupHandler?.OnClickPopupButton(myPopup, isOpening);
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

    void Update()
    {
        Assert.IsNotNull(clickPopupHandler, "Click popup handler should not be null");
    }
}

public interface IClickPopup { void OnClickPopupButton(PopupsManager.Popup myPopup, bool isOpening); }

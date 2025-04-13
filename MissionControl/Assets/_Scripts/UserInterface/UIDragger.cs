using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Component to easily drag UI elements
public class UIDragger : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform rectTransform;

    Vector3 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint))
        {
            offset = rectTransform.position - worldPoint;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 worldPoint))
        {
            rectTransform.position = worldPoint + offset;
        }
    }

}

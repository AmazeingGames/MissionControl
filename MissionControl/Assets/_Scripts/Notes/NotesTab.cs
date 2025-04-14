using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NotesTab : MonoBehaviour, IPointerClickHandler
{
    [Header("Properties")]
    [SerializeField] CrewData crewData;

    [Header("Components")]
    [SerializeField] Image image;

    public static IClickTab clickTabHandler;


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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickTabHandler?.OnClickTab(crewData);
    }
}

public interface IClickTab
{
    void OnClickTab(CrewData crewData);
}
using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FateSelectButton : MonoBehaviour, IPointerClickHandler, IPageButton
{
    [SerializeField] TextMeshProUGUI display_TMP;
    FateData fateData;

    public static ISelectFate selectFateHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        selectFateHandler?.HandleSelectFate(new FateArguments(fateData));
    }

    public void Initialize<T>(T data) where T : IPageData
    {
        fateData = data as FateData;

        if (fateData != null)
        {
            gameObject.SetActive(true);
            display_TMP.text = fateData.ShortDisplay;
            LogsManager.Log(LogsManager.Instance.PageButtonLoggingObject, $"Initialized fate select button | Short Text: {this.fateData.ShortDisplay}");
        }
        else
            gameObject.SetActive(false);
    }

}

public interface ISelectFate { void HandleSelectFate(FateArguments fateArguments); }

public class FateArguments 
{ 
    public readonly FateData fate;
    public FateArguments(FateData fate)
    {
        this.fate = fate;
    }
}
using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FateSelectButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI display_TMP;
    FateData fate;

    public static ISelectFate selectFateHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        selectFateHandler?.HandleSelectFate(new FateArguments(fate));
    }

    public void InitializeFate(FateData fate)
    {
        this.fate = fate;

        if (fate != null)
        {
            gameObject.SetActive(true);
            display_TMP.text = fate.ShortDisplay;
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
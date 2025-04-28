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
    public static IOpenSubfate openSubFateHandler;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (fate.SubFates == null || fate.SubFates.Count == 0)
            selectFateHandler?.HandleSelectFate(new FateArguments(fate));
        else
            openSubFateHandler?.HandleOpenFateSelect(new FateArguments(fate));
    }

    public void Initialize(FateData fate)
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

/*public class SelectFateEventArgs : EventArgs
{
    public bool SelectSubFate { get; private set; }
    public FateData Fate { get; private set; }
    public SelectFateEventArgs(FateData fate)
    {
        Fate = fate;
        SelectSubFate = fate.SubFates != null && fate.SubFates.Count > 0;
    }
}*/

public interface IOpenSubfate
{
    void HandleOpenFateSelect(FateArguments fateArguments);
}

public interface ISelectFate
{
    void HandleSelectFate(FateArguments fateArguments);
}

public class FateArguments
{
    public readonly FateData fate;

    public FateArguments(FateData fate)
    {
        this.fate = fate;
    }
}
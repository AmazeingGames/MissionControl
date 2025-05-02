using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FateData", menuName = "Scriptable Objects/FateData")]
public class FateData : ScriptableObject
{
    [field: SerializeField] public string ShortDisplay { get; private set; }
    [field: SerializeField] public string FullDisplay { get; private set; }
    [field: SerializeField] public bool HasPerpetrator { get; private set; }
    [field: SerializeField] public List<FateData> SubFates { get; private set; } = new();

    [field: Header("Guess and Check")]
    [field: SerializeField] public FateData SelectedSubFate { get; private set; }

    public bool HasSubFates => SubFates.Count > 0;
}

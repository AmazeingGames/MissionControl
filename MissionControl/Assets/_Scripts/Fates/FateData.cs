using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FateData", menuName = "Scriptable Objects/FateData")]
public class FateData : ScriptableObject
{
    [field: SerializeField] public string ShortDisplay { get; private set; }
    [field: SerializeField] public string FullDisplay { get; private set; }
    [field: SerializeField] public bool HasPerpetrator { get; private set; }
    [field: SerializeField] public List<SubFateData> SubFates { get; private set; }

    [field: Header("Guess and Check")]
    [field: SerializeField] public SubFateData SelectedSubFate { get; private set; }
}

using UnityEngine;

[CreateAssetMenu(fileName = "SubFateData", menuName = "Scriptable Objects/SubFateData")]
public class SubFateData : ScriptableObject
{
    [SerializeField] string shortDisplay;
    [SerializeField] string fullDisplay;
}


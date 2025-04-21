using Sirenix.Serialization;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "CrewData", menuName = "Scriptable Objects/CrewData")]
public class CrewData : ScriptableObject
{
    public enum Role { Captain, Engineer, Specialist, Scientist, Doctor }

    [field: PreviouslySerializedAs("name")]
    [field: SerializeField] public string Name { get; private set; }
    
    [field: PreviouslySerializedAs("myRole")]
    [field: SerializeField] public Role MyRole {get; private set; }

    [field: PreviouslySerializedAs("icon")]
    [field: SerializeField] public Sprite Icon {get; private set; }
    [field: SerializeField] public Sprite Picture {get; private set; }
    [field: SerializeField] public List<string> LogNotes { get; private set; }
    [field: SerializeField] public TMP_FontAsset NotesFont { get; private set; }
}

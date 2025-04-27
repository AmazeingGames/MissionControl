using Sirenix.Serialization;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CrewData", menuName = "Scriptable Objects/CrewData")]
public class CrewData : ScriptableObject
{
    public enum Role { Captain, Engineer, Specialist, Scientist, Doctor }

    [Header("Text")]
    [field: PreviouslySerializedAs("name")]
    [field: SerializeField] public string Name { get; private set; }
    
    [field: PreviouslySerializedAs("myRole")]
    [field: SerializeField] public Role MyRole {get; private set; }

    [Header("Visuals")]
    [field: PreviouslySerializedAs("icon")]
    [field: SerializeField] public Sprite Icon {get; private set; }
    [field: SerializeField] public Color IconColor { get; private set; }    
    [field: SerializeField] public Sprite Picture {get; private set; }
}

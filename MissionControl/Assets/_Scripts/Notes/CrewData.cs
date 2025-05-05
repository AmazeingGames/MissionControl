using Sirenix.Serialization;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CrewData", menuName = "Scriptable Objects/CrewData")]
public class CrewData : ScriptableObject
{
    public enum Role { Captain, Engineer, Specialist, Scientist, Doctor }
    public enum Name { Socha, Mel, Blake, Zen, Ethena, NoOne }

    [Header("Text")]
    [field: SerializeField] public Name MyName { get; private set; } = Name.NoOne;
    [field: SerializeField] public Role MyRole {get; private set; }

    [Header("Visuals")]
    [field: PreviouslySerializedAs("icon")]
    [field: SerializeField] public Sprite Icon {get; private set; }
    [field: SerializeField] public Color IconColor { get; private set; }    
    [field: SerializeField] public Sprite Picture {get; private set; }
}

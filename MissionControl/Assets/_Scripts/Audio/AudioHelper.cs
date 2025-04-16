using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;

// Stores every FMOD event reference and creates any needed event instances.
public class AudioHelper : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference GameAmbience { get; private set; }

    [field: Header("Music")]
    [field: SerializeField] public EventReference GameMusic { get; private set; }
    [field: SerializeField] public EventReference PauseMusic { get; private set; }
    [field: SerializeField] public EventReference TitleMusic { get; private set; }

    [field: Header("UI Actions")]
    [field: SerializeField] public EventReference UIHover { get; private set; }
    [field: SerializeField] public EventReference UISelect { get; private set; }

    [field: Header("Cutscenes")]
    [field: SerializeField] public EventReference BeatGame { get; private set; }
    [field: SerializeField] public EventReference Credits { get; private set; }

    [field: Header("Game Actions")]
    [field: SerializeField] public EventReference DiscoverClue { get; private set; }
    [field: SerializeField] public EventReference WriteEntry { get; private set; }
    [field: SerializeField] public EventReference CorrectDeduction { get; private set; }

    public Bus MasterBus { get; private set; }

    public EventInstance GameAmbience_Instance { get; private set; }
    public EventInstance GameMusic_Instance { get; private set; }
    public EventInstance PauseMusic_Instance { get; private set; }
    public EventInstance TitleMusic_Instance { get; private set; }

    void Start()
    {
        MasterBus = RuntimeManager.GetBus("bus:/");

        GameAmbience_Instance = CreateInstance(GameAmbience);
        TitleMusic_Instance = CreateInstance(GameMusic);
        GameMusic_Instance = CreateInstance(PauseMusic);
        PauseMusic_Instance = CreateInstance(TitleMusic);
    }

    EventInstance CreateInstance(EventReference eventReference)
    {
        if (eventReference.IsNull)
        {
            this.LogWarning("Event reference should not be null");
            return new EventInstance();
        }
        return RuntimeManager.CreateInstance(eventReference);
    }
       
}



using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static PlayerMovement;
using static System.Collections.Specialized.BitVector32;

// Simulates player movement by scaling canvases and gameobjects
public class PlayerMovement : MonoBehaviour
{
    public enum StationType { None, Computer, Desk, ComputerFull }

    [SerializeField] List<StationData> stationsData = new();
    [SerializeField] float movementDuration = .5f;

    StationType myStationType = StationType.None;

    public static IStationConnect StationConnectHandler;

    private void OnEnable()
    {
        GameStateManager.PerformGameActionEventHandler += HandlePerformGameAction;    
    }

    private void OnDisable()
    {
        GameStateManager.PerformGameActionEventHandler -= HandlePerformGameAction;
    }

    // I should define stations with scriptable objects, that way I can easily define and edit movement paths in the inspector
    // Update is called once per frame
    void Update()
    {        
        if (GameStateManager.IsFocusedOnInput || GameStateManager.MyPlayState == GameStateManager.PlayState.Notes)
            return;

        bool zoomIn = Input.GetButtonDown("ZoomIn");
        bool zoomOut = Input.GetButtonDown("ZoomOut");

        switch (myStationType)
        {
            case StationType.None:
                break;

            case StationType.Computer:
                if (zoomOut)
                    ChangeStation(StationType.Desk);
                else if (zoomIn)
                    ChangeStation(StationType.ComputerFull);
                break;

            case StationType.Desk:
                if (zoomIn)
                    ChangeStation(StationType.Computer);
                break;

            case StationType.ComputerFull:
                if (zoomOut)
                    ChangeStation(StationType.Computer);
                break;
        }
    }

    void HandlePerformGameAction(object sender, PerformGameActionEventArgs e)
    {
        switch (e.myGameAction)
        {
            case GameStateManager.GameAction.StartGame:
                ChangeStation(StationType.Desk);
                break;

            case GameStateManager.GameAction.None:
            case GameStateManager.GameAction.EnterMainMenu:
            case GameStateManager.GameAction.PauseGame:
            case GameStateManager.GameAction.ResumeGame:
            case GameStateManager.GameAction.LoseGame:
                break;
        }
    }

    void ChangeStation(StationType myStationType)
    {
        StationConnectHandler?.HandleConnectToStation(new(myStationType));

        if (myStationType == this.myStationType)
            Debug.LogWarning("Should generally not be trying to set station to its current station.");
        this.myStationType = myStationType;

        foreach (StationData stationData in stationsData)
            stationData.ChangeStation(myStationType, movementDuration);

    }
}

[Serializable]
class StationData
{
    [field: SerializeField] public PlayerMovement.StationType MyStationType { get; private set; }
    [field: SerializeField] public List<StationObject> StationObjects { get; private set; }
    
    // We might need a scale for every single station, but this is fine for a binary system

    public void ChangeStation(PlayerMovement.StationType myNewStation, float duration)
    {
        foreach (StationObject stationObject in StationObjects)
        {
            if (myNewStation == MyStationType)
            stationObject.gameObject.transform.DOScale(stationObject.InFocusScale, duration);
        }
    }
}

public interface IStationConnect { public void HandleConnectToStation(ConnectToStationArgs e); }

public class ConnectToStationArgs : EventArgs
{
    public readonly StationType myStationType;
    public ConnectToStationArgs(StationType myStationType)
    {
        this.myStationType = myStationType;
    }
}


[Serializable]
class StationObject
{
    [SerializeField] public GameObject gameObject;
    [field: SerializeField] public float InFocusScale { get; private set; } = 1;
}

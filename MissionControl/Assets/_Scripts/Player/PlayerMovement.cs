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
    public enum StationType { None, Computer, Desk }

    [SerializeField] List<StationData> stationsData = new();
    [SerializeField] float movementDuration = .5f;

    StationType myStationType = StationType.None;

    public static EventHandler<ConnectToStationEventArgs> ConnectToStationEventHandler;

    private void OnEnable()
    {
        GameStateManager.PerformGameActionEventHandler += HandlePerformGameAction;    
    }

    private void OnDisable()
    {
        GameStateManager.PerformGameActionEventHandler -= HandlePerformGameAction;
    }

    void HandlePerfromGameAction(object sender, PerformGameActionEventArgs e)
    {

    }

    // Update is called once per frame
    void Update()
    {        
        if (GameStateManager.IsFocusedOnInput || GameStateManager.MyPlayState == GameStateManager.PlayState.Notes)
            return;

        float verticalInput = Input.GetAxisRaw("Vertical");

        switch (myStationType)
        {
            case StationType.None:
                break;

            case StationType.Computer:
                if (verticalInput == -1)
                    ChangeStation(StationType.Desk);
                break;

            case StationType.Desk:
                if (verticalInput == 1)
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
        ConnectToStationEventHandler?.Invoke(this, new(myStationType));

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
    [field: SerializeField] public List<GameObject> StationObjects { get; private set; }
    
    // We might need a scale for every single station, but this is fine for a binary system
    [field: SerializeField] public float InFocusScale { get; private set; }
    [field: SerializeField] public float OutOfFocusScale { get; private set; }

    public void ChangeStation(PlayerMovement.StationType myNewStation, float duration)
    {
        float newScale = myNewStation == MyStationType ? InFocusScale : OutOfFocusScale;
        foreach (GameObject gameObject in StationObjects)
            gameObject.transform.DOScale(newScale, duration);
    }
}

public class ConnectToStationEventArgs : EventArgs
{
    public readonly PlayerMovement.StationType myStationType;
    public ConnectToStationEventArgs(StationType myStationType)
    {
        this.myStationType = myStationType;
    }
}
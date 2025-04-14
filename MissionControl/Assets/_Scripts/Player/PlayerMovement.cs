using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Simulates player movement by scaling canvases and gameobjects
public class PlayerMovement : MonoBehaviour
{
    public enum StationType { None, Computer, Desk }

    [SerializeField] List<Station> stationsData = new();
    [SerializeField] float movementDuration = .5f;

    StationType myStationType = StationType.None;

    private void OnEnable()
    {
        GameStateManager.GameStateActionEventHandler += HandleGameStateAction;    
    }

    private void OnDisable()
    {
        GameStateManager.GameStateActionEventHandler -= HandleGameStateAction;
    }

    // Update is called once per frame
    void Update()
    {
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

    void HandleGameStateAction(object sender, GameStateActionEventArgs e)
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
        Assert.IsTrue(myStationType != this.myStationType, "Should not be trying to set station to its current station.");
        this.myStationType = myStationType;

        foreach (Station station in stationsData)
            station.ChangeStation(myStationType, movementDuration);
    }
}

[Serializable]
class Station
{
    [SerializeField] public PlayerMovement.StationType MyStation { get; private set; }
    [SerializeField] public List<GameObject> StationObjects { get; private set; }
    
    // We might need a scale for every single station, but this is fine for a binary system
    [SerializeField] public float InFocusScale { get; private set; }
    [SerializeField] public float OutOfFocusScale { get; private set; }

    public Station(PlayerMovement.StationType myStation, List<GameObject> stationObjects, float inFocusScale, float outOfFocusScale)
    {
        MyStation = myStation;
        StationObjects = stationObjects;
        InFocusScale = inFocusScale;
        OutOfFocusScale = outOfFocusScale;
    }

    public void ChangeStation(PlayerMovement.StationType myNewStation, float duration)
    {
        float newScale = myNewStation == MyStation ? InFocusScale : OutOfFocusScale;
        foreach (GameObject gameObject in StationObjects)
            gameObject.transform.DOScale(newScale, duration);
    }
}

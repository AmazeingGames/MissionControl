using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static GameStateManager;
using static PlayerMovement;

// Scripts that need to communicate with one another instead communicate with the game manager:
// The Game Manager serves as a global hub of communication for all scripts.

// The number of repetitive, shallow modules might indicate some part of the design should change.
// I think this amount of indirection may be harmful: objects are no longer allowed to take care of themself, and the GameManager inserts itself in every part of the codebase in a way that is almost arbitrary

public class GameStateManager : MonoBehaviour, IClickGameButton, IToggleNotes, IStationConnect
{
    public enum NotesState { Notebook, Popup }
    public enum PlayState { None, Station, Notes }
    public enum GameState { None, InMenu, Running, Paused, Loading }
    public enum GameAction { None, EnterMainMenu, StartGame, PauseGame, ResumeGame, LoseGame, QuitGame }

    // Scripts should not access these variables directly, but instead get them through event handling
    public static bool IsFocusedOnInput { get => instance.inputFields.Any(i => i.isFocused); }

    public static PlayState MyPreviousPlayState { get; private set; }
    public static PlayState MyPlayState { get; private set; }
    public static GameState MyPreviousGameState { get; private set; }
    public static GameState MyGameState { get; private set; }
    public static GameAction MyLastGameAction { get; private set; }

    public static EventHandler<ChangeGameStateEventArgs>   ChangeGameStateEventHandler;
    public static EventHandler<ChangePlayStateEventArgs>   ChangePlayStateEventHandler;
    public static EventHandler<PerformGameActionEventArgs> PerformGameActionEventHandler;

    bool isFocusedOnInput;
    static GameStateManager instance;

    List<TMP_InputField> inputFields;

    readonly KeyCode pauseKey = KeyCode.Escape;

    // Start is called before the first frame update
    void Start()
    {
        NotesManager.ToggleNotesHandler = this;
        PlayerMovement.StationConnectHandler = this;

        Assert.IsNull(instance, "Only 1 game state manager should exist in the scene at a time.");
        instance = this;
        UIButton.GameStateActionHandler = this;
        
        PerformGameAction(GameAction.EnterMainMenu);

        var inputObjects = FindObjectsByType(typeof(TMP_InputField), FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        inputFields = inputObjects.Select(x => x.GetComponent<TMP_InputField>()).ToList();

        Assert.AreEqual(inputObjects.Count, inputFields.Count, "Input fields is not correctly grabbing InputField components from inputObjects.");
    }

    private void Update()
    {
        Assert.IsNotNull(this);

        isFocusedOnInput = IsFocusedOnInput;
        RunGameState();
    }

    public void HandleGameButton(GameAction myGameAction)
        => PerformGameAction(myGameAction);

    public void HandleConnectToStation(ConnectToStationArgs e)
        => ChangePlayState(PlayState.Station);

    public void HandleToggleNotes(ToggleNotesArgs e)
    {
        ChangePlayState(e.isOpening ? PlayState.Notes : PlayState.Station);
    }

    void RunGameState()
    {
        switch (MyGameState)
        {
            case GameState.Loading:
            case GameState.None:
            case GameState.InMenu:
                break;

            // In the future, I would like to better handle edge cases, and having to directly access scripts signals some amount of structure change is due 
            // Having a smooth, instantaneous transitions between attempted menu opens would be nice
            case GameState.Running:
                if (Input.GetKeyDown(pauseKey) && !IsFocusedOnInput)
                    PerformGameAction(GameAction.PauseGame);
                break;

            case GameState.Paused:
                if (Input.GetKeyDown(pauseKey))
                    PerformGameAction(GameAction.ResumeGame);
                break;

            default:
                throw new DataException("Game State not recognized by GameStateManager");
        }
    }

    /// <summary> Informs listeners when we perform a game action, and updates the game state accordingly. </summary>
    void PerformGameAction(GameAction action)
    {
        if (action == GameAction.None)
        {
            this.LogWarning("Cannont run comand 'none'.");
            return;
        }

        this.Log($"Performed game action: {action}");
        MyLastGameAction = action;
        PerformGameActionEventHandler?.Invoke(this, new(this, action));

        // Updates game state to fit the action
        switch (action)
        {
            case GameAction.EnterMainMenu:
            case GameAction.LoseGame:
                ChangeGameState(GameState.InMenu);
                break;

            case GameAction.ResumeGame:
            case GameAction.StartGame:
                ChangeGameState(GameState.Running);
                break;

            case GameAction.PauseGame:
                ChangeGameState(GameState.Paused);
                break;

            case GameAction.QuitGame:
            case GameAction.None:
                break;

            default:
                throw new DataException("Game Action not recognized by GameStateManager");
        }

        switch (action)
        {
            case GameAction.QuitGame:
                Application.Quit();
                break;
        }
    }

    /// <summary> Informs listeners on how to align with the current state of the game. </summary>
    /// <param name="myNewGameState"> The state of the game to update to. </param>
    void ChangeGameState(GameState myNewGameState)
    {
        if (myNewGameState == GameState.None)
        {
            this.LogWarning("Cannont update game state to 'none'.");
            return;
        }
        else if (myNewGameState == MyGameState)
        {
            this.LogWarning($"Cannont update game state to its own state ({myNewGameState}).");
            return;
        }

        MyPreviousGameState = MyGameState;
        MyGameState = myNewGameState;

        ChangeGameStateEventHandler?.Invoke(this, new(this, myNewGameState, MyPreviousGameState));
    }

    void ChangePlayState(PlayState myNewPlayState)
    {
        switch (myNewPlayState)
        {
            case PlayState.None:
            case PlayState.Station:
            case PlayState.Notes:
            break;

            default:
            throw new DataException("Play State not recognized by GameStateManager");
        }

        MyPreviousPlayState = MyPlayState;
        MyPlayState = myNewPlayState;

        ChangePlayStateEventHandler?.Invoke(this, new(myNewPlayState, MyPreviousPlayState));

        this.Log($"Changed play state to {myNewPlayState}");
    }
}

public class PerformGameActionEventArgs : EventArgs
{
    public readonly GameStateManager gameManager;
    public readonly GameAction myGameAction;

    public PerformGameActionEventArgs(GameStateManager gameManager, GameAction gameAction)
    {
        this.gameManager = gameManager;
        this.myGameAction = gameAction;
    }
}

public class ChangeGameStateEventArgs : EventArgs
{
    public readonly GameStateManager gameManager;
    public readonly GameState myNewState;
    public readonly GameState myPreviousState;

    public ChangeGameStateEventArgs(GameStateManager gameManager, GameState newState, GameState previousState)
    {
        this.gameManager = gameManager;
        this.myNewState = newState;
        this.myPreviousState = previousState;
    }
}

public class ChangePlayStateEventArgs : EventArgs
{
    public readonly PlayState myPlayState;
    public readonly PlayState myPreviousPlayState;

    public ChangePlayStateEventArgs(PlayState myPlayState, PlayState myPreviousPlayState)
    {
        this.myPlayState = myPlayState;
        this.myPreviousPlayState = myPreviousPlayState;
    }
}
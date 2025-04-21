using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static GameStateManager;
using static PlayerMovement;

// Responsible for knowing the global game state and informing listeners of important global events

/* 
 * It's unclear as to exactly which scripts should be responsible for communicating important actions to the Game State Manager in game state, as well as how they should do it as, as the game manager currently acts as 'middle-man' between different events.
 * I would potentially be interested in exploring a different communication patterns to use instead of events, namely dependency injection, because I think it's an interesting enough design problem worth looking into. 
 * The less events this class subscribes to the better, as it necessarily creates large chains of events.
*/

public class GameStateManager : MonoBehaviour, IGameStateActionHandler
{
    readonly KeyCode pauseKey = KeyCode.Escape;
    public enum PlayState { None, Station, Notes }
    public enum GameState { None, InMenu, Running, Paused, Loading }
    public enum GameAction { None, EnterMainMenu, StartGame, PauseGame, ResumeGame, LoseGame, QuitGame }

    static GameStateManager instance;
    public static bool IsFocusedOnInput { get => instance.inputFields.Any(i => i.isFocused); }
    public static PlayState MyPlayState { get; private set; }
    public static GameState MyGameState { get; private set; }
    
    public GameAction MyLastGameAction { get; private set; }

    // I really dislike the wordy names for these, but they're named this way to differentiate themselves from the GameplayManager's events
    public static EventHandler<GameStateChangeEventArgs> GameStateChangeEventHandler;
    public static EventHandler<PerformGameActionEventArgs> PerformGameActionEventHandler;

    List<TMP_InputField> inputFields;

    void OnEnable()
    {
        NotesManager.OpenNotesEventHandler += HandleOpenNotes;
        PlayerMovement.ConnectToStationEventHandler += HandleConnectToStation;
    }

    private void OnDisable()
    {
        NotesManager.OpenNotesEventHandler -= HandleOpenNotes;
        PlayerMovement.ConnectToStationEventHandler -= HandleConnectToStation;
    }

    void HandleConnectToStation(object sender, ConnectToStationEventArgs e)
    {
        MyPlayState = PlayState.Station;
    }    

    void HandleOpenNotes(object sender, OpenNotesEventArgs e)
    {
        MyPlayState = e.isOpening ? PlayState.Notes : PlayState.Station;
    }

    public void OnClick(GameAction myGameAction)
    {
        PerformGameAction(myGameAction);
    }


    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNull(instance, "Only 1 game state manager should exist in the scene at a time.");
        instance = this;
        UIButton.GameStateActionHandler = this as IGameStateActionHandler;
        
        PerformGameAction(GameAction.EnterMainMenu);

        var inputObjects = FindObjectsByType(typeof(TMP_InputField), FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        inputFields = inputObjects.Select(x => x.GetComponent<TMP_InputField>()).ToList();

        Assert.AreEqual(inputObjects.Count, inputFields.Count, "Input fields is not correctly grabbing InputField components from inputObjects.");
    }

    bool isFocusedOnInput;
    private void Update()
    {
        isFocusedOnInput = IsFocusedOnInput;
        GameStateUpdate();
    }

    void GameStateUpdate()
    {
        switch (MyGameState)
        {
            case GameState.None:
                break;
            case GameState.InMenu:
                break;

            // In the future, I would like to better handle edge cases, and having to directly access scripts signals some amount of structure change is due 
            // Having a smooth, instantaneous transitions between attempted menu opens would be nice
            case GameState.Running:
                if (Input.GetKeyDown(pauseKey) && !IsFocusedOnInput) // && !ScreenTransitions.IsPlayingTransitionAnimation)
                    PerformGameAction(GameAction.PauseGame);
                break;

            case GameState.Paused:
                if (Input.GetKeyDown(pauseKey)) // && !ScreenTransitions.IsPlayingTransitionAnimation)
                    PerformGameAction(GameAction.ResumeGame);
                break;
            case GameState.Loading:
                break;
        }
    }

    /// <summary> Informs listeners of a game action and updates the game state accordingly. </summary>
    /// <param name="action"> The game action to perform. </param>
    /// <param name="levelToLoad"> If we should load a level, otherwise leave at -1. </param>
    // Update game state in response to menu changes
    void PerformGameAction(GameAction action)
    {
        if (action == GameAction.None)
        {
            this.LogWarning("Cannont run comand 'none'.");
            return;
        }

        // Logger.Log($"Performed game action: {action}");
        MyLastGameAction = action;
        PerformGameActionEventHandler?.Invoke(this, new(this, action));

        // Updates game state to fit the action
        switch (action)
        {
            case GameAction.EnterMainMenu:
            case GameAction.LoseGame:
                OnGameStateChange(GameState.InMenu);
                break;

            case GameAction.ResumeGame:
            case GameAction.StartGame:
                OnGameStateChange(GameState.Running);
                break;

            case GameAction.PauseGame:
                OnGameStateChange(GameState.Paused);
                break;

            case GameAction.QuitGame:
            case GameAction.None:
                break;

            default:
                throw new Exception("Game action not recognized by GameStateManager");
        }

        switch (action)
        {
            case GameAction.None:
                break;
            case GameAction.EnterMainMenu:
                break;
            case GameAction.StartGame:
                break;
            case GameAction.PauseGame:
                break;
            case GameAction.ResumeGame:
                break;
            case GameAction.LoseGame:
                break;
            case GameAction.QuitGame:
                Application.Quit();
                break;
        }


    }

    /// <summary> Informs listeners on how to align with the current state of the game. </summary>
    /// <param name="newState"> The state of the game to update to. </param>
    void OnGameStateChange(GameState newState)
    {
        if (newState == GameState.None)
        {
            this.LogWarning("Cannont update game state to 'none'.");
            return;
        }
        else if (newState == MyGameState)
        {
            this.LogWarning($"Cannont update game state to its own state ({newState}).");
            return;
        }

        var previousState = MyGameState;
        MyGameState = newState;

        GameStateChangeEventHandler?.Invoke(this, new(this, newState, previousState));
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

public class GameStateChangeEventArgs : EventArgs
{
    public readonly GameStateManager gameManager;
    public readonly GameState myNewState;
    public readonly GameState myPreviousState;

    public GameStateChangeEventArgs(GameStateManager gameManager, GameState newState, GameState previousState)
    {
        this.gameManager = gameManager;
        this.myNewState = newState;
        this.myPreviousState = previousState;
    }
}

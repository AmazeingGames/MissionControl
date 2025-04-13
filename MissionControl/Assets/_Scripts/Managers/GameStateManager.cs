using System;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using static GameStateManager;

// Responsible for knowing the global game state and informing listeners of important global events

/* 
 * It's unclear as to exactly which scripts should be responsible for communicating important actions to the Game State Manager in game state, as well as how they should do it as, as the game manager currently acts as 'middle-man' between different events.
 * I would potentially be interested in exploring a different communication patterns to use instead of events, namely dependency injection, because I think it's an interesting enough design problem worth looking into. 
 * The less events this class subscribes to the better, as it necessarily creates large chains of events.
*/

public class GameStateManager : MonoBehaviour, IGameStateActionHandler
{
    readonly KeyCode pauseKey = KeyCode.Escape;

    public enum GameState { None, InMenu, Running, Paused, Loading }
    public enum GameAction { None, EnterMainMenu, PlayGame, PauseGame, ResumeGame, LoseGame }

    public GameState MyGameState { get; private set; }
    public GameAction MyLastGameAction { get; private set; }


    public static EventHandler<GameStateChangeEventArgs> GameStateChangeEventHandler;
    public static EventHandler<PerformGameActionEventArgs> GameStateActionEventHandler;


    // Start is called before the first frame update
    void Start()
    {
        UIButton.GameStateActionHandler = this as IGameStateActionHandler;
        
        PerformGameAction(GameAction.EnterMainMenu);
    }

    private void Update()
    {
        GameStateUpdate();
    }

    public void OnClick(GameAction myGameAction)
    {
        PerformGameAction(myGameAction);
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
                if (Input.GetKeyDown(pauseKey)) // && !ScreenTransitions.IsPlayingTransitionAnimation)
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

    /// <summary> Informs listerners of a game action and updates the game state accordingly. </summary>
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
        GameStateActionEventHandler?.Invoke(this, new(this, action));

        // Updates game state to fit the action
        switch (action)
        {
            case GameAction.EnterMainMenu:
            case GameAction.LoseGame:
                OnGameStateChange(GameState.InMenu);
                break;

            case GameAction.ResumeGame:
            case GameAction.PlayGame:
                OnGameStateChange(GameState.Running);
                break;

            case GameAction.PauseGame:
                OnGameStateChange(GameState.Paused);
                break;

            case GameAction.None:
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

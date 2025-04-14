using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;
using static UIButton;

// The difference between GameplayManager and GameStateManager is a little muddy, but essentially Gameplay Manager is directly responsibile for all events that occur inside while the game is actively being played, whereas gamestate manager is responsible for the overall global state of the game, even when it's not being played.
public class GameplayManager : MonoBehaviour, IGameplayActionHandler
{
    public static EventHandler<PerformGameplayActionEventArgs> GameplayActionEventHandler;

    public PlayState MyPlayState { get; private set; }
    public PlayState MyPreviousPlayState { get; private set; }

    public enum PlayState { None, }

    public static int EmployeesHiredToday { get; private set; } = 0;
    public static int EmployeesRejectedToday { get; private set; } = 0;
    public static int MistakesMadeToday { get; private set; } = 0;

    private void OnEnable()
    {
        GameStateManager.GameStateActionEventHandler += HandleGameAction;
    }

    private void OnDisable()
    {
        GameStateManager.GameStateActionEventHandler -= HandleGameAction;
    }

    void Start()
    {
        UIButton.GameplayActionHandler = this as IGameplayActionHandler;
    }    

    void HandleGameAction(object sender, GameStateActionEventArgs e)
    {
        switch (e.myGameAction)
        {
            case GameStateManager.GameAction.StartGame:
            break;
        }
    }

    public void OnClick(PlayState myPlayState)
    {
        throw new NotImplementedException();
    }
}

public class PerformGameplayActionEventArgs : EventArgs
{
    public readonly GameplayManager.PlayState myDayState;

    public PerformGameplayActionEventArgs(GameplayManager.PlayState myDayState)
    {
        this.myDayState = myDayState;
    }
}


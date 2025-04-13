using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;
using static UIButton;

public class GameplayManager : MonoBehaviour
{
    [field: SerializeField] List<int> employeesPerDay = new();

    public static EventHandler<PerformGameplayActionEventArgs> PerformGameplayActionEventHandler;

    public DayState MyDayState { get; private set; }
    public DayState MyPreviousDayState { get; private set; }
    public static int CurrentDay { get; private set; }
    public static int RemainingEmployees { get; private set; }

    public enum DayState { None, StartDay, StartWork, EndWork, EndDay }

    public static int EmployeesHiredToday { get; private set; } = 0;
    public static int EmployeesRejectedToday { get; private set; } = 0;
    public static int MistakesMadeToday { get; private set; } = 0;


    private void OnEnable()
    {
        GameStateManager.PerformGameActionEventHandler += HandleGameAction;
        UIButton.UIInteractEventHandler += HandleUIInteract;
    }

    private void OnDisable()
    {
        GameStateManager.PerformGameActionEventHandler -= HandleGameAction;
        UIButton.UIInteractEventHandler -= HandleUIInteract;
    }

    void HandleUIInteract(object sender, UIInteractEventArgs e)
    {
    }

    void HandleGameAction(object sender, PerformGameActionEventArgs e)
    {
        switch (e.myGameAction)
        {
            case GameStateManager.GameAction.PlayGame:
            break;
        }
    }
}

public class PerformGameplayActionEventArgs : EventArgs
{
    public readonly GameplayManager.DayState myDayState;

    public PerformGameplayActionEventArgs(GameplayManager.DayState myDayState)
    {
        this.myDayState = myDayState;
    }
}


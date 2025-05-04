using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class ComputerIcon : MonoBehaviour, IPointerClickHandler
{
    [Header("Properties")]
    [SerializeField] ScreenType myWindowToOpen;
    [SerializeField] ButtonType myButtonType;
    [SerializeField] ConditionalType myConditionalType;

    [Header("Components")]
    [SerializeField] Window defaultWindow;
    [SerializeField] Window conditionalWindow;

    bool hasCompletedCondition = false;
    public enum ButtonType { None, DesktopIcon, TaskbarIcon }
    public enum ScreenType { None, LockScreen, HomeScreen, Logs }
    public enum ConditionalType { None, Password }

    private void OnValidate()
    {
        Assert.IsTrue(myButtonType != ButtonType.None);
        Assert.IsTrue(myWindowToOpen != ScreenType.None);
    }

    void OnEnable()
    {
        PasswordUnlock.EnterPasswordEventHandler += HandleEnterPassword;
    }

    void OnDisable()
    {
        PasswordUnlock.EnterPasswordEventHandler += HandleEnterPassword;
    }

    void HandleEnterPassword(object sender, EnterPasswordEventArgs e)
    {
        LogsManager.Log(LogsManager.Instance.ComputerIconLoggingObject, $"Handled enter correct password | {e.myScreenToOpen} to {myWindowToOpen}");

        if (e.myScreenToOpen == myWindowToOpen && myConditionalType == ConditionalType.Password)
        {
            LogsManager.Log(LogsManager.Instance.ComputerIconLoggingObject, "Desktop Icon's condition has been met");
            hasCompletedCondition = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var windowToOpen = hasCompletedCondition ? conditionalWindow : defaultWindow;

        switch (myButtonType)
        {
            case ButtonType.DesktopIcon:
                windowToOpen.SetWindow(true);
            break;

            case ButtonType.TaskbarIcon:
                windowToOpen.ToggleWindow();
            break;

            default:
                throw new System.NotImplementedException("ButtonType not handled by switch statement.");
        }
    }
}

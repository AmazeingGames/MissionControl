using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : MonoBehaviour
{
    public List<Window.WindowType> openWindowTypes = new();

    private void OnEnable()
    {
        Window.SetWindowEventHandler += HandleSetWindow;
    }

    private void OnDisable()
    {
        Window.SetWindowEventHandler -= HandleSetWindow;
    }

    void HandleSetWindow(object sender, SetWindowEventArgs e)
    {
        if (e.isOpening)
            openWindowTypes.Add(e.myWindowType);
        else
            openWindowTypes.Remove(e.myWindowType);
    }    
}

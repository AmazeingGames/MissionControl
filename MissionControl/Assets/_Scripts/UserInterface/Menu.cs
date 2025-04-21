using static UIManager;
using System.Collections.Generic;
using System;
using UnityEngine;

// Represents a menu in the game
[Serializable]
class Menu
{
    [field: SerializeField] public Canvas Canvas { get; private set; } = new();
    // [field: SerializeField] public bool CanSeePaper { get; private set; }
    public Transform CanvasElements { get; private set; }
    public MenuType MenuType { get; private set; }

    bool isReady = false;

    UIManager uiManager;

    public static EventHandler<SetCanvasEventArgs> SetCanvasEventHandler;

    public enum CanvasAction { StartSet, FinishSet }

    public void SetCanvas(bool setActive, bool needsToMoveOutOfFrame = false, bool wasNested = false)
    {
        if (setActive == isReady)
            return;

        isReady = setActive;

        uiManager.StartCoroutine(SetCanvasActive(setActive, needsToMoveOutOfFrame, wasNested));
    }

    System.Collections.IEnumerator SetCanvasActive(bool ready, bool needsToMoveOutOfFrame, bool wasNested)
    {
        OnCanvasAction(ready, needsToMoveOutOfFrame, wasNested, CanvasAction.StartSet);

        // Enables the canvas before it starts moving into frame
        if (ready)
            OnCanvasAction(ready, needsToMoveOutOfFrame, wasNested, CanvasAction.FinishSet);

        // Waits until the canvas moves out of frame before disabling it
        while (UIAnimator.IsPlayingTransitionAnimation)
            yield return null;

        if (!ready)
            OnCanvasAction(ready, needsToMoveOutOfFrame, wasNested, CanvasAction.FinishSet);
    }

    void OnCanvasAction(bool setActive, bool moveOutOfFrame, bool wasNested, CanvasAction mySetCanvasAction)
    {
        if (Canvas == null)
            return;

        if (mySetCanvasAction == CanvasAction.FinishSet)
            Canvas.gameObject.SetActive(setActive);

        LogsManager.Log(uiManager.gameObject, $"Setting {Canvas.name} canvas {(setActive ? "active" : "disabled")}");
        SetCanvasEventHandler?.Invoke(this, new(this, setActive, CanvasElements, moveOutOfFrame, wasNested, mySetCanvasAction));

    }

    public void InitializeMenu(UIManager uiManager, MenuType menuType)
    {
        if (Canvas == null)
        {
            LogsManager.Log(uiManager.gameObject, "Gameobject should not be null");
            return;
        }

        for (int i = 0; i < Canvas.transform.childCount; i++)
        {
            var child = Canvas.transform.GetChild(i);

            if (child.name == "Elements")
                CanvasElements = child;
        }

        this.uiManager = uiManager;
        this.MenuType = menuType;
    }

    public class SetCanvasEventArgs : EventArgs
    {
        public readonly Menu menu;
        public readonly bool setActive;

        public readonly Transform canvasElements;
        public readonly bool needsToMoveOutOfFrame;
        public readonly bool wasNested;
        public readonly CanvasAction mySetAction;

        public SetCanvasEventArgs(Menu menu, bool setActive, Transform canvasElements, bool needsToMoveOutOfFrame, bool wasNested, CanvasAction mySetAction)
        {
            this.menu = menu;
            this.setActive = setActive;
            this.canvasElements = canvasElements;
            this.needsToMoveOutOfFrame = needsToMoveOutOfFrame;
            this.wasNested = wasNested;
            this.mySetAction = mySetAction;
        }
    }
}

public class MenuChangeEventArgs
{
    public readonly MenuType newMenuType;
    public readonly MenuType previousMenuType;
    public readonly bool isEnablingMenu;
    public MenuChangeEventArgs(MenuType newMenuType, MenuType previousMenuType, bool isEnablingMenu)
    {
        this.newMenuType = newMenuType;
        this.previousMenuType = previousMenuType;
        this.isEnablingMenu = isEnablingMenu;
    }
}
using Unity.VisualScripting;
using UnityEngine;

public class ComputerManager : MonoBehaviour
{
    [SerializeField] bool startUnlocked;

    [Header("Locked")]
    [SerializeField] GameObject computerLocked;

    [Header("Unlocked")]
    [SerializeField] GameObject computerUnlocked;
    [SerializeField] GameObject windows;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetChildrenActive(windows.transform, false);
        SetChildrenActive(computerUnlocked.transform, true, true);
        SetChildrenActive(computerLocked.transform, true, true);

        if (Application.isEditor && startUnlocked)
            ToggleLock(setLocked: false);
        else
            ToggleLock(setLocked: true);
    }

    void SetChildrenActive(Transform parent, bool setActive, bool alsoSetParent = false)
    {
        if (alsoSetParent)
            parent.gameObject.SetActive(setActive);

        for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.SetActive(setActive);
    }

    void ToggleLock(bool setLocked)
    {
        computerLocked.SetActive(setLocked);
        computerUnlocked.SetActive(!setLocked);
    }
}

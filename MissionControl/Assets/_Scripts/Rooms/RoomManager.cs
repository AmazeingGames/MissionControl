using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SearchService;
using UnityEngine.UI;
using static IPInformation;

public class RoomManager : MonoBehaviour
{
    [SerializeField] GameObject shipMap;
    [SerializeField] RoomButton closeRoomButton;
    [SerializeField] GameObject roomDisplaysParent;
    [SerializeField] GameObject roomButtonsParent;

    List<RoomButton> roomButtons = new List<RoomButton>();
    private void OnEnable()
    {
        RoomButton.ToggleCameraEventHandler += HandleToggleCamera;
        CameraUnlockButton.UnlockCameraEventHandler += HandleUnlockCamera;
    }

    private void OnDisable()
    {
        RoomButton.ToggleCameraEventHandler -= HandleToggleCamera;
        CameraUnlockButton.UnlockCameraEventHandler -= HandleUnlockCamera;
    }

    private void Start()
    {
        closeRoomButton.gameObject.SetActive(false);

        for (int i = 0; i < roomDisplaysParent.transform.childCount; i++)
        {
            var child = roomDisplaysParent.transform.GetChild(i);
            child.gameObject.SetActive(false);
        }

        for (int i = 0; i < roomButtonsParent.transform.childCount; i++)
        {
            RoomButton child = roomButtonsParent.transform.GetChild(i).GetComponent<RoomButton>();
            roomButtons.Add(child);
        }
    }

    void HandleUnlockCamera(object sender, UnlockCameraEventArgs e)
    {
        foreach (RoomButton roomButton in roomButtons)
        {
            Debug.Log($"handled camera unlock : {e.ipInformation.myRoom} | my camera is : {roomButton.myRoom}");
            if (e.ipInformation.myRoom != roomButton.myRoom)
                return;
            Debug.Log("unlocked camera");

            e.ipInformation.HandleCameraUnlock();

            if (roomButton.lockImage != null)
                roomButton.lockImage.gameObject.SetActive(false);
            roomButton.isUnlocked = true;
        }
        
    }


    void HandleToggleCamera(object sender, ToggleCameraEventArgs e)
    {
        shipMap.SetActive(!e.isOpening);
        closeRoomButton.gameObject.SetActive(e.isOpening);

        if (!e.isOpening)
        {
            for (int i = 0; i < roomDisplaysParent.transform.childCount; i++)
            {
                var child = roomDisplaysParent.transform.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }
        
    }
}

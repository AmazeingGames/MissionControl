using UnityEngine;
using UnityEngine.SearchService;

public class RoomManager : MonoBehaviour
{
    [SerializeField] GameObject captainsQuarters;

    private void OnEnable()
    {
        RoomButton.OpenCameraEventHandler += HandleOpenCamera;
    }

    private void OnDisable()
    {
        RoomButton.OpenCameraEventHandler -= HandleOpenCamera;
    }

    void HandleOpenCamera(object sender, OpenCameraEventArgs e)
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

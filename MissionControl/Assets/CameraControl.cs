using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] float scrollSpeed;
    [SerializeField] float distFromEdge;

    [SerializeField] float maxLeft;
    [SerializeField] float maxRight;

    Vector3 targetPosition;

    void Start() =>
        targetPosition = transform.position;

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        float lookDirection = 0f;

        //Checks the distance between the mouse and the edge of the screen
        if (mousePos.x >= Screen.width - distFromEdge)
            lookDirection = 1f;
        else if (mousePos.x <= distFromEdge)
            lookDirection = -1f;

        //Moves the camera in the direction determined
        targetPosition += new Vector3(lookDirection * scrollSpeed * Time.deltaTime, 0f, 0f);

        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);

        targetPosition.x = Mathf.Clamp(targetPosition.x, maxLeft, maxRight);

    }
}
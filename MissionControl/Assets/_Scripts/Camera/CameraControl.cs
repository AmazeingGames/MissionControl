using System.Runtime.CompilerServices;
using UnityEngine;
using static CameraControl;

public class CameraControl : MonoBehaviour
{
    public enum PanMode { Mouse, Keyboard, Both }
    [SerializeField] PanMode panMode = PanMode.Both;

    [SerializeField] float scrollSpeed;
    [SerializeField] float distFromEdge;

    [SerializeField] float maxLeft;
    [SerializeField] float maxRight;

    Vector3 targetPosition;

    void Start() =>
        targetPosition = transform.position;

    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        float mouseDirection = 0f;

        if (mousePosition.x >= Screen.width - distFromEdge)
            mouseDirection = 1f;
        else if (mousePosition.x <= distFromEdge)
            mouseDirection = -1f;

        var horizontalInput = Input.GetAxisRaw("Horizontal");

        float panDirection;

        switch (panMode)
        {
            case PanMode.Mouse:
                panDirection = mouseDirection;
            break;

            case PanMode.Keyboard:
                panDirection = horizontalInput;
            break;

            case PanMode.Both:

                if (horizontalInput != 0f)
                    panDirection = horizontalInput;
                else if (mouseDirection != 0f)
                    panDirection = mouseDirection;
                else
                    panDirection = 0f;
            break;

            default:
                throw new SwitchExpressionException("Camera control pan mode not handled");
        }


        targetPosition += new Vector3(panDirection * scrollSpeed * Time.deltaTime, 0f, 0f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
        targetPosition.x = Mathf.Clamp(targetPosition.x, maxLeft, maxRight);

    }
}
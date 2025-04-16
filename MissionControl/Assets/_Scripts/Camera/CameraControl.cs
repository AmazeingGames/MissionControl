using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine;
using static CameraControl;

public class CameraControl : MonoBehaviour
{
    public enum PanMode { Mouse, Keyboard, Both }
    [SerializeField] PanMode myDefaultPanMode = PanMode.Both;

    [SerializeField] float scrollSpeed;
    [SerializeField] float distFromEdge;

    [SerializeField] float maxLeft;
    [SerializeField] float maxRight;

    Vector3 targetPosition;
    PanMode myPanMode;
    void Start()
    {
        targetPosition = transform.position;
        myPanMode = myDefaultPanMode;
    }

    void Update()
    {
        myPanMode = GameStateManager.IsFocusedOnInput ? PanMode.Mouse : myDefaultPanMode;

        Vector2 mousePosition = Input.mousePosition;
        float mouseDirection = 0f;

        if (mousePosition.x >= Screen.width - distFromEdge)
            mouseDirection = 1f;
        else if (mousePosition.x <= distFromEdge)
            mouseDirection = -1f;

        var horizontalInput = Input.GetAxisRaw("Horizontal");

        float panDirection;

        switch (myPanMode)
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

        bool isAtRightEdge = transform.position.x >= maxRight;
        bool isAtLeftEdge = transform.position.x <= maxLeft;

        targetPosition += new Vector3(panDirection * scrollSpeed * Time.deltaTime, 0f);

        bool isTryingToPanRight = targetPosition.x > transform.position.x;
        bool isTryingToPanLeft = targetPosition.x < transform.position.x;

        // Fixes camera stutter when at screen edge
        if ((isTryingToPanRight && !isAtRightEdge) || (isTryingToPanLeft && !isAtLeftEdge) || (!isTryingToPanLeft && !isTryingToPanRight))
            transform.DOMoveX(targetPosition.x, .1f);

        targetPosition.x = Mathf.Clamp(targetPosition.x, maxLeft, maxRight);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, maxLeft, maxRight), transform.position.y, transform.position.z); 
    }
}
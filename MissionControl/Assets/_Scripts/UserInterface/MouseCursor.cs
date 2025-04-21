using UnityEngine;
using UnityEngine.UI;

public class MouseCursor : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] Sprite cursor;
    [SerializeField] Sprite highlightedCursor;
    [SerializeField] float zPosition;

    [Header("Components")]
    [SerializeField] GameObject mouseCursor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseCursor.transform.position = new(mousePosition.x, mousePosition.y, zPosition);
    }
}

using UnityEngine;

public class Clamper : MonoBehaviour
{
    RectTransform rectTransform;
    [SerializeField] Vector2 negativeBounds;
    [SerializeField] Vector2 bounds;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();    
    }

    // Update is called once per frame
    void Update()
    {
         rectTransform.localPosition = new Vector2(Mathf.Clamp(rectTransform.localPosition.x, negativeBounds.x, bounds.x), 
             Mathf.Clamp(rectTransform.localPosition.y, negativeBounds.y, bounds.y));
    }
}

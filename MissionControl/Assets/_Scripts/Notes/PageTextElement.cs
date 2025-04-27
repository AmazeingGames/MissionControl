using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PageTextElement : MonoBehaviour
{
    [SerializeField] bool isOnRight;

    TextMeshProUGUI text_TMP;
    bool shouldBeVisible;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text_TMP = GetComponent<TextMeshProUGUI>();

        if (!isOnRight)
            transform.localRotation = new(0, -1, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        shouldBeVisible = transform.rotation.y > -.7;

        if (text_TMP.enabled != shouldBeVisible)
        {
            Debug.Log("Set text component");
            text_TMP.enabled = shouldBeVisible;
        }
    }
}

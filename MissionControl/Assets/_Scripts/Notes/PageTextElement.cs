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
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnRight)
        {
            shouldBeVisible = transform.rotation.y > -.7;
        }
        text_TMP.enabled = shouldBeVisible;

    }
}

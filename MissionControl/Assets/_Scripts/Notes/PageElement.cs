using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageElement : MonoBehaviour
{
    TextMeshProUGUI text_TMP;
    Button button;
    Image image;
    List<Transform> children;

    bool shouldBeVisible;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text_TMP = GetComponent<TextMeshProUGUI>();
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        children = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
            children.Add(transform.GetChild(i));
    }

    // Update is called once per frame
    void Update()
    {
        shouldBeVisible = transform.rotation.y > -.7;

        SetComponent(text_TMP);
        SetComponent(button);
        SetComponent(image);

        foreach (Transform child in children)
        {
            child.gameObject.SetActive(shouldBeVisible);
        }
    }

    void SetComponent(MonoBehaviour component)
    {
        if (component != null && component.enabled != shouldBeVisible)
            component.enabled = shouldBeVisible;
    }
}

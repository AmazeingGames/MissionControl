using UnityEngine;
using UnityEngine.UI;

public class SinewaveScroll : MonoBehaviour
{
    [SerializeField] GameObject loopBackPoint;
    [SerializeField] float moveSpeed;

    [HideInInspector] public float frequency;
    [HideInInspector] public float amplitude;

    Vector3 position;
    Vector3 startPosition;
    float startingGlobalXPoisiton;


    void Start()
    {
        frequency = transform.localScale.x;
        amplitude = transform.localScale.y;
        startPosition = transform.localPosition;
        startingGlobalXPoisiton = transform.position.x;
    }

    void Update()
    {
        position = transform.localPosition;
        position.x += moveSpeed * Time.deltaTime;

        if (loopBackPoint.transform.position.x >= startingGlobalXPoisiton)
            position = startPosition;
        transform.localPosition = position;
    }
}

using UnityEngine;

public class Sinewave : MonoBehaviour
{
    Vector3 position;
    Vector3 startPosition;
    [SerializeField] float moveSpeed;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        position = transform.localPosition;
    }

    void FixedUpdate()
    {
        transform.localPosition = new Vector3(position.x + moveSpeed, startPosition.y, startPosition.z);
    }
}

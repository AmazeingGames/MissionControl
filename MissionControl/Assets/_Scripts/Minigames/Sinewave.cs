using UnityEngine;
using UnityEngine.UI;

public class Sinewave : MonoBehaviour
{
    [SerializeField] SinewaveScroll sineWaveToMatch;
    [SerializeField] Slider frequencySlider;
    [SerializeField] Slider amplitudeSlider;
    [SerializeField] GameObject loopBackPoint;
    [SerializeField] float moveSpeed;

    public float frequency;
    public float amplitude;

    Vector3 position;
    Vector3 startPosition;
    float startingGlobalXPoisiton;

    bool canBeChanged = true;

    float correctCounter;

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

        if (canBeChanged)
            CheckForMatch();
    }

    public void UpdateWave()
    {
        if (!canBeChanged)
            return;
        frequency = frequencySlider.value;
        amplitude = amplitudeSlider.value;
        transform.localScale = new Vector3(frequency, amplitude, transform.localScale.z);
    }

    void CheckForMatch()
    {
        if (CheckWaveDisatnce())
            correctCounter += Time.deltaTime;
        else if (!CheckWaveDisatnce())
            correctCounter = 0;

        if (correctCounter > 1.5f)
        {
            canBeChanged = false;
            Debug.Log("SOLVED");
            //Give access to audio files
        }
    }

    bool CheckWaveDisatnce()
    {
        float amplitudeDifference = Mathf.Abs(sineWaveToMatch.amplitude) - Mathf.Abs(amplitude);
        float frequencyDifference = Mathf.Abs(sineWaveToMatch.frequency) - Mathf.Abs(frequency);
        if (amplitudeDifference > .02f || amplitudeDifference < -.02f)
            return false;
        else if (frequencyDifference > .02f || frequencyDifference < -.02f)
            return false;
        else
            return true;
    }
}

using BNG;
using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class RadioTower : MonoBehaviour
{
    public Slider frequencySlider; // Assign your slider in inspector
    public float correctFrequency; // You can set this in inspector
    public float tolerance = 0.5f; // Tolerance within which the dial is considered correct
    private bool isDialCorrect;

    public GameObject screen;
    public TextMeshProUGUI screentext;
    public GameObject Slider;

    public GameObject[] grabbables;

    public float time;
    public float activationTime;

    public GameObject indicator;

    public Material far;
    public Material mid;
    public Material close;

    private void Start()
    {
        // Generate a random correct frequency between the slider's min and max values
        correctFrequency = Random.Range(frequencySlider.minValue, frequencySlider.maxValue);

        // Register to the slider's value change event
        frequencySlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        // Calculate the difference between the current value and the correct frequency
        float difference = Mathf.Abs(value - correctFrequency);

        if (difference > 3)
        {
            indicator.GetComponent<MeshRenderer>().material = far;
        }

        if (difference > .5f && difference <= 3) 
        {
            indicator.GetComponent<MeshRenderer>().material = mid;
        }

        // Check if the difference is within the tolerance range
        isDialCorrect = difference <= tolerance;

        // You can add more logic here to provide feedback to the player or unlock something when the dial is correct
        if (isDialCorrect)
        {
            time += Time.deltaTime;
            indicator.GetComponent<MeshRenderer>().material = close;
        }
    }

    private void OnDestroy()
    {
        // Always good practice to remove listeners when the object is destroyed
        frequencySlider.onValueChanged.RemoveAllListeners();
    }

    void Update()
    {
        if (isDialCorrect)
        {
            time += Time.deltaTime;
        }

        if (isDialCorrect && time > activationTime)
        {
            Slider.SetActive(false);
            screen.SetActive(false);
            screentext.text = "Connection Established";
            foreach (GameObject obj in grabbables)
            {
                obj.GetComponent<Grabbable>().enabled = true;
            }
        }
    }

}

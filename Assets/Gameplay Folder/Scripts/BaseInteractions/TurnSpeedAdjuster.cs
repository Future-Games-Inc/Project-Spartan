using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class TurnSpeedAdjuster : MonoBehaviour
{
    public ContinuousTurnProviderBase turnProvider;
    public SnapTurnProviderBase snapProvider;
    public Slider turnSpeedSlider;

    public GameObject snapTrue;
    public GameObject contTrue;

    public float savedTurnSpeed;
    public TextMeshProUGUI sliderValue;

    public bool snapTurn = false;
    public Button snapButton;
    public bool contTurn = true;
    public Button contButton;

    private void Start()
    {
        turnSpeedSlider.value = PlayerPrefs.GetFloat("TurnSpeed");
        turnSpeedSlider.minValue = 75f;
        turnSpeedSlider.maxValue = 500f;
        if (PlayerPrefs.HasKey("SnapTurn"))
        {
            int snapBool = PlayerPrefs.GetInt("SnapTurn", 0);
            snapTurn = snapBool != 0;
            PlayerPrefs.SetInt("SnapTurn", snapBool);
        }
        if (PlayerPrefs.HasKey("ContTurn"))
        {
            int contBool = PlayerPrefs.GetInt("ContTurn");
            contTurn = contBool != 0;
            PlayerPrefs.SetInt("ContTurn", contBool);
        }
        savedTurnSpeed = turnSpeedSlider.value;
    }

    public void OnTurnSpeedSliderValueChanged()
    {
        turnProvider.turnSpeed = turnSpeedSlider.value;
        savedTurnSpeed = turnSpeedSlider.value;
        PlayerPrefs.SetFloat("TurnSpeed", savedTurnSpeed);
    }

    private void Update()
    {
        sliderValue.text = turnSpeedSlider.value.ToString("F0");
        snapProvider.enabled = snapTurn;
        turnProvider.enabled = contTurn;

        snapTrue.SetActive(snapTurn);
        contTrue.SetActive(contTurn);
    }

    public void Snap()
    {
        snapTurn = true;
        contTurn = false;
        int snapBool = snapTurn ? 1 : 0;
        int turnBool = contTurn ? 1 : 0;
        PlayerPrefs.SetInt("SnapTurn", snapBool);
        PlayerPrefs.SetInt("ContTurn", turnBool);
    }

    public void Cont()
    {
        snapTurn = false;
        contTurn = true;
        int snapBool = snapTurn ? 1 : 0;
        int turnBool = contTurn ? 1 : 0;
        PlayerPrefs.SetInt("SnapTurn", snapBool);
        PlayerPrefs.SetInt("ContTurn", turnBool);
    }
}

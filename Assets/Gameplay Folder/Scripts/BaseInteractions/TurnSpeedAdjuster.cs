using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class TurnSpeedAdjuster : MonoBehaviour
{
    public ContinuousTurnProviderBase turnProvider;
    public SnapTurnProviderBase snapProvider;
    public XRDirectInteractor[] directInteractors;
    public XRRayInteractor[] rayInteractors;
    public Slider turnSpeedSlider;

    public GameObject snapTrue;
    public GameObject contTrue;
    public GameObject holdTrue;
    public GameObject toggleTrue;

    public float savedTurnSpeed;
    public TextMeshProUGUI sliderValue;

    public bool snapTurn = false;
    public Button snapButton;
    public bool contTurn = true;
    public Button contButton;

    public bool toggleInteract = false;
    public Button toggleButton;
    public bool stateInteract = true;
    public Button stateButton;
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
        if (PlayerPrefs.HasKey("ToggleTurn"))
        {
            int toggleBool = PlayerPrefs.GetInt("ToggleTurn", 0);
            toggleInteract = toggleBool != 0;
            PlayerPrefs.SetInt("ToggleTurn", toggleBool);
        }
        if (PlayerPrefs.HasKey("StateTurn"))
        {
            int stateBool = PlayerPrefs.GetInt("StateTurn");
            stateInteract = stateBool != 0;
            PlayerPrefs.SetInt("StateTurn", stateBool);
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
        if (toggleInteract)
        {
            foreach (XRDirectInteractor interactor in directInteractors)
            {
                interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.Toggle;
            }
            foreach (XRRayInteractor interactor in rayInteractors)
            {
                interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.Toggle;
            }
        }

        else if (stateInteract)
        {
            foreach (XRDirectInteractor interactor in directInteractors)
            {
                interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
            }
            foreach (XRRayInteractor interactor in rayInteractors)
            {
                interactor.selectActionTrigger = XRBaseControllerInteractor.InputTriggerType.State;
            }
        }

        snapTrue.SetActive(snapTurn);
        contTrue.SetActive(contTurn);
        holdTrue.SetActive(stateInteract);
        toggleTrue.SetActive(toggleInteract);
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

    public void Toggle()
    {
        toggleInteract = true;
        stateInteract = false;
        int toggleBool = toggleInteract ? 1 : 0;
        int stateBool = stateInteract ? 1 : 0;
        PlayerPrefs.SetInt("ToggleTurn", toggleBool);
        PlayerPrefs.SetInt("StateTurn", stateBool);
    }

    public void State()
    {
        toggleInteract = false;
        stateInteract = true;
        int toggleBool = toggleInteract ? 1 : 0;
        int stateBool = stateInteract ? 1 : 0;
        PlayerPrefs.SetInt("ToggleTurn", toggleBool);
        PlayerPrefs.SetInt("StateTurn", stateBool);
    }
}

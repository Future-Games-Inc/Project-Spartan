using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EarlyExtraction : MonoBehaviourPunCallbacks
{
    public InputActionProperty leftSelectButton;
    public GameObject extractionIcon;
    public TextMeshProUGUI extractionCountdown;

    private float holdTime = 0f;
    private int waitTime = 30;

    private bool isHolding = false;
    private bool hasLeftRoom = false;
    private bool activatedExtraction;

    public static readonly byte ExtractEarly = 40;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Countdown());
    }

    // Update is called once per frame
    void Update()
    {
        if (isHolding)
        {
            holdTime += Time.deltaTime;        
        }
        else
        {
            if (leftSelectButton.action.ReadValue<float>() >= .78f && activatedExtraction == false)
            {
                StartCoroutine(Extraction());
            }
        }

        if (!hasLeftRoom && holdTime >= 30f)
        {
            // Leave the room
            PhotonNetwork.LeaveRoom();
            hasLeftRoom = true;
        }
    }

    IEnumerator Extraction()
    {
        yield return new WaitForSeconds(3);
        if (leftSelectButton.action.ReadValue<float>() >= .78f && activatedExtraction == false)
        {
            isHolding = true;
            activatedExtraction = true;
            extractionIcon.SetActive(true);
            extractionCountdown.gameObject.SetActive(true);
        }
    }

    IEnumerator Countdown()
    {
        while(true)
        {
            if(isHolding)
            {
                extractionCountdown.text = waitTime--.ToString();
            }
            yield return new WaitForSeconds(1);
        }
    }
}

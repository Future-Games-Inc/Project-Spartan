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
    PlayerHealth player;

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
        player = GetComponent<PlayerHealth>();
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
                isHolding = true;
                activatedExtraction = true;
                extractionIcon.SetActive(true);
                extractionCountdown.gameObject.SetActive(true);
            }
        }

        if (!hasLeftRoom && holdTime >= 30f)
        {
            if (player.Artifact1 == true)
            {
                StartCoroutine(player.GetXP(100));
                player.Artifact1 = false;
            }
            if (player.Artifact2 == true)
            {
                StartCoroutine(player.GetXP(100));
                player.Artifact2 = false;
            }
            if (player.Artifact3 == true)
            {
                StartCoroutine(player.GetXP(100));
                player.Artifact3 = false;
            }
            if (player.Artifact4 == true)
            {
                StartCoroutine(player.GetXP(100));
                player.Artifact4 = false;
            }
            if (player.Artifact5 == true)
            {
                StartCoroutine(player.GetXP(100));
                player.Artifact5 = false;
            }
            // Leave the room
            PhotonNetwork.LeaveRoom();
            hasLeftRoom = true;
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

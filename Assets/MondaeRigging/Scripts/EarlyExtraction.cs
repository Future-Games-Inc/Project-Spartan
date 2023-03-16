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
    public PlayerHealth player;

    private float holdTime = 0f;
    private int waitTime = 30;

    private bool isHolding = false;
    private bool hasLeftRoom = false;
    private bool activatedExtraction;

    public static readonly byte ExtractEarly = 40;

    // Cache frequently accessed components
    private void Start()
    {
        StartCoroutine(Countdown());
    }

    // Update is called once per frame
    void Update()
    {
        // Use Time.unscaledDeltaTime for consistent behavior
        float deltaTime = Time.unscaledDeltaTime;

        if (isHolding)
        {
            holdTime += deltaTime;
        }
        else
        {
            if (leftSelectButton.action.ReadValue<float>() >= .78f && !activatedExtraction)
            {
                isHolding = true;
                activatedExtraction = true;
                extractionIcon.SetActive(true);
                extractionCountdown.gameObject.SetActive(true);
            }
        }

        if (!hasLeftRoom && holdTime >= 30f)
        {
            hasLeftRoom = true;

            // Use a switch statement for clarity
            for (int i = 1; i <= 5; i++)
            {
                bool artifact = (bool)player.GetType().GetProperty("Artifact" + i).GetValue(player, null);
                if (artifact)
                {
                    StartCoroutine(player.GetXP(100));
                    player.GetType().GetProperty("Artifact" + i).SetValue(player, false, null);
                }
            }

            // Use the null-conditional operator for simplicity
            if (photonView.IsMine)
                VirtualWorldManager.Instance.LeaveRoomAndLoadHomeScene();
        }
    }

    IEnumerator Countdown()
    {
        while (true)
        {
            if (isHolding)
            {
                extractionCountdown.text = waitTime--.ToString();
            }
            yield return new WaitForSecondsRealtime(1);
        }
    }
}

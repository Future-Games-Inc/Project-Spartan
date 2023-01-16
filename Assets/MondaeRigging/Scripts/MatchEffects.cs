using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using System.Threading;
using UnityEditor.XR;
using Unity.VisualScripting;

public class MatchEffects : MonoBehaviourPunCallbacks, IOnEventCallback
{

    public int matchCountdown;
    public int currentMatchTime;
    public GameObject spawnManager;
    public GameObject[] weaponCaches;
    public GameObject uiCanvas;

    public TextMeshProUGUI countdownText;

    private Coroutine timerCoroutine;

    public AudioSource audioSource;
    public AudioClip newPlayerEntered;
    public AudioClip matchBegan;
    public AudioClip countdownFive;
    public AudioClip countdownOne;
    public AudioClip countdownTwo;
    public AudioClip countdownThree;
    public AudioClip countdownFour;
    public AudioClip countdownBegan;

    public enum EventCodes : byte
    {
        RefreshTimer
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code >= 200)
            return;

        EventCodes e = (EventCodes)photonEvent.Code;
        object[] o = (object[])photonEvent.CustomData;

        switch (e)
        {
            case EventCodes.RefreshTimer:
                RefreshTimer_R(o);
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeTimer();
        audioSource.PlayOneShot(countdownBegan);
        spawnManager.SetActive(false);
        foreach (GameObject weapon in weaponCaches)
            weapon.layer = 10;
    }

    private void RefreshTimerUI()
    {
        string seconds = (currentMatchTime % 60).ToString("00");
        countdownText.text = $"{seconds}";
    }

    private void InitializeTimer()
    {
        currentMatchTime = matchCountdown;
        RefreshTimerUI();

        if (PhotonNetwork.IsMasterClient)
        {
            timerCoroutine = StartCoroutine(TimerEvent());
        }
    }

    IEnumerator TimerEvent()
    {
        yield return new WaitForSeconds(1f);

        currentMatchTime -= 1;
        
        if(currentMatchTime == 5)
        {
            audioSource.PlayOneShot(countdownFive);
        }
        if (currentMatchTime == 4)
        {
            audioSource.PlayOneShot(countdownFour);
        }
        if (currentMatchTime == 3)
        {
            audioSource.PlayOneShot(countdownThree);
        }
        if (currentMatchTime == 2)
        {
            audioSource.PlayOneShot(countdownTwo);
        }
        if (currentMatchTime == 1)
        {
            audioSource.PlayOneShot(countdownOne);
        }

        if (currentMatchTime <= 0)
        {
            audioSource.PlayOneShot(matchBegan);
            timerCoroutine = null;
            spawnManager.SetActive(true);
            foreach (GameObject weapon in weaponCaches)
                weapon.layer = 0;
            PhotonNetwork.Destroy(uiCanvas);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
        else
        {
            RefreshTimer_S();
            timerCoroutine = StartCoroutine(TimerEvent());
        }
    }

    public void RefreshTimer_S()
    {
        object[] package = new object[] { currentMatchTime };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.RefreshTimer, package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );
    }

    public void RefreshTimer_R(object[] data)
    {
        currentMatchTime = (int)data[0];
        RefreshTimerUI();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(newPlayerEntered);
        }
    }
}


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
using static UnityEngine.UI.CanvasScaler;

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

    public bool startMatchBool = false;

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
        photonView.RPC("AudioEnter", RpcTarget.All);
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
            photonView.RPC("Audio5", RpcTarget.All);
        }
        if (currentMatchTime == 4)
        {
            photonView.RPC("Audio4", RpcTarget.All);
        }
        if (currentMatchTime == 3)
        {
            photonView.RPC("Audio3", RpcTarget.All);
        }
        if (currentMatchTime == 2)
        {
            photonView.RPC("Audio2", RpcTarget.All);
        }
        if (currentMatchTime == 1)
        {
            photonView.RPC("Audio1", RpcTarget.All);
        }

        if (currentMatchTime <= 0)
        {
            photonView.RPC("AudioStart", RpcTarget.All);
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

    [PunRPC]
    void Audio5()
    {
        audioSource.PlayOneShot(countdownFive);
    }

    [PunRPC]
    void PlayAudio()
    {
        audioSource.PlayOneShot(countdownFour);
    }

    [PunRPC]
    void Audio4()
    {
        audioSource.PlayOneShot(countdownThree);
    }

    [PunRPC]
    void Audio3()
    {
        audioSource.PlayOneShot(countdownTwo);
    }

    [PunRPC]
    void Audio2()
    {
        audioSource.PlayOneShot(countdownOne);
    }

    [PunRPC]
    void AudioStart()
    {
        audioSource.PlayOneShot(matchBegan);
        timerCoroutine = null;
        spawnManager.SetActive(true);
        PhotonNetwork.Destroy(uiCanvas);
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        startMatchBool = true;
    }

    [PunRPC]
    void AudioEnter()
    {
        audioSource.PlayOneShot(countdownBegan);
        spawnManager.SetActive(false);
        startMatchBool = false;
    }
}


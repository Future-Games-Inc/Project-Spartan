using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class MatchEffects : MonoBehaviourPunCallbacks, IOnEventCallback
{

    public int matchCountdown;
    public int currentMatchTime;
    public GameObject spawnManager;
    public GameObject uiCanvas;
    public GameObject[] artifacts;
    public Transform[] artifactLocations;

    public TextMeshProUGUI countdownText;

    private Coroutine timerCoroutine;

    public AudioSource audioSource;
    public AudioClip matchBegan;
    public AudioClip countdownFive;
    public AudioClip countdownOne;
    public AudioClip countdownTwo;
    public AudioClip countdownThree;
    public AudioClip countdownFour;
    public AudioClip supplyShip1;
    public AudioClip supplyShip2;

    public GameObject supplyDropShipPrefab;
    public float spawnInterval = 330f; // 5 minutes in seconds
    public float lastSpawnTime;
    public Transform spawnLocation;

    public bool startMatchBool = false;
    public bool spawned = false;

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
        spawnInterval = 180f;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Time.time > lastSpawnTime + spawnInterval && spawned == false)
            {
                lastSpawnTime = Time.time;
                PhotonNetwork.Instantiate(supplyDropShipPrefab.name, spawnLocation.position, Quaternion.Euler(0, 90, 90), 0);
                StartCoroutine(SupplyShipAudio());
                spawned = true;
            }
        }
    }

    IEnumerator SupplyShipAudio()
    {
        yield return new WaitForSeconds(0);
        photonView.RPC("SupplyAudio1", RpcTarget.All);
        yield return new WaitForSeconds(supplyShip1.length);
        photonView.RPC("SupplyAudio2", RpcTarget.All);
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

        if (currentMatchTime == 5)
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
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            StartCoroutine(Artifacts());
        }
        else
        {
            RefreshTimer_S();
            timerCoroutine = StartCoroutine(TimerEvent());
        }
    }

    IEnumerator Artifacts()
    {
        yield return new WaitForSeconds(1f);
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < artifacts.Length; i++)
            {
                PhotonNetwork.Instantiate(artifacts[i].name, artifactLocations[i].position, Quaternion.identity);
            }
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

    [PunRPC]
    void Audio5()
    {
        audioSource.PlayOneShot(countdownFive);
    }

    [PunRPC]
    void Audio4()
    {
        audioSource.PlayOneShot(countdownFour);
    }

    [PunRPC]
    void Audio3()
    {
        audioSource.PlayOneShot(countdownThree);
    }

    [PunRPC]
    void Audio2()
    {
        audioSource.PlayOneShot(countdownTwo);
    }

    [PunRPC]
    void Audio1()
    {
        audioSource.PlayOneShot(countdownOne);
    }

    [PunRPC]
    void AudioStart()
    {
        audioSource.PlayOneShot(matchBegan);
        uiCanvas.SetActive(false);
        spawnManager.SetActive(true);
        timerCoroutine = null;
        startMatchBool = true;
    }

    [PunRPC]
    void AudioEnter()
    {
        spawnManager.SetActive(false);
        startMatchBool = false;
    }


    [PunRPC]
    void SupplyAudio1()
    {
        audioSource.PlayOneShot(supplyShip1);
    }

    [PunRPC]
    void SupplyAudio2()
    {
        audioSource.PlayOneShot(supplyShip2);
    }
}


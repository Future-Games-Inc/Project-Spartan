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
    //public GameObject[] artifacts;
    //public Transform[] artifactLocations;

    public TextMeshProUGUI countdownText;

    private Coroutine timerCoroutine;

    public AudioSource audioSource;
    public AudioClip[] countdownClips;
    public AudioClip matchBegan;
    public AudioClip supplyShip1;
    public AudioClip supplyShip2;

    public GameObject supplyDropShipPrefab;
    public float spawnInterval; // 5 minutes in seconds
    public float lastSpawnTime;
    public Transform spawnLocation;

    public bool startMatchBool = false;
    public bool spawnReactor = false;
    public bool spawned = false;
    public bool DE_supplyDrop;

    public string numSequence;

    public TextMeshProUGUI nexusCodePanel;

    public enum EventCodes : byte
    {
        RefreshTimer
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // Check if this is the object's current owner and if the new master client exists
        if (photonView.IsMine && newMasterClient != null)
        {
            // Transfer ownership of the object to the new master client
            photonView.TransferOwnership(newMasterClient.ActorNumber);
        }
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

        // Generate a random 4-digit sequence
        photonView.RPC("RPC_GenerateSequence", RpcTarget.MasterClient);
    }

    private void Update()
    {
        if (Time.time > lastSpawnTime + spawnInterval && spawned == false && DE_supplyDrop == true)
        {
            lastSpawnTime = Time.time;
            PhotonNetwork.InstantiateRoomObject(supplyDropShipPrefab.name, spawnLocation.position, Quaternion.Euler(0, 90, 90), 0, null);
            StartCoroutine(SupplyShipAudio());
            spawned = true;
        }
    }

    IEnumerator SupplyShipAudio()
    {
        yield return new WaitForSeconds(0);
        photonView.RPC("PlaySupplyDropAudio", RpcTarget.All);
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

        timerCoroutine = StartCoroutine(TimerEvent());
    }

    IEnumerator TimerEvent()
    {
        yield return new WaitForSeconds(1f);

        currentMatchTime -= 1;

        if (currentMatchTime >= 1 && currentMatchTime <= countdownClips.Length)
        {
            photonView.RPC("PlayCountdownAudio", RpcTarget.All, currentMatchTime - 1);
        }

        if (currentMatchTime <= 0)
        {
            photonView.RPC("AudioStart", RpcTarget.All);
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            //StartCoroutine(Artifacts());
            timerCoroutine = null; // Stop the coroutine
        }
        else
        {
            RefreshTimer_S();
            timerCoroutine = StartCoroutine(TimerEvent());
        }
    }

    //IEnumerator Artifacts()
    //{
    //    yield return new WaitForSeconds(1f);
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        for (int i = 0; i < artifacts.Length; i++)
    //        {
    //            PhotonNetwork.Instantiate(artifacts[i].name, artifactLocations[i].position, Quaternion.identity);
    //        }
    //    }
    //}

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
    void PlayCountdownAudio(int clipIndex)
    {
        audioSource.PlayOneShot(countdownClips[clipIndex]);
    }

    [PunRPC]
    void PlaySupplyDropAudio()
    {
        audioSource.PlayOneShot(supplyShip1);
        StartCoroutine(PlaySupplyDropAudioDelayed());
    }

    IEnumerator PlaySupplyDropAudioDelayed()
    {
        yield return new WaitForSeconds(supplyShip1.length);
        audioSource.PlayOneShot(supplyShip2);
    }

    [PunRPC]
    void AudioStart()
    {
        audioSource.PlayOneShot(matchBegan);
        uiCanvas.SetActive(false);
        spawnManager.SetActive(true);
        startMatchBool = true;
    }

    [PunRPC]
    void AudioEnter()
    {
        spawnManager.SetActive(false);
        startMatchBool = false;
    }

    [PunRPC]
    void RPC_GenerateSequence()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            numSequence = GenerateRandomSequence(4);
            photonView.RPC("RPC_ReceiveSequence", RpcTarget.All, numSequence);
        }
    }

    [PunRPC]
    void RPC_ReceiveSequence(string sequence)
    {
        numSequence = sequence;
        nexusCodePanel.text = numSequence.ToString();
    }

    private string GenerateRandomSequence(int length)
    {
        string sequence = "";
        System.Random rnd = new System.Random();

        for (int i = 0; i < length; i++)
        {
            sequence += rnd.Next(0, 10);
        }

        return sequence;
    }
}


using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;
using System;

public class WeaponCrate : MonoBehaviourPunCallbacks
{
    [Header("Cache Effects ------------------------------------------------------")]
    [SerializeField]
    private VisualEffect _visualEffect;    
    
    public Transform spawn1;
    public Transform spawn2;
    public Transform spawn3;    
    
    public AudioSource cacheAudio;
    public AudioClip cacheClip;

    public bool cacheActive;

    [Header("Cache Properties ------------------------------------------------------")]
    public string[] powerups;
    public string[] weapons;

    public Animator _animator;

    private BoxCollider _collider;

    public GameObject[] cacheBase;

    public Material activeMaterial;
    public Material deactiveMaterial;

    public MatchEffects matchProps;



    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        cacheActive = true;
    }
    void Start()
    {

    }

    private void Update()
    {

    }

    String[] ShuffleArray(String[] array)
    {
        String[] shuffledArray = (String[])array.Clone();

        for (int i = shuffledArray.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            String temp = shuffledArray[i];
            shuffledArray[i] = shuffledArray[randomIndex];
            shuffledArray[randomIndex] = temp;
        }

        return shuffledArray;
    }

    String[] ShufflePowerups(String[] array)
    {
        String[] shuffledArray = (String[])array.Clone();

        for (int i = shuffledArray.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            String temp = shuffledArray[i];
            shuffledArray[i] = shuffledArray[randomIndex];
            shuffledArray[randomIndex] = temp;
        }

        return shuffledArray;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") && cacheActive == true && matchProps.startMatchBool == true || other.CompareTag("RightHand") && cacheActive == true && matchProps.startMatchBool == true || other.CompareTag("Player") && cacheActive == true && matchProps.startMatchBool == true)
        {
            _collider.enabled = false;
            _animator.SetBool("Open", true);
            _visualEffect.SendEvent("OnPlay");
            cacheAudio.PlayOneShot(cacheClip);
            cacheActive = false;
            foreach (GameObject cache in cacheBase)
                cache.GetComponent<Renderer>().material = deactiveMaterial;
            StartCoroutine(WeaponCache());
        }
    }

    IEnumerator WeaponCache()
    {
        String[] shuffledWeapons = ShuffleArray(weapons);
        String[] shuffledPowerups = ShufflePowerups(powerups);
        yield return new WaitForSeconds(1);
        PhotonNetwork.InstantiateRoomObject(shuffledWeapons[0], spawn1.position, spawn1.rotation, 0, null);
        PhotonNetwork.InstantiateRoomObject(shuffledWeapons[2], spawn3.position, spawn3.rotation, 0, null);
        PhotonNetwork.InstantiateRoomObject(shuffledPowerups[0], spawn2.position, spawn2.rotation, 0, null);
        StartCoroutine(CacheRespawn());
        _animator.SetBool("Open", false);
    }

    IEnumerator CacheRespawn()
    {
        yield return new WaitForSeconds(30);
        _animator.SetBool("Open", false);
        cacheActive = true;
        foreach (GameObject cache in cacheBase)
            cache.GetComponent<Renderer>().material = activeMaterial;
        _collider.enabled = true;
    }

    [PunRPC]
    void RPC_Obstacle(bool state)
    {
        GetComponent<NavMeshObstacle>().enabled = state;
        GetComponent<Rigidbody>().isKinematic = !state;
    }
}

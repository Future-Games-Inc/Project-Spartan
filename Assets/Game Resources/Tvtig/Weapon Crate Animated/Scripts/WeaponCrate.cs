using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;
using Photon.Realtime;

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
        photonView.RPC("RPC_CacheStart", RpcTarget.All);
    }
    void Start()
    {

    }

    private void Update()
    {

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
            photonView.RPC("RPC_CacheOpened", RpcTarget.All);
        }
    }

    IEnumerator WeaponCache()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.InstantiateRoomObject(weapons[Random.Range(0, weapons.Length)], spawn1.position, spawn1.rotation, 0, null);
        PhotonNetwork.InstantiateRoomObject(weapons[Random.Range(0, weapons.Length)], spawn3.position, spawn3.rotation, 0, null);
        PhotonNetwork.InstantiateRoomObject(powerups[Random.Range(0, powerups.Length)], spawn2.position, spawn2.rotation, 0, null);
        StartCoroutine(CacheRespawn());
        photonView.RPC("RPC_CacheExit", RpcTarget.All);
    }

    IEnumerator CacheRespawn()
    {
        yield return new WaitForSeconds(30);
        photonView.RPC("RPC_CacheClosed", RpcTarget.All);
    }

    [PunRPC]
    void RPC_CacheStart()
    {
        cacheActive = true;
    }

    [PunRPC]
    void RPC_CacheOpened()
    {
        _collider.enabled = false;
        _animator.SetBool("Open", true);
        _visualEffect.SendEvent("OnPlay");
        cacheAudio.PlayOneShot(cacheClip);
        cacheActive = false;
        foreach(GameObject cache in cacheBase)
            cache.GetComponent<Renderer>().material = deactiveMaterial;
        StartCoroutine(WeaponCache());
    }

    [PunRPC]
    void RPC_CacheClosed()
    {
        _animator.SetBool("Open", false);
        cacheActive = true;
        foreach (GameObject cache in cacheBase)
            cache.GetComponent<Renderer>().material = activeMaterial;
        _collider.enabled = true;
    }

    [PunRPC]
    void RPC_CacheExit()
    {
        _animator.SetBool("Open", false);
    }
}

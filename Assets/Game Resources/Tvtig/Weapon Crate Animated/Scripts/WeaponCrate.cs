using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Photon.Pun;

public class WeaponCrate : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private VisualEffect _visualEffect;

    public string[] powerups;
    public string[] weapons;
    public Transform spawn1;
    public Transform spawn2;
    public Transform spawn3;

    private Animator _animator;
    private BoxCollider _collider;

    public bool cacheActive;
    public GameObject cacheBase;

    public Transform spawnPosition;

    public MatchEffects matchProps;

    public AudioSource cacheAudio;
    public AudioClip cacheClip;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider>();
        cacheAudio = GetComponent<AudioSource>();
        photonView.RPC("RPC_Awake", RpcTarget.AllBuffered);
    }
    void Start()
    {

    }

    private void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") && cacheActive == true && matchProps.startMatchBool == true || other.CompareTag("RightHand") && cacheActive == true && matchProps.startMatchBool == true || other.CompareTag("Player") && cacheActive == true && matchProps.startMatchBool == true)
        {
            photonView.RPC("RPC_Opened", RpcTarget.AllBuffered);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        photonView.RPC("RPC_Exit", RpcTarget.AllBuffered);
    }

    IEnumerator WeaponCache()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.Instantiate(weapons[Random.Range(0, weapons.Length)], spawn1.position, spawn1.rotation, 0);
        PhotonNetwork.Instantiate(weapons[Random.Range(0, weapons.Length)], spawn3.position, spawn3.rotation, 0);
        PhotonNetwork.Instantiate(powerups[Random.Range(0, powerups.Length)], spawn2.position, spawn2.rotation, 0);
        StartCoroutine(CacheRespawn());
    }

    IEnumerator CacheRespawn()
    {
        yield return new WaitForSeconds(30);
        photonView.RPC("RPC_Closed", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void RPC_Awake()
    {
        cacheActive = true;
    }

    [PunRPC]
    void RPC_Opened()
    {
        _collider.enabled = false;
        _animator.SetBool("Open", true);
        _visualEffect.SendEvent("OnPlay");
        cacheAudio.PlayOneShot(cacheClip);
        cacheActive = false;
        cacheBase.SetActive(false);
        StartCoroutine(WeaponCache());
    }

    [PunRPC]
    void RPC_Closed()
    {
        _animator.SetBool("Open", false);
        cacheActive = true;
        cacheBase.SetActive(true);
        _collider.enabled = true;
    }

    [PunRPC]
    void RPC_Exit()
    {
        _animator.SetBool("Open", false);
    }
}

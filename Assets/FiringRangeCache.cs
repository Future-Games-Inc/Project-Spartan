using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class FiringRangeCache : MonoBehaviour
{
    [SerializeField]
    private VisualEffect _visualEffect;

    public GameObject[] weapons;
    public GameObject[] enemies;
    public Transform spawn1;
    public Transform spawn2;
    public Transform spawn3;
    public Transform[] enemiesSpawn;

    private Animator _animator;
    private BoxCollider _collider;

    public bool cacheActive;
    public GameObject cacheBase;

    public Transform spawnPosition;

    public AudioSource cacheAudio;
    public AudioClip cacheClip;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider>();
        cacheAudio = GetComponent<AudioSource>();
        cacheActive = true;
    }
    void Start()
    {

    }

    private void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") && cacheActive == true || other.CompareTag("RightHand") && cacheActive == true || other.CompareTag("Player") && cacheActive == true)
        {
            _collider.enabled = false;
            _animator.SetBool("Open", true);
            _visualEffect.SendEvent("OnPlay");
            cacheAudio.PlayOneShot(cacheClip);
            cacheActive = false;
            cacheBase.SetActive(false);
            StartCoroutine(WeaponCache());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _animator.SetBool("Open", false);
    }

    IEnumerator WeaponCache()
    {
        yield return new WaitForSeconds(1);
        Instantiate(weapons[Random.Range(0, weapons.Length)], spawn1.position, spawn1.rotation);
        Instantiate(weapons[Random.Range(0, weapons.Length)], spawn3.position, spawn3.rotation);
        Instantiate(weapons[Random.Range(0, weapons.Length)], spawn2.position, spawn2.rotation);
        foreach(Transform spawn in enemiesSpawn)
        {
            Instantiate(enemies[Random.Range(0, enemies.Length)], spawn.position, spawn.rotation);
        }
        StartCoroutine(CacheRespawn());
    }

    IEnumerator CacheRespawn()
    {
        yield return new WaitForSeconds(30);
        _animator.SetBool("Open", false);
        cacheActive = true;
        cacheBase.SetActive(true);
        _collider.enabled = true;
    }
}


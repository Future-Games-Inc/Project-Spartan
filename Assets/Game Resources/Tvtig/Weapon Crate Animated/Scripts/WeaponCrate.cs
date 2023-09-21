using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.AI;

public class WeaponCrate : MonoBehaviour
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
    public GameObject[] powerups;
    public GameObject[] weapons;

    public Animator _animator;

    private BoxCollider _collider;

    public GameObject[] cacheBase;

    public Material activeMaterial;
    public Material deactiveMaterial;

    public MatchEffects matchProps;



    private void Start()
    {
        
        _collider = GetComponent<BoxCollider>();
        cacheActive = true;
    }

    private void Update()
    {
        if (!cacheActive)
        {
            foreach (GameObject cache in cacheBase)
                cache.GetComponent<Renderer>().material = deactiveMaterial;
        }
        else
        {
            foreach (GameObject cache in cacheBase)
                cache.GetComponent<Renderer>().material = activeMaterial;
        }
        _collider.enabled = cacheActive;

    }

    GameObject[] ShuffleArray(GameObject[] array)
    {
        GameObject[] shuffledArray = (GameObject[])array.Clone();

        for (int i = shuffledArray.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            GameObject temp = shuffledArray[i];
            shuffledArray[i] = shuffledArray[randomIndex];
            shuffledArray[randomIndex] = temp;
        }

        return shuffledArray;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") && cacheActive == true && matchProps.startMatchBool == true || other.CompareTag("RightHand") && cacheActive == true && matchProps.startMatchBool == true || other.CompareTag("Player") && cacheActive == true && matchProps.startMatchBool == true)
        {
            _animator.SetBool("Open", true);
            _visualEffect.SendEvent("OnPlay");
            cacheAudio.PlayOneShot(cacheClip);
            cacheActive = false;
            StartCoroutine(WeaponCache());
        }
    }

    IEnumerator WeaponCache()
    {
        yield return new WaitForSeconds(1.5f);
        GameObject[] shuffledWeapons = ShuffleArray(weapons);
        GameObject[] shuffledPowerups = ShuffleArray(powerups);
        Instantiate(shuffledWeapons[0], spawn1.position, spawn1.rotation);
        Instantiate(shuffledWeapons[2], spawn3.position, spawn3.rotation);
        Instantiate(shuffledPowerups[0], spawn2.position, spawn2.rotation);
        yield return new WaitForSeconds(1);
        _animator.SetBool("Open", false);
        StartCoroutine(CacheRespawn());
    }

    IEnumerator CacheRespawn()
    {
        yield return new WaitForSeconds(30);
        _animator.SetBool("Open", false);
        cacheActive = true;
    }

    public void Obstacle(bool state)
    {
        GetComponent<NavMeshObstacle>().enabled = state;
        GetComponent<Rigidbody>().isKinematic = !state;
    }
}

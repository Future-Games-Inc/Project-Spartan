using CSCore;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SupplyDropCrate : MonoBehaviourPunCallbacks
{
    public GameObject[] weaponPrefabs;
    public Slider activationSlider;
    public GameObject[] effects;
    public TextMeshProUGUI openText;
    public MatchEffects matchProps;
    public Image sliderImage;
    public AudioSource audioSource;
    public AudioClip audioClip;

    private float elapsedTime;
    public float activationTime = 15;
    public float radius = 4;

    private bool isActive;
    private bool contact = false;
    public bool playAudio = true;

    private void OnEnable()
    {
        StartCoroutine(PlayAudioLoop());
        activationSlider.maxValue = activationTime;
        activationSlider.value = activationTime;
        activationSlider.gameObject.SetActive(true);
        StartCoroutine(NoContact());
    }
    IEnumerator PlayAudioLoop()
    {
        while (playAudio)
        {
            if (CheckForPlayerWithinRadius() && !isActive)
            {
                audioSource.PlayOneShot(audioClip);
                yield return new WaitForSeconds(audioClip.length);
            }
            yield return new WaitForSeconds(.75f);
        }
    }

    void Update()
    {
        if (CheckForPlayerWithinRadius() == true)
        {
            photonView.RPC("RPC_Update", RpcTarget.All);
        }
        else
        {
            photonView.RPC("RPC_Update2", RpcTarget.All);
        }
        photonView.RPC("RPC_Color", RpcTarget.All);
    }

    bool CheckForPlayerWithinRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    void InstantiateWeapons()
    {
        photonView.RPC("RPC_InstantiateWeapons", RpcTarget.All);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand") || other.CompareTag("Player") && isActive == true)
        {
            photonView.RPC("RPC_Trigger", RpcTarget.All);
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(.75f);
        matchProps.lastSpawnTime = Time.time;
        matchProps.spawned = false;
        PhotonNetwork.Destroy(gameObject);
    }

    IEnumerator NoContact()
    {
        yield return new WaitForSeconds(30);
        if (contact == false)
        {
            matchProps = GameObject.FindGameObjectWithTag("Props").GetComponent<MatchEffects>();
            matchProps.lastSpawnTime = Time.time;
            matchProps.spawned = false;
            yield return new WaitForSeconds(2);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    void RPC_InstantiateWeapons()
    {
        int count = Random.Range(1, weaponPrefabs.Length + 1);
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = transform.position + Random.onUnitSphere * radius;
            GameObject weaponPrefab = weaponPrefabs[Random.Range(0, weaponPrefabs.Length)];
            PhotonNetwork.Instantiate(weaponPrefab.name, randomPosition, Quaternion.identity, 0);
            Rigidbody rb = weaponPrefab.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    [PunRPC]
    void RPC_Trigger()
    {
        InstantiateWeapons();
        StartCoroutine(Destroy());
        contact = true;
    }

    [PunRPC]
    void RPC_Update()
    {
        if (!isActive)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = activationTime - elapsedTime;
            activationSlider.value = remainingTime;
            if (elapsedTime >= activationTime)
            {
                isActive = true;
                activationSlider.gameObject.SetActive(false);
                openText.gameObject.SetActive(true);
            }

            foreach (GameObject vfx in effects)
            {
                vfx.SetActive(false);
            }
        }
    }

    [PunRPC]
    void RPC_Update2()
    {
        foreach (GameObject vfx in effects)
        {
            vfx.SetActive(true);
        }
    }

    [PunRPC]
    void RPC_Color()
    {
        if (activationSlider.value <= (activationTime * 0.75) && activationSlider.value > (activationTime * 0.25))
            sliderImage.color = Color.yellow;
        if (activationSlider.value <= (activationTime * 0.25))
            sliderImage.color = Color.red;
    }
}
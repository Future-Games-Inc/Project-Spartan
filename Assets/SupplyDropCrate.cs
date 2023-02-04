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

    private float elapsedTime;
    public float activationTime = 15;
    public float radius = 2;

    private bool isActive;
    private bool contact = false;


    private void Start()
    {
        activationSlider.maxValue = activationTime;
        activationSlider.value = activationTime;
        activationSlider.gameObject.SetActive(true);
        StartCoroutine(NoContact());
    }
    void Update()
    {
        if (CheckForPlayerWithinRadius())
        {
            photonView.RPC("RPC_Update", RpcTarget.All);
        }
        else if (!CheckForPlayerWithinRadius())
        {
            photonView.RPC("RPC_Update2", RpcTarget.All);
        }
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
            activationSlider.value -= elapsedTime;
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
}
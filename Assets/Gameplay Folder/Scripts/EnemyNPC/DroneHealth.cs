using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DroneHealth : MonoBehaviour
{
    public int Health = 100;
    public GameObject xpDrop;
    public GameObject xpDropExtra;
    public SpawnManager1 enemyCounter;
    public bool alive = true;
    public Transform[] lootSpawn;
    public float xpDropRate = 10f;

    public AudioSource audioSource;
    public AudioClip bulletHit;
    public AudioClip[] audioClip;
    public GameObject explosionEffect;
    public NavMeshAgent agent;
    public bool hit;

    public string type;

    void OnEnable()
    {
        enemyCounter = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
        InvokeRepeating("RandomSFX", 15, 20f);
        explosionEffect.SetActive(false);
        alive = true;
    }

    void Update()
    {
        if (agent != null && !agent.isOnNavMesh)
        {
            TakeDamage(300);
        }
    }

    public void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(bulletHit);
        Health -= damage;
        if (!hit)
            StartCoroutine(Hit());

        if (Health <= 0 && alive)
        {
            alive = false;
            TriggerDeathEffects();
        }
    }

    IEnumerator Hit()
    {
        hit = true;
        explosionEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        explosionEffect.SetActive(false);
        hit = false;
    }

    private void TriggerDeathEffects()
    {
        explosionEffect.SetActive(true);
        if (agent != null)
            agent.enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        SpawnLoot();
        enemyCounter.UpdateSecurity();
        Destroy(gameObject);
    }

    private void SpawnLoot()
    {
        foreach (Transform t in lootSpawn)
        {
            GameObject drop = Random.Range(0, 100f) < xpDropRate ? xpDropExtra : xpDrop;
            GameObject loot = Instantiate(drop, t.position, Quaternion.identity);
            loot.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void RandomSFX()
    {
        if (alive && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)]);
        }
    }
}

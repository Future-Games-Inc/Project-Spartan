using UnityEngine;
using Photon.Pun;

public class EnemyXPDrop : MonoBehaviourPunCallbacks
{
    public SpawnManager1 spawnManager;
    // Start is called before the first frame update
    void OnEnable()
    {
        spawnManager = GameObject.FindGameObjectWithTag("spawnManager").GetComponent<SpawnManager1>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.CompareTag("XP"))
        {
            if (other.CompareTag("Player"))
            {
                float xpDrop = 10f;

                //cal it at random probability
                if (Random.Range(0, 100f) < xpDrop)
                {
                    other.gameObject.GetComponentInParent<PlayerHealth>().UpdateSkills(10);
                }
                else
                {
                    other.gameObject.GetComponentInParent<PlayerHealth>().UpdateSkills(5);
                }
                PhotonNetwork.Destroy(gameObject);
            }           
        }

        else if (this.CompareTag("Health"))
        {
            if (other.CompareTag("Player"))
            {
                spawnManager.photonView.RPC("RPC_UpdateHealthCount", RpcTarget.All);
                other.GetComponentInParent<PlayerHealth>().AddHealth(10);
                PhotonNetwork.Destroy(gameObject);
            }           
        }

        else if (this.CompareTag("MinorHealth"))
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponentInParent<PlayerHealth>().AddHealth(5);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if(this.CompareTag("ExtraXP"))
        {
            if (other.CompareTag("Player"))
            {
                float xpDrop = 10f;

                //cal it at random probability
                if (Random.Range(0, 100f) < xpDrop)
                {
                    other.gameObject.GetComponentInParent<PlayerHealth>().UpdateSkills(100);
                }
                else
                {
                    other.gameObject.GetComponentInParent<PlayerHealth>().UpdateSkills(50);
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (this.CompareTag("toxicDropNormal"))
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponentInParent<PlayerHealth>().toxicEffectActive = true;
                other.GetComponentInParent<PlayerHealth>().Toxicity(10);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (this.CompareTag("toxicDropExtra"))
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponentInParent<PlayerHealth>().toxicEffectActive = true;
                other.GetComponentInParent<PlayerHealth>().Toxicity(20);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (this.CompareTag("bulletModifierNormal"))
        {
            if (other.CompareTag("Player"))
            {                
                other.GetComponentInParent<PlayerHealth>().BulletImprove(10,2);
                other.GetComponentInParent<PlayerHealth>().bulletImproved = true;
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (this.CompareTag("bulletModifierExtra"))
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponentInParent<PlayerHealth>().BulletImprove(15, 4);
                other.GetComponentInParent<PlayerHealth>().bulletImproved = true;
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else if (this.CompareTag("MPShield"))
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponentInParent<PlayerHealth>().shieldActive = true;
                other.GetComponentInParent<PlayerHealth>().Shield(10);
                PhotonNetwork.Destroy(gameObject);
            }
        }

        else
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}


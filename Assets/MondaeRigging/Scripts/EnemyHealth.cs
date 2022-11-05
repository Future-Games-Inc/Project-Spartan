using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyHealth : MonoBehaviour
{
    public FollowAI aiScript;
    public float Health;
    public GameObject xpDrop;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (aiScript.Health <= 0)
        {
            Instantiate(xpDrop, transform.position, Quaternion.identity);
            Invoke(nameof(DestroyEnemy), 0.5f);
        }

    }

    public void TakeDamage(int damage)
    {


    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}

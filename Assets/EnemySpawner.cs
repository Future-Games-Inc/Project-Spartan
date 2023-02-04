using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public GameObject[] enemy;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn", 10, 30);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Spawn()
    {
        foreach(Transform location in spawnPoint)
        {
            Instantiate(enemy[Random.Range(0, enemy.Length)], location.position, location.rotation);
        }
    }
}

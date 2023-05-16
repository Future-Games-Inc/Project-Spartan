using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringRangeRespawn : MonoBehaviour
{
    public Transform respawn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.position = respawn.transform.position;
        }
    }
}

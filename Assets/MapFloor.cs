using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFloor : MonoBehaviour
{
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
            PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
            health.TakeDamage(1000);
        }
    }
}

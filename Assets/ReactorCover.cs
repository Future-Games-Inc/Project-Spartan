using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorCover : MonoBehaviour
{
    public int Health;
    public MatchEffects matchEffects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            matchEffects.spawnReactor = true;
        }
    }
}

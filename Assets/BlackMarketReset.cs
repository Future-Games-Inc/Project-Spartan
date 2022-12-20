using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackMarketReset : MonoBehaviour
{
    public GameObject[] abilities;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject ability in abilities) 
        { 
            ability.GetComponent<Skill>().skillLevel= 0;
            ability.GetComponent<Skill>().UpdateLoadout();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

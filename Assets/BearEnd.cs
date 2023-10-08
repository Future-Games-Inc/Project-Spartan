using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearEnd : MonoBehaviour
{
    public MatchEffects matchEffects;
    public PlayerHealth player;
    // Start is called before the first frame update
    void OnEnable()
    {
        matchEffects = GameObject.FindGameObjectWithTag("Props").GetComponent<MatchEffects>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Artifact5"))
        {
            other.gameObject.SetActive(false);
            matchEffects.MissionEnd();
            matchEffects.AddTime(60);
            player.UpdateSkills(2700);
            player.GetXP(120);
            matchEffects.MissionStart.SetActive(false);
        }
    }
}

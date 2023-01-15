using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerDeathToken : MonoBehaviour
{
    public int tokenValue;
    public string faction;

    public bool tokenActivated = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TokenActivation());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tokenActivated == true)
        {
            other.gameObject.GetComponent<PlayerHealth>().UpdateSkills(tokenValue);
            if (faction != other.gameObject.GetComponent<PlayerHealth>().characterFaction)
            {
                other.GetComponent<PlayerHealth>().FactionDataCard(faction);
            }
        }
    }

    private IEnumerator TokenActivation() 
    {
        yield return new WaitForSeconds(1.5f);
        tokenActivated = true;
    }
}

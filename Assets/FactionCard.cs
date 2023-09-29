using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionCard : MonoBehaviour
{
    public string faction;
    public GameObject[] factionScreens;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(AssignFaction());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AssignFaction()
    {
        yield return new WaitForSeconds(0.25f);
        foreach (GameObject obj in factionScreens)
        {
            obj.SetActive(false);
        }

        // Activate the corresponding GameObject based on the value of owner
        if (faction == "Cyber SK Gang") factionScreens[0].SetActive(true);
        else if (faction == "Muerte De Dios") factionScreens[1].SetActive(true);
        else if (faction == "Chaos Cartel") factionScreens[2].SetActive(true);
        else if (faction == "CintSix Cartel") factionScreens[3].SetActive(true);
    }

    public void rescale()
    {
        this.gameObject.transform.localScale = Vector3.one;
    }
}

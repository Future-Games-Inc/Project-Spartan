using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTrigger : MonoBehaviour
{

    public GameObject MainMenu;
    public GameObject touchParticles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            MainMenu.SetActive(true);
            touchParticles.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        MainMenu.SetActive(false);
        touchParticles.SetActive(false);
    }
}

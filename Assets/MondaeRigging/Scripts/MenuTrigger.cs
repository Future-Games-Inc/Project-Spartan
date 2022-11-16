using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTrigger : MonoBehaviour
{

    public GameObject MainMenu;
    public GameObject touchParticles;

    public bool activated;

    // Start is called before the first frame update
    void Start()
    {
        activated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated ==true)
        {
            MainMenu.SetActive(true);
            touchParticles.SetActive(true);
        }
        else
        {
            MainMenu.SetActive(false);
            touchParticles.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
        {
            activated = true;
        }
    }

    public void Disable()
    {
        activated = false;
    }
}


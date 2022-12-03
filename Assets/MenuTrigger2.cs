using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTrigger2 : MonoBehaviour
{

    public GameObject MainMenu;
    public GameObject bannerCanvas;

    public bool activated;

    // Start is called before the first frame update
    void Start()
    {
        activated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated == true)
        {
            MainMenu.SetActive(true);
            bannerCanvas.SetActive(false);
        }
        else
        {
            MainMenu.SetActive(false);
            bannerCanvas.SetActive(true);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activated = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        activated = false;
    }
}
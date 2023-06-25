using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTrigger3 : MonoBehaviour
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
        if (MainMenu != null)
            MainMenu.SetActive(activated);
        bannerCanvas.SetActive(!activated);
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
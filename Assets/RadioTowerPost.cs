using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RadioTowerPost : MonoBehaviour
{
    public XRSocketInteractor socket;
    public TextMeshProUGUI displayText;
    public bool Connected;

    // Start is called before the first frame update
    void Start()
    {
        displayText.fontSize = 24;
        displayText.text = "Disconneected";
    }

    // Update is called once per frame
    void Update()
    {
        if(socket.hasSelection) 
        {
            displayText.text = "Connected";
            Connected = true;
        }
    }
}

using System.Collections;
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
        StartCoroutine(CheckTower());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CheckTower()
    {
        while (true)
        {
            while (!socket.hasSelection)
            {
                yield return null;
            }
            displayText.text = "Connected";
            Connected = true;
        }
    }
}

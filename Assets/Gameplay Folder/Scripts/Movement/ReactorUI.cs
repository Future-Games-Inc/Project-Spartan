using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReactorUI : MonoBehaviour
{
    public TextMeshProUGUI reactorText;
    public PlayerHealth playerHealth;

    private void Start()
    {
        reactorText.text = "Reactor Extraction: " + playerHealth.reactorExtraction + "%";
    }
    private void Update()
    {
        
    }

    public void UpdateReactorUI()
    {
        reactorText.text = "Reactor Extraction: " + playerHealth.reactorExtraction + "%";
    }
}
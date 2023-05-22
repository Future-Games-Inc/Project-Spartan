using System.Collections;
using TMPro;
using UnityEngine;

public class DisplayText : MonoBehaviour
{

    public TextMeshProUGUI displayText;
    public string text = "";
    public MatchEffects matchEffects;
    public GameObject[] keyButtons;
    public GameObject granted;
    public GameObject denied;

    // Start is called before the first frame update
   void Start()
    {
        keyButtons = GameObject.FindGameObjectsWithTag("KeypadButton");
    }
    
    public void SetText(string txt)
    {
        text = txt;
        UpdateDisplayText();
    }

    public void AddCharacter(string character)
    {
        text += character;
        UpdateDisplayText();
    }

    private void UpdateDisplayText()
    {
        displayText.text = (text.Length > 4) ? text.Substring(text.Length - 4) : text;
    }

    public void Clear()
    {
        text = "";
        UpdateDisplayText ();
    }

    public void Submit()
    {
        if(displayText.text == matchEffects.numSequence)
        {
            displayText.fontSize = 24;
            displayText.text = "Access Granted";
            matchEffects.spawnReactor = true;
            foreach(GameObject keys in keyButtons)
                keys.SetActive(false);
            granted.SetActive(true);
            StartCoroutine(Deactivate());
        }
        else
        {
            displayText.fontSize = 24;
            displayText.text = "Access Denied";
            foreach (GameObject keys in keyButtons)
                keys.SetActive(false);
            denied.SetActive(true);
            StartCoroutine(Deactivate());
        }
    }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(3);
        text = "";
        UpdateDisplayText();
        displayText.fontSize = 36;
        denied.SetActive(false);
        granted.SetActive(false);
        foreach (GameObject keys in keyButtons)
            keys.SetActive(true);
    }
}

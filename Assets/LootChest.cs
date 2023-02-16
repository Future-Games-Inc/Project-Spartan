using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.GameFoundation;
using UnityEngine.UI;

public class LootChest : MonoBehaviour
{
    public RawImage crateIcon;
    public TextMeshProUGUI rewardText;
    public GameObject rewardTextHolder;
    public SaveData saveData;
   
    public int[] rewardValues = { 1000, 250, 350, 450, 550, 650, 750, 850, 950 };
    public bool rewardGiven;

    // Start is called before the first frame update
    void Start()
    {
        rewardTextHolder.SetActive(false);
        rewardText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && rewardGiven == false && crateIcon.color == Color.white)
        {
            rewardGiven = true;
            crateIcon.color = Color.cyan;

            int randomIndex = Random.Range(0, rewardValues.Length);
            int rewardValue = rewardValues[randomIndex];

            rewardTextHolder.SetActive(true);
            rewardText.text = "Great Work " + PlayerPrefs.GetString("PlayerName") + ". Weekly Reward Claimed: " + rewardValue.ToString() + " Cints";
            saveData.UpdateSkills(rewardValue);
            StartCoroutine(Deactivate());
        }
    }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(10);
        {
            rewardTextHolder.SetActive(false);
            rewardText.text = "";
            rewardGiven = false;
        }
    }
}

using LootLocker.Requests;
using TMPro;
using UnityEngine;

public class LeaderCheck : MonoBehaviour
{
    public TextMeshProUGUI sceneName;
    public string owner;
    public GameObject[] gameObjects; // assuming you have 4 gameObjects corresponding to 4 owner strings
    public TextMeshProUGUI extractionTime;

    void OnEnable()
    {
        LootLockerSDKManager.GetMemberRank(sceneName.text.ToString(), sceneName.text.ToString(), (response) =>
        {
            if (response.success)
            {
                owner = response.metadata;
                extractionTime.text = "Extraction Time: " + response.score + " secs";
                ActivateCorrespondingGameObject();
            }
        });
    }

    void ActivateCorrespondingGameObject()
    {
        // Deactivate all GameObjects first
        foreach (var obj in gameObjects)
        {
            obj.SetActive(false);
        }

        // Activate the corresponding GameObject based on the value of owner
        if (owner == "Cyber SK Gang") gameObjects[0].SetActive(true);
        else if (owner == "Muerte De Dios") gameObjects[1].SetActive(true);
        else if (owner == "Chaos Cartel") gameObjects[2].SetActive(true);
        else if (owner == "CintSix Cartel") gameObjects[3].SetActive(true);
    }
}

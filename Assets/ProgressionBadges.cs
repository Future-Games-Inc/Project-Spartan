using System.Collections;
using UnityEngine;
using TMPro;

public class ProgressionBadges : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI prestigetext;
    public int levelIncrement = 10;
    public GameObject[] badges;
    public SaveData saveData;

    public GameObject awardButton;

    // Start is called before the first frame update
    void Start()
    {
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
    }

    // Update is called once per frame
    void Update()
    {
        awardButton.SetActive(saveData.awarded);
    }

    public IEnumerator UpdateBadges()
    {
        yield return new WaitForSeconds(2.75f);
        int playerLevel;

        if (int.TryParse(levelText.text, out playerLevel))
        {
            // Calculate the number of badges to activate
            int badgeCount = (playerLevel / levelIncrement) + 1;

            // Ensure the badge count is within the bounds of the badge array
            badgeCount = Mathf.Clamp(badgeCount, 0, badges.Length);

            // Activate the appropriate number of badges
            for (int i = 0; i < badgeCount; i++)
            {
                badges[i].SetActive(true);
            }
        }

        prestigetext.text = ((playerLevel / levelIncrement) + 1).ToString();
    }
}

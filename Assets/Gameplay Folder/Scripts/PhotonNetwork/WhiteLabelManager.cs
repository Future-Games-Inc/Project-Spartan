using UnityEngine;
using TMPro;
using LootLocker.Requests;

public class WhiteLabelManager : MonoBehaviour
{
    public TopReactsLeaderboard topReactsLeaderboard;
    public static string playerID;

    public TMP_InputField _inputField;

    public GameObject inputFieldText;

    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";

    private void OnEnable()
    {

    }

    // Start is called before the first frame update
    public void Start()
    {
        AutoLogin();
    }

    public void AutoLogin()
    {
        LootLockerSDKManager.CheckWhiteLabelSession(response =>
        {
            if (response == false)
            {
            }
            else
            {
                // Session is valid, start game session
                LootLockerSDKManager.StartWhiteLabelSession((response) =>
                                {
                                    if (response.success)
                                    {
                                        // It was succeful, log in
                                        playerID = response.player_id.ToString();
                                        string defaultName = string.Empty;
                                        if (_inputField != null)
                                        {
                                            LootLockerSDKManager.GetPlayerName((response) =>
                                            {
                                                if (response.success)
                                                {
                                                    defaultName = response.name.ToString();
                                                    _inputField.text = defaultName.ToString();
                                                }
                                            });
                                        }
                                        else
                                        {
                                            defaultName = "Unknown REACT: " + (int)UnityEngine.Random.Range(100, 350);
                                        }
                                        PlayerPrefs.SetString(playerNamePrefKey, defaultName);
                                        playerID = response.player_id.ToString();
                                        StartCoroutine(topReactsLeaderboard.FetchTopHighScores());
                                        inputFieldText.SetActive(true);
                                    }
                                    else
                                    {

                                        return;
                                    }

                                });

            }

        });
    }
}

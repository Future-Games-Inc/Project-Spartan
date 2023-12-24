using UnityEngine;
using TMPro;
using LootLocker.Requests;
using System.Collections;
using LootLocker;

public class WhiteLabelManager : MonoBehaviour
{
    public TopReactsLeaderboard topReactsLeaderboard;
    public static string playerID;

    public TMP_InputField _inputField;

    public GameObject inputFieldText;

    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }

    // Start is called before the first frame update
    public void Start()
    {
        AutoLogin();
    }

    public void AutoLogin()
    {
        StartCoroutine(AutoLoginCoroutine());
    }

    private IEnumerator AutoLoginCoroutine()
    {
        bool loginSuccessful = false;

        while (!loginSuccessful)
        {
            LootLockerSDKManager.StartGuestSession((response) =>
            {
                if (response.success)
                {
                    // Login successful
                    loginSuccessful = true;
                    playerID = response.player_id.ToString();
                    string defaultName = HandlePlayerName(response);
                    PlayerPrefs.SetString(playerNamePrefKey, defaultName);
                    playerID = response.player_id.ToString();
                    StartCoroutine(topReactsLeaderboard.FetchTopHighScores());
                    inputFieldText.SetActive(true);
                    Debug.Log("Login successful");
                }
                else
                {
                    Debug.LogWarning("Login failed, retrying...");
                }
            });

            // Wait for a short delay before retrying to avoid spamming requests
            yield return new WaitForSeconds(2);
        }
    }

    private string HandlePlayerName(LootLockerResponse response)
    {
        string defaultName = string.Empty;
        if (_inputField != null)
        {
            LootLockerSDKManager.GetPlayerName((response) =>
            {
                if (response.success)
                {
                    defaultName = response.name;
                    _inputField.text = defaultName;
                }
            });
        }
        else
        {
            defaultName = "Unknown REACT: " + (int)UnityEngine.Random.Range(100, 350);
        }
        return defaultName;
    }
}

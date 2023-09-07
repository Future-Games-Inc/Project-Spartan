using LootLocker.Requests;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(InputField))]
public class NicknameSave : MonoBehaviour

{
    public TMP_InputField _inputField;

    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void OnEnable()
    {
        _inputField.text = PlayerPrefs.GetString(playerNamePrefKey);
    }

    /// <summary>
    /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
    /// </summary>
    /// <param name="value">The name of the Player</param>
    public void SetPlayerName(string value)
    {
        // #Important
        if (string.IsNullOrEmpty(value))
        {
            return;
        }
        LootLockerSDKManager.SetPlayerName(value, (response) =>
        {
        });
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
}

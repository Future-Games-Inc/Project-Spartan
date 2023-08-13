using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using LootLocker.Requests;
using Photon.Pun;
using JetBrains.Annotations;

public class WhiteLabelManager : MonoBehaviour
{
    // Input fields
    [Header("New User")]
    public TMP_InputField newUserEmailInputField;
    public TMP_InputField newUserPasswordInputField;

    [Header("Existing User")]
    public TMP_InputField existingUserEmailInputField;
    public TMP_InputField existingUserPasswordInputField;

    [Header("Reset password")]
    public TMP_InputField resetPasswordInputField;

    [Header("RememberMe")]
    // Components for enabling auto login
    public Toggle rememberMeToggle;
    private int rememberMe;

    public TopReactsLeaderboard topReactsLeaderboard;
    public string playerID;
    public GameObject inputField;
    public GameObject returningScreens;

    public TMP_InputField _inputField;

    public GameObject returning;
    public GameObject inputFieldText;
    public GameObject[] keyboards;

    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";

    // Called when pressing "LOGIN" on the login-page
    public void Login()
    {
        string email = existingUserEmailInputField.text;
        string password = existingUserPasswordInputField.text;
        LootLockerSDKManager.WhiteLabelLogin(email, password, Convert.ToBoolean(rememberMe), response =>
        {
            if (!response.success)
            {
                // Error
                return;
            }

            LootLockerSDKManager.StartWhiteLabelSession((response) =>
            {
                if (!response.success)
                {
                    // Error
                }
                else
                {
                    // Session was succesfully started;
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
                    PhotonNetwork.NickName = defaultName;
                    PlayerPrefs.SetString(playerNamePrefKey, defaultName);
                    playerID = response.player_id.ToString();
                    StartCoroutine(topReactsLeaderboard.FetchTopHighScores());
                }
            });
        });
    }

    // Called when pressing "CREATE" on new user screen
    public void NewUser()
    {
        string email = newUserEmailInputField.text;
        string password = newUserPasswordInputField.text;

        LootLockerSDKManager.WhiteLabelSignUp(email, password, (response) =>
        {
            if (!response.success)
            {
                return;
            }
            else
            {
            }
        });
    }

    // Start is called before the first frame update
    public void Start()
    {
        // See if we should log in the player automatically
        rememberMe = PlayerPrefs.GetInt("rememberMe", 0);
        if (rememberMe == 0)
        {
            rememberMeToggle.isOn = false;
        }
        else
        {
            rememberMeToggle.isOn = true;
        }
    }

    // Called when changing the value on the toggle
    public void ToggleRememberMe()
    {
        bool rememberMeBool = rememberMeToggle.isOn;
        rememberMe = Convert.ToInt32(rememberMeBool);
        PlayerPrefs.SetInt("rememberMe", rememberMe);
    }

    public void AutoLogin()
    {
        // Does the user want to automatically log in?
        if (Convert.ToBoolean(rememberMe) == true)
        {
            // Hide the buttons on the login screen

            LootLockerSDKManager.CheckWhiteLabelSession(response =>
            {
                if (response == false)
                {
                    // Session was not valid, show error animation
                    // and show back button

                    // set the remember me bool to false here, so that the next time the player press login
                    // they will get to the login screen
                    rememberMeToggle.isOn = false;
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
                            PhotonNetwork.NickName = defaultName;
                            PlayerPrefs.SetString(playerNamePrefKey, defaultName);
                            foreach (GameObject keys in keyboards)
                                keys.SetActive(false);
                            playerID = response.player_id.ToString();
                            StartCoroutine(topReactsLeaderboard.FetchTopHighScores());
                            returning.SetActive(false);
                            inputFieldText.SetActive(true);
                        }
                        else
                        {
                            // Error
                            // set the remember me bool to false here, so that the next time the player press login
                            // they will get to the login screen
                            rememberMeToggle.isOn = false;

                            return;
                        }

                    });

                }

            });
        }
        else if (Convert.ToBoolean(rememberMe) == false)
        {

        }
    }

    public void PasswordReset()
    {
        string email = resetPasswordInputField.text;
        LootLockerSDKManager.WhiteLabelRequestPassword(email, (response) =>
        {
            if (!response.success)
            {
                return;
            }
        });
    }

    public void ResendVerificationEmail()
    {
        int playerID = 0;
        LootLockerSDKManager.WhiteLabelRequestVerification(playerID, (response) =>
        {
            if (response.success)
            {
                // Email was sent!
            }
        });
    }

    public void LogOut()
    {
        LootLockerSDKManager.EndSession((response) =>
        { 
            if(response.success)
            {
                PlayerPrefs.SetInt("rememberMe", 0);
            }
        });
    }
}

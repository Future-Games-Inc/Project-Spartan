using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using LootLocker.Requests;

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
                Debug.Log("error while logging in");
                return;
            }
            else
            {
                Debug.Log("Player was logged in succesfully");
            }

            LootLockerSDKManager.StartWhiteLabelSession((response) =>
            {
                if (!response.success)
                {
                    // Error

                    Debug.Log("error starting LootLocker session");
                    return;
                }
                else
                {
                    // Session was succesfully started;
                    Debug.Log("session started successfully");
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
                Debug.Log("Error signing up:" + response.Error);
                return;
            }
            else
            {
                // Succesful response
                Debug.Log("Account created");
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

                        }
                        else
                        {
                            // Error

                            Debug.Log("error starting LootLocker session");
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
                Debug.Log("error requesting password reset");
                return;
            }

            Debug.Log("requested password reset successfully");
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_InputField = null;
    [SerializeField] private GameObject continueButton = null;

    private const string PlayerPrefsNameKey = "PlayerName";

    // Start is called before the first frame update
    void Start() => SetUpInputField();

    // Update is called once per frame
    void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey))
        {
            return;
        }

        string defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);

        m_InputField.text = defaultName;

        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            continueButton.SetActive(true);
        }
    }

    public void SavePlayerName()
    {
        string playerName = m_InputField.text;
        PhotonNetwork.NickName = playerName;
        PlayerPrefs.SetString(PlayerPrefsNameKey, playerName);
    }
}

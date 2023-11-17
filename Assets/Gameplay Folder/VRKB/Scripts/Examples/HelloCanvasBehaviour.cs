/// \file
/// <summary>
/// Canvas that displays a greeting message, which is used in
/// Oculus/SteamVR example scenes.
/// </summary>

using TMPro;
using UnityEngine;
using Photon.Pun;

public class HelloCanvasBehaviour : MonoBehaviour
{

    private const string PlayerPrefsNameKey = "PlayerName";

    public void SetName(string name)
    {
        if (name == null || name.Length == 0)
            name = "<blank>";

        TextMeshProUGUI textField = GetComponentInChildren<TextMeshProUGUI>();

        textField.text = string.Format("Hi {0}", name + "! Touch the Connect button and wait a few seconds. Then touch the Adventure button. Wait 15 seconds");


        PlayerPrefs.SetString(PlayerPrefsNameKey, name);

        PhotonNetwork.NickName = name;
       
    }

    
}

using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CuaFiles : MonoBehaviour
{
    public Button textFileButton;
    public GameObject textfile;
    public GameObject textView;
    public GameObject textFileFolder;
    public AudioSource audioSource;

    public string textFileKey;

    public void Start()
    {
        bool unlocked = PlayerHasUnlockedTextFile();

        if (textFileButton != null)
        {
            textFileButton.interactable = unlocked;
        }
    }

    private bool PlayerHasUnlockedTextFile()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(textFileKey, out object textState))
        {
            // Check the custom property value for the audio file key
            bool unlocked = (bool)textState;
            return unlocked;
        }

        return false;
    }

    public void ReadTextFile()
    {
        if (PlayerHasUnlockedTextFile())
        {
            // Play the audio file
            audioSource.Stop();
            textView.SetActive(true);
            int childCount = textFileFolder.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = textFileFolder.transform.GetChild(i);
                child.gameObject.SetActive(false);
            }
            textfile.SetActive(true);
        }
    }
}
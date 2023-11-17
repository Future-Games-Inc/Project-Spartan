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
    public RectTransform rectTransform;

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
        if (PlayerPrefs.HasKey(textFileKey))
        {
            // Check the custom property value for the audio file key
            return true;
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
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}
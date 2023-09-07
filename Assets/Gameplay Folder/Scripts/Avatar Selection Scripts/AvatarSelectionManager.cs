using UnityEngine;

public class AvatarSelectionManager : MonoBehaviour
{
    [SerializeField]
    GameObject AvatarSelectionPlatformGameobject;

  
    public GameObject[] selectableAvatarModels;
    public GameObject[] loadableAvatarModels;

    public int avatarSelectionNumber = 0;

    //public AvatarInputConverter avatarInputConverter;


    /// <summary>
    /// Singleton Implementation
    /// </summary>
    public static AvatarSelectionManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ////Initially, de-activating the Avatar Selection Platform.
        //AvatarSelectionPlatformGameobject.SetActive(false);
        // To Retrieve
        if (PlayerPrefs.HasKey("AvatarSelectionNumber"))
        {
            avatarSelectionNumber = PlayerPrefs.GetInt("AvatarSelectionNumber");
            ActivateAvatarModelAt(avatarSelectionNumber);
            LoadAvatarModelAt(avatarSelectionNumber);
        }
        else
        {
            avatarSelectionNumber = 0;
            ActivateAvatarModelAt(avatarSelectionNumber);
            LoadAvatarModelAt(avatarSelectionNumber);
        }


    }

    public void ActivateAvatarSelectionPlatform()
    {
        AvatarSelectionPlatformGameobject.SetActive(true);
    }

    public void DeactivateAvatarSelectionPlatform()
    {
        AvatarSelectionPlatformGameobject.SetActive(false);

    }

    public void NextAvatar()
    {
        avatarSelectionNumber += 1;
        if (avatarSelectionNumber >= selectableAvatarModels.Length)
        {
            avatarSelectionNumber = 0;
        }
        ActivateAvatarModelAt(avatarSelectionNumber);
    }

    public void PreviousAvatar()
    {
        avatarSelectionNumber -= 1;

        if (avatarSelectionNumber < 0)
        {
            avatarSelectionNumber = selectableAvatarModels.Length - 1;
        }
        ActivateAvatarModelAt(avatarSelectionNumber);
    }

    /// <summary>
    /// Activates the selected Avatar model inside the Avatar Selection Platform
    /// </summary>
    /// <param name="avatarIndex"></param>
    private void ActivateAvatarModelAt(int avatarIndex)
    {
        foreach (GameObject selectableAvatarModel in selectableAvatarModels)
        {
            selectableAvatarModel.SetActive(false);
        }

        selectableAvatarModels[avatarIndex].SetActive(true);
        
        LoadAvatarModelAt(avatarSelectionNumber);
    }

    /// <summary>
    /// Loads the Avatar Model and integrates into the VR Player Controller gameobject
    /// </summary>
    /// <param name="avatarIndex"></param>
    private void LoadAvatarModelAt(int avatarIndex)
    {
        foreach (GameObject loadableAvatarModel in loadableAvatarModels)
        {
            loadableAvatarModel.SetActive(false);
        }

        loadableAvatarModels[avatarIndex].SetActive(true);
        loadableAvatarModels[avatarIndex].GetComponent<Animator>().enabled = true;

        PlayerPrefs.SetInt("AvatarSelectionNumber", avatarSelectionNumber);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VirtualWorldManager : MonoBehaviour
{
    public static VirtualWorldManager Instance;
    public float extractionTime = 300;
    public PlayerHealth player; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerHealth>();
    }

    public void LeaveRoomAndLoadHomeScene()
    {
        // Leave the room and load the new scene
        SceneManager.LoadScene("TD Main Menu");
    }

    public void TimesUP()
    {
        player.UpdateSkills(-50);
        SceneManager.LoadScene("TD Main Menu");
    }

}

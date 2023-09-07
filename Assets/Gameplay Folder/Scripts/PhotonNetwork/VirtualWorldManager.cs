using UnityEngine;
using UnityEngine.SceneManagement;

public class VirtualWorldManager : MonoBehaviour
{
    public static VirtualWorldManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void LeaveRoomAndLoadHomeScene()
    {
        // Leave the room and load the new scene
        SceneManager.LoadScene("TD Main Menu");
    }

}

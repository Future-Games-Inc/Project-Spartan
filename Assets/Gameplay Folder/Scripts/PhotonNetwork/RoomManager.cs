using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void OnEnterButtonClicked_Playground()
    {
        SceneManager.LoadScene("Playground");
    }

    public void OnEnterButtonClicked_Bear()
    {
        SceneManager.LoadScene("Bear");
    }

    public void OnEnterButtonClicked_Tower()
    {
        SceneManager.LoadScene("Tower");
    }

    public void OnEnterButtonClicked_DropZone1()
    {
        SceneManager.LoadScene("DZ1");
    }
    public void OnEnterButtonClicked_DropZone2()
    {
        SceneManager.LoadScene("DZ2");
    }

    public void OnEnterButtonClicked_DropZone3()
    {
        SceneManager.LoadScene("DZ3");
    }

    public void OnEnterButtonClicked_WeaponTest()
    {
        SceneManager.LoadScene("WeaponTest");
    }
}
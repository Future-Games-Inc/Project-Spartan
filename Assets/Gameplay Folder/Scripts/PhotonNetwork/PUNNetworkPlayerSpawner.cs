using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PUNNetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject spawnPlayerPrefab;

    private void Start()
    {

    }

    // Start is called before the first frame update
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined");
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(1))
        {
            spawnPlayerPrefab = PhotonNetwork.Instantiate("Scavenger", transform.position, transform.rotation);
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2))
        {
            spawnPlayerPrefab = PhotonNetwork.Instantiate("Explorer", transform.position, transform.rotation);
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(3))
        {
            spawnPlayerPrefab = PhotonNetwork.Instantiate("Outcast", transform.position, transform.rotation);
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(4))
        {
            spawnPlayerPrefab = PhotonNetwork.Instantiate("Soldier", transform.position, transform.rotation);
        }
    }

    // Update is called once per frame
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnPlayerPrefab);
    }
}

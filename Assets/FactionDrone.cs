using System.Collections;
using System.Collections.Generic;
using Umbrace.Unity.PurePool;
using UnityEngine;

public class FactionDrone : MonoBehaviour
{
    public GameObject dronePrefab; // assign your drone prefab here in inspector
    public Transform[] waypoints; // assign your waypoints here in inspector
    public float droneTimer;

    public GameObjectPoolManager PoolManager;

    private void OnEnable()
    {
        // Find the manager if one hasn't been specified.
        if (this.PoolManager == null)
        {
            this.PoolManager = Object.FindObjectOfType<GameObjectPoolManager>();
        }
    }


    void Start()
    {
        StartCoroutine(SpawnDrones());
    }

    IEnumerator SpawnDrones()
    {
        while (true)
        {
            yield return new WaitForSeconds(droneTimer);
            GameObject drone = this.PoolManager.Acquire(dronePrefab, transform.position + new Vector3(0, 20, 0), Quaternion.identity);
            drone.GetComponent<Drone>().SetWaypoints(waypoints);
            yield return new WaitForSeconds(20); // wait 5 seconds before spawning next drone
        }
    }
}

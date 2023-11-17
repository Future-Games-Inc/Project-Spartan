using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionDrone : MonoBehaviour
{
    public GameObject dronePrefab; // assign your drone prefab here in inspector
    public Transform[] waypoints; // assign your waypoints here in inspector
    public float droneTimer;

    void Start()
    {
        StartCoroutine(SpawnDrones());
    }

    IEnumerator SpawnDrones()
    {
        while (true)
        {
            yield return new WaitForSeconds(droneTimer);
            GameObject drone = Instantiate(dronePrefab, transform.position + new Vector3(0, 20, 0), Quaternion.identity);
            drone.GetComponent<Drone>().SetWaypoints(waypoints);
            yield return new WaitForSeconds(20); // wait 5 seconds before spawning next drone
        }
    }
}

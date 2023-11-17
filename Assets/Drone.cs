using System.Collections;
using UnityEngine;

public class Drone : MonoBehaviour
{
    private Transform[] waypoints;
    private int currentIndex = 0;
    public float speed = 5f;
    public float rotationSpeed = 2f; // You can adjust this value as needed in the inspector.
    public string[] faction;
    public string chosenFaction;
    public GameObject[] flags;
    public string playerFaction;

    private void OnEnable()
    {
        playerFaction = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerHealth>().faction;
        int index;
        do
        {
            index = Random.Range(0, faction.Length);
            chosenFaction = faction[index];
        } while (chosenFaction == playerFaction); // Keep choosing until the chosen faction is different from the player faction

        foreach (GameObject obj in flags)
        {
            obj.SetActive(false);
        }

        // Activate the corresponding GameObject based on the value of chosenFaction
        if (chosenFaction == "Cyber SK Gang") flags[0].SetActive(true);
        else if (chosenFaction == "Muerte De Dios") flags[1].SetActive(true);
        else if (chosenFaction == "Chaos Cartel") flags[2].SetActive(true);
        else if (chosenFaction == "CintSix Cartel") flags[3].SetActive(true);
    }
    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = ShuffleArray(newWaypoints); // Shuffle waypoints
        StartCoroutine(Destroy());
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        Vector3 targetPosition = target.position + new Vector3(0, 20, 0); // Keep drone 20m above the waypoint

        // Rotate to face the target only around Y axis
        Vector3 directionToTarget = new Vector3(targetPosition.x - transform.position.x, 0, targetPosition.z - transform.position.z);
        if (directionToTarget.sqrMagnitude > 0.01f) // Avoid LookRotation viewing vector zero warning
        {
            Quaternion toRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(30);
        Destroy(gameObject);
    }

    // Shuffle the array
    Transform[] ShuffleArray(Transform[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i);

            Transform temp = array[i];
            array[i] = array[rnd];
            array[rnd] = temp;
        }
        return array;
    }
}

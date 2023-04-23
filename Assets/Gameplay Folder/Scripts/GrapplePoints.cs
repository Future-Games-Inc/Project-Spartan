using UnityEngine;

public class GrapplePoints : MonoBehaviour
{
    private string tagToSearchFor = "GrapplePoint";
    public GameObject[] taggedObjects;

    void Start()
    {
        taggedObjects = GameObject.FindGameObjectsWithTag(tagToSearchFor);
        foreach (GameObject go in taggedObjects)
        {
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (!rb)
            {
                rb = go.AddComponent<Rigidbody>();
            }
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.useGravity= false;
        }
    }
}

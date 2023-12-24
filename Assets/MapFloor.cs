using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFloor : MonoBehaviour
{
    private bool entered;
    private bool activatedFloor;

    // Start is called before the first frame update
    void Start()
    {
        entered = false;
        activatedFloor = false;
        StartCoroutine(Activated());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!entered && activatedFloor)
        {
            entered = true;
            GameObject obj = other.gameObject;
            while (obj != null)
            {
                if (obj.GetComponent<PlayerHealth>() != null)
                {
                    PlayerHealth health = obj.GetComponent<PlayerHealth>();
                    health.TakeDamage(1000);
                    entered = false;
                }
                obj = obj.transform.parent?.gameObject;
            }
        }
    }

    IEnumerator Activated()
    {
        yield return new WaitForSeconds(5);
        activatedFloor = true;
    }
}

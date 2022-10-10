using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{

    public static Transform[] points;

    private void Awake()
    {
        points = new Transform[transform.childCount];
        for (int i = 0; i < points.Length; i++)
        {

            points[i] = transform.GetChild(i);

        }
    }

    private void OnDrawGizmos()
    {
        foreach(Transform t in transform)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(t.position, 1f);
        }

        Gizmos.color = Color.red;
        for(int i = 0; i < transform.childCount -1; i++)
        {

            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);

        }


    }



}

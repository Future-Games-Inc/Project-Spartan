using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLight : MonoBehaviour
{
    [SerializeField] private Transform rotatingObjectTransform;

    [SerializeField] private float speed = 1.0f;

    void Update()
    {
        rotatingObjectTransform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}

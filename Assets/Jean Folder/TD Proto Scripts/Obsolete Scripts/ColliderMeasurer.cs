using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class ColliderMeasurer : MonoBehaviour
{
    public static ColliderMeasurer Instance = null;

    private MeshFilter meshFilter = null;


    private void Awake()
    {
        Instance = this;

        meshFilter = GetComponent<MeshFilter>();
    }


    // Start is called before the first frame update
    private void Start()
    {
        transform.rotation = Quaternion.identity;
    }

    public Vector3 Measure(Mesh mesh)
    {
        // set the mesh
        meshFilter.sharedMesh = mesh;

        //Get the size of the mesh and factor in the object size
        Vector3 size = meshFilter.sharedMesh.bounds.size;

        // clearh the mesh
        meshFilter.sharedMesh = null;

        return size;

    }


    private void OnValidate()
    {
        // Always mae sure the rotate is zero'd out
        if(transform.rotation != Quaternion.identity)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}

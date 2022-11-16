using ProceduralWorlds.SceneOptimizer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    public MinimapSettings settings;
    public float cameraHeight;
    // Start is called before the first frame update

    private void Awake()
    {
        settings = GetComponentInParent<MinimapSettings>();
        cameraHeight = 20f;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = settings.targetToFollow.transform.position;

        transform.position = new Vector3(targetPosition.x, targetPosition.y + cameraHeight, targetPosition.z);

        if(settings.rotateWithTarget)
        {
            Quaternion targetRotation = settings.targetToFollow.transform.rotation;

            transform.rotation = Quaternion.Euler(90, targetRotation.eulerAngles.y, 0);
        }
    }
}

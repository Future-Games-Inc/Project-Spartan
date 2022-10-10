using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReactorCore : MonoBehaviour
{

    AISensor reactorAISensor;
    MeshRenderer[] Nodes = new MeshRenderer[3];
    Nodo[] nodo = new Nodo[3];
    XRSimpleInteractable[] xRSimpleInteractables = new XRSimpleInteractable[3];

    public Material ActivatedNodeMesh;
    public Material DeactivatedNodeMesh;
    public int listSize;

    float scanInterval;
    float scanTimer;
    public float scanFrequency = 1f;

    // Start is called before the first frame update
    void Start()
    {
        reactorAISensor = GetComponentInChildren<AISensor>();
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;

        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            LineOfSight();
        }

    }

    public void LineOfSight()
    {
       if(reactorAISensor.Objects.Count > 0)
        {
            //listSize = reactorAISensor.Objects.Count;
            //for(int i = 0; i <= reactorAISensor.Objects.Count; i++)
            //{
            //    Nodes[i] = reactorAISensor.Objects[i].GetComponent<MeshRenderer>();
            //    nodo[i] = reactorAISensor.Objects[i].GetComponent<Nodo>();
            //    xRSimpleInteractables[i] = reactorAISensor.Objects[i].GetComponent<XRSimpleInteractable>();
            //}

            Nodes[0] = reactorAISensor.Objects[0].GetComponent<MeshRenderer>();
            Nodes[1] = reactorAISensor.Objects[1].GetComponent<MeshRenderer>();
            Nodes[2] = reactorAISensor.Objects[2].GetComponent<MeshRenderer>();

            nodo[0] = reactorAISensor.Objects[0].GetComponent<Nodo>();
            nodo[1] = reactorAISensor.Objects[1].GetComponent<Nodo>();
            nodo[2] = reactorAISensor.Objects[2].GetComponent<Nodo>();

            xRSimpleInteractables[0] = reactorAISensor.Objects[0].GetComponent<XRSimpleInteractable>();
            xRSimpleInteractables[1] = reactorAISensor.Objects[1].GetComponent<XRSimpleInteractable>();
            xRSimpleInteractables[2] = reactorAISensor.Objects[2].GetComponent<XRSimpleInteractable>();

        }
        //else
        //{
        //    for(int i = 0; i == listSize; i ++)
        //    {
        //        Nodes[i] = null;
        //        nodo[i] = null;
        //        xRSimpleInteractables[i] = null;
        //    }
        //}


    }

    public void ActivateNodes()
    {
        //for (int i = 1; i == listSize; i++)
        //{
        //    Nodes[i].material = ActivatedNodeMesh;
        //    nodo[i].enabled = true;
        //    xRSimpleInteractables[i].enabled = true;
        //}

        Nodes[0].material = ActivatedNodeMesh;
        Nodes[1].material = ActivatedNodeMesh;
        Nodes[2].material = ActivatedNodeMesh;

        nodo[0].enabled = true;
        nodo[1].enabled = true;
        nodo[2].enabled = true;
        xRSimpleInteractables[0].enabled = true;
        xRSimpleInteractables[1].enabled = true;
        xRSimpleInteractables[2].enabled = true;

    }

    public void DeactivateNodes()
    {
        //for(int i = 1; i == listSize; i++)
        //{
        //    Nodes[i].material = DeactivatedNodeMesh;
        //    nodo[i].enabled = false;
        //    xRSimpleInteractables[i].enabled = false;
        //}
        Nodes[0].material = DeactivatedNodeMesh;
        Nodes[1].material = DeactivatedNodeMesh;
        Nodes[2].material = DeactivatedNodeMesh;

        nodo[0].enabled = false;
        nodo[1].enabled = false;
        nodo[2].enabled = false;
        xRSimpleInteractables[0].enabled = false;
        xRSimpleInteractables[1].enabled = false;
        xRSimpleInteractables[2].enabled = false;
    }


}

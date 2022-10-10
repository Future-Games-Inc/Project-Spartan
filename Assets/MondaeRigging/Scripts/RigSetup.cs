using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigSetup : MonoBehaviour
{

    public GameObject m_xrCamera;
    public GameObject m_lefthand;
    public GameObject m_rightHand;

    // Start is called before the first frame update
    void Start()
    {
        m_xrCamera.GetComponent<GameObject>();
        m_lefthand.GetComponent<GameObject>();
        m_rightHand.GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

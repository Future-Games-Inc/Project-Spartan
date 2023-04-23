using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LoginUIManager : MonoBehaviour
{

    public GameObject connectionOptionPanel;
    public GameObject connectNamePanel;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        connectionOptionPanel.SetActive(true);
        connectNamePanel.SetActive(false);
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }
}

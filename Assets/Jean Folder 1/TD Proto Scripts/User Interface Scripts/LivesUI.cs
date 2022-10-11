using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    public TMP_Text livesText;


    //TODO: Seperate into a coroutine this is not effecient coded wise. 
    void Update()
    {
        livesText.text = PlayerStats.Lives.ToString() + " LIVES";
    }
}

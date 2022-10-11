using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundSurvived_TD : MonoBehaviour
{
    public TMP_Text roundsText;

    private void OnEnable()
    {
        roundsText.text = ("Rounds completed: ") + PlayerStats.Rounds.ToString();
    }

    


}

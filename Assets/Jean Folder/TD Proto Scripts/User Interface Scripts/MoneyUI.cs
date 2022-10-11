using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public TMP_Text moneyText;


    private void Update()
    {
        moneyText.text = PlayerStats.Money.ToString();
    }

}

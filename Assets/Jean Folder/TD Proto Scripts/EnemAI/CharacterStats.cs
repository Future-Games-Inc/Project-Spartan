using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class CharacterStats : MonoBehaviour
    {
    [Header("Health Stats")]
    [Tooltip("MaxHealth will equal to health * 10," +
        " Also dont change the max and current health values just adjust health level")]
        public int healthLevel = 10;
        public int maxHealth;
        public int currentHealth;
    [Header("Stamina or Magic ")]
        public int staminaLevel = 10;
        public int maxStamina;
        public int currentStamina;


    }




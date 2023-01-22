using UnityEngine;
using UnityEngine.UI;

public class Supercharge : MonoBehaviour
{
    public int targetKills = 5;
    public float timePeriod = 60f;
    public float superchargeDuration = 30f;
    public Slider killSlider;
    public Slider timeRemainingSlider;

    public int currentKills;
    private float timePeriodTimer;
    private float superchargeTimer;

    public PlayerHealth player;

    public void Start()
    {
        timeRemainingSlider.enabled = false;
    }

    private void Update()
    {
        if (timePeriodTimer > 0)
        {
            killSlider.enabled = true;
            timeRemainingSlider.enabled = false;
            timePeriodTimer -= Time.deltaTime;
            killSlider.value = (float)currentKills / targetKills;

            if (currentKills >= targetKills)
            {
                superchargeTimer = superchargeDuration;
                timePeriodTimer = 0;
                currentKills = 0;
            }
        }
        else if (superchargeTimer > 0)
        {
            killSlider.enabled = false;
            timeRemainingSlider.enabled = true;
            superchargeTimer -= Time.deltaTime;
            player.bulletXPModifier = 10;
            timeRemainingSlider.value = superchargeTimer / superchargeDuration;
        }
        else
        {
            killSlider.enabled = true;
            timeRemainingSlider.enabled = false;
            timePeriodTimer = timePeriod;
            currentKills = 0;
        }
    }

    public void IncreaseKillCount()
    {
        currentKills++;
    }
}

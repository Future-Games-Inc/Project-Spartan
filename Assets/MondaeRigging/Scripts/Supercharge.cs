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
    public float timePeriodTimer;
    public float superchargeTimer;

    public PlayerHealth player;

    public void Start()
    {
        timeRemainingSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (timePeriodTimer > 0)
        {
            killSlider.gameObject.SetActive(true);
            timeRemainingSlider.gameObject.SetActive(false);
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
            killSlider.gameObject.SetActive(false);
            timeRemainingSlider.gameObject.SetActive(true);
            superchargeTimer -= Time.deltaTime;
            player.BulletImprove(superchargeTimer, (player.bulletModifier + 10));
            timeRemainingSlider.value = superchargeTimer / superchargeDuration;
        }
        else
        {
            killSlider.gameObject.SetActive(true);
            timeRemainingSlider.gameObject.SetActive(false);
            timePeriodTimer = timePeriod;
            currentKills = 0;
        }
    }

    public void IncreaseKillCount()
    {
        currentKills++;
    }
}

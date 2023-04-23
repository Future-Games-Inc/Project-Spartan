using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Supercharge : MonoBehaviour
{
    public int targetKills = 5;
    public float timePeriod = 60f;
    public float superchargeDuration = 30f;
    public Slider progressSlider;
    public Image sliderImage;

    private int currentKills;
    private float superchargeTimer;
    private PlayerHealth player;

    private void Start()
    {
        player = GetComponent<PlayerHealth>();
        progressSlider.value = 0;
        StartCoroutine(TimePeriodCoroutine());
    }

    private IEnumerator TimePeriodCoroutine()
    {
        while (true)
        {
            while (currentKills < targetKills)
            {
                yield return null;
            }

            superchargeTimer = superchargeDuration;
            currentKills = 0;
            progressSlider.value = 1;
            sliderImage.color = Color.yellow;

            yield return new WaitForSeconds(superchargeDuration);

            superchargeTimer = 0;
            progressSlider.value = 0;

            yield return new WaitForSeconds(timePeriod - superchargeDuration);
        }
    }

    private void Update()
    {
        if (superchargeTimer > 0)
        {
            player.BulletImprove(superchargeTimer, (player.bulletModifier + 10));
            progressSlider.value = superchargeTimer / superchargeDuration;
            sliderImage.color = Color.red;
            superchargeTimer -= Time.deltaTime;
        }
    }

    public void IncreaseKillCount()
    {
        currentKills++;
        progressSlider.value = (float)currentKills / targetKills;
    }
}

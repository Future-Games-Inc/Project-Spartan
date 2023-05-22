using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{

    public Image img;
    public AnimationCurve curve;
    public int playerDeaths;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeTo(string scene)
    {
        StartCoroutine(FadeOut(scene));
    }
    public void ScreenFadeToBlack()
    {
        StartCoroutine(FadeBlack());
    }

    IEnumerator FadeBlack()
    {
        yield return new WaitForSeconds(2);
        float t = 0f;

        while (t <= 1f)
        {
            t += Time.deltaTime;

            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);

            yield return 0; // wait a frame and then continue...
        }
    }

    public IEnumerator FadeIn()
    {
        float t = 2f;

        while (t > 0f)
        {
            t -= Time.deltaTime;

            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);

            yield return 0; // wait a frame and then continue...
        }
    }

    public IEnumerator Respawn()
    {
        if (playerDeaths <= 2)
        {
            float t = 2f;

            while (t > 0f)
            {
                t -= Time.deltaTime;

                float a = curve.Evaluate(t);
                img.color = new Color(106f, 0f, 0f, a);
                playerDeaths++;

                yield return 0; // wait a frame and then continue...
            }
        }
        else if (playerDeaths > 2 && playerDeaths <= 6)
        {
            float t = 2f;

            while (t > 0f)
            {
                t -= Time.deltaTime;

                float a = curve.Evaluate(t);
                img.color = new Color(106f, 0f, 0f, a);
                playerDeaths++;

                yield return 5; // wait a frame and then continue...
            }
        }
        else if (playerDeaths > 6 && playerDeaths <= 10)
        {
            float t = 2f;

            while (t > 0f)
            {
                t -= Time.deltaTime;

                float a = curve.Evaluate(t);
                img.color = new Color(106f, 0f, 0f, a);
                playerDeaths++;

                yield return 10; // wait a frame and then continue...
            }
        }
        else if (playerDeaths > 10)
        {
            float t = 2f;

            while (t > 0f)
            {
                t -= Time.deltaTime;

                float a = curve.Evaluate(t);
                img.color = new Color(106f, 0f, 0f, a);
                playerDeaths++;

                yield return 15; // wait a frame and then continue...
            }
        }
    }

    public IEnumerator BlackOut(float duration)
    {
        float t = duration;

        while (t > 0f)
        {
            t -= Time.deltaTime;

            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);

            yield return 0; // wait a frame and then continue...
        }
    }

    public IEnumerator FadeOut(string scene)
    {
        float t = 2f;

        while (t < 0f)
        {
            t += Time.deltaTime;

            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);

            yield return 0; // wait a frame and then continue...
        }
        SceneManager.LoadScene(scene);
    }

    public IEnumerator ScreenFade()
    {
        float t = 2f;

        while (t < 0f)
        {
            t += Time.deltaTime;

            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);

            yield return 0;
        }
    }

    public IEnumerator ScreenFadeIn()
    {
        float t = 2f;

        while (t > 0f)
        {
            t -= Time.deltaTime;

            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);

            yield return 0; // wait a frame and then continue...
        }
    }
}

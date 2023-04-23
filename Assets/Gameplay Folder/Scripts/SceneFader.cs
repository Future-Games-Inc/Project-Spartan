using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{

    public Image img;
    public AnimationCurve curve;

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
        float t = 2f;

        while (t > 0f)
        {
            t -= Time.deltaTime;

            float a = curve.Evaluate(t);
            img.color = new Color(106f, 0f, 0f, a);

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

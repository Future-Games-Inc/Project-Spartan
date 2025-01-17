using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Cinematic : MonoBehaviour
{
    public GameObject startEffect;
    public GameObject startEffect2;
    public GameObject character;
    public GameObject reactor;
    public GameObject authority;
    public GameObject players;
    public SceneFader fader;

    public AudioSource voiceoverSource;
    public AudioClip voiceOver;
    public AudioClip transmission;
    public AudioClip teleport;

    private float holdTime = 0f;

    private bool isHolding = false;
    private bool hasLeftRoom = false;
    private bool activatedExtraction;

    public InputActionProperty leftmenu;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CinematicStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (isHolding)
        {
            holdTime += Time.deltaTime;
        }
        else
        {
            if (leftmenu.action.ReadValue<float>() >= .78f && activatedExtraction == false)
            {
                isHolding = true;
                activatedExtraction = true;
                voiceoverSource.Stop();
            }
        }

        if (!hasLeftRoom && holdTime >= .75f)
        {
            // Leave the room
            fader.FadeTo("DemoStory");
            hasLeftRoom = true;
        }
    }

    IEnumerator CinematicStart()
    {
        yield return new WaitForSeconds(3);
        voiceoverSource.PlayOneShot(transmission);
        yield return new WaitForSeconds(2);
        voiceoverSource.PlayOneShot(teleport);
        startEffect.SetActive(true);
        yield return new WaitForSeconds(2);
        startEffect.SetActive(false);
        voiceoverSource.PlayOneShot(voiceOver);
        character.SetActive(true);
        yield return new WaitForSeconds(8);
        reactor.SetActive(true);
        yield return new WaitForSeconds(10);
        reactor.SetActive(false);
        yield return new WaitForSeconds(11);
        authority.SetActive(true);
        yield return new WaitForSeconds(8);
        authority.SetActive(false);
        yield return new WaitForSeconds(2);
        players.SetActive(true);
        yield return new WaitForSeconds(12);
        players.SetActive(false);
        yield return new WaitForSeconds(6);
        reactor.SetActive(true);
        yield return new WaitForSeconds(8);
        reactor.SetActive(false);
        yield return new WaitForSeconds(7);
        startEffect2.SetActive(true);
        voiceoverSource.PlayOneShot(teleport);
        yield return new WaitForSeconds(2f);
        character.SetActive(false);
        yield return new WaitForSeconds(.75f);
        SceneManager.LoadScene(7);
    }
}

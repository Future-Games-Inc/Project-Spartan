using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVoiceover : MonoBehaviour
{
    public AudioClip[] CyberSK;
    public AudioClip[] Chaos;
    public AudioClip[] Muerte;
    public AudioClip[] CintSix;

    public AudioSource audioSource;

    public GameObject[] leaderImages;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator VoiceOvers(string faction, int Index)
    {
        yield return new WaitForSeconds(1f);
        if (faction == "Cyber SK Gang" && !audioSource.isPlaying)
        {
            leaderImages[0].SetActive(true);

            audioSource.PlayOneShot(CyberSK[Index]);
            yield return new WaitForSeconds(CyberSK[Index].length + .75f);
            leaderImages[0].SetActive(false);
        }

        else if (faction == "Chaos Cartel" && !audioSource.isPlaying)
        {
            leaderImages[1].SetActive(true);

            audioSource.PlayOneShot(Chaos[Index]);
            yield return new WaitForSeconds(Chaos[Index].length + .75f);
            leaderImages[1].SetActive(false);
        }

        else if (faction == "Muerte De Dios" && !audioSource.isPlaying)
        {
            leaderImages[2].SetActive(true);

            audioSource.PlayOneShot(Muerte[Index]);
            yield return new WaitForSeconds(Muerte[Index].length + .75f);
            leaderImages[2].SetActive(false);
        }

        else if (faction == "CintSix Cartel" && !audioSource.isPlaying)
        {
            leaderImages[3].SetActive(true);

            audioSource.PlayOneShot(CintSix[Index]);
            yield return new WaitForSeconds(CintSix[Index].length + .75f);
            leaderImages[3].SetActive(false);
        }
    }
}

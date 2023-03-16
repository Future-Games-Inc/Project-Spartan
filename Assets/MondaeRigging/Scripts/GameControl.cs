using System.Collections;
using TMPro;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI Timer;
    public GameObject highScoreBanner;
    public GameObject scoreBanner;
    public EnemySpawner spawner;

    private int score;
    private int highScore;
    private float countDown = 10f;
    private float gameDuration = 60f;
    private float timeLeft;
    private bool gameStarted = false;

    public GameObject[] enemies1;
    public GameObject[] minigameEnemies;
    public Transform[] spawnPoint;

    public AudioSource audioSource;
    public AudioClip startClip;
    public AudioClip endClip;
    public AudioClip countdown5;
    public AudioClip countdown4;
    public AudioClip countdown3;
    public AudioClip countdown2;
    public AudioClip countdown1;
    public AudioClip matchStart;

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore.ToString();
        highScoreBanner.SetActive(false);
        scoreBanner.SetActive(false);
    }

    void Update()
    {
        if (!gameStarted)
            return;

        if (countDown > 0)
        {
            countDown -= Time.deltaTime;
            Timer.text = countDown.ToString("F0");
            if (countDown <= 0)
            {
                timeLeft = gameDuration;
                SpawnEnemies(10);
                audioSource.PlayOneShot(matchStart);
            }
        }

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            Timer.text = timeLeft.ToString("F0");
            if (timeLeft <= 0)
            {
                EndGame();
            }
        }
    }

    void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // spawn enemy code here
            Instantiate(minigameEnemies[Random.Range(0, minigameEnemies.Length)], spawnPoint[Random.Range(0, spawnPoint.Length)].position, Quaternion.identity);
        }
    }

    void EndGame()
    {
        gameStarted = false;

        // show score canvas
        scoreText.text = "Score: " + score.ToString();
        enemies1 = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies1)
            Destroy(enemy);
        spawner.enabled = false;

        // check for new high score
        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = "High Score: " + highScore.ToString();
            PlayerPrefs.SetInt("HighScore", highScore);
            highScoreBanner.SetActive(true);
        }

        audioSource.PlayOneShot(endClip);

        StartCoroutine(Deactivate());
    }

    public void StartGame()
    {
        if (gameStarted == false)
        {
            StartCoroutine(CountDown());
            audioSource.PlayOneShot(startClip);
            // reset score and countdowns
            score = 0;
            countDown = 10f;
            timeLeft = 0f;

            scoreBanner.SetActive(true);
            // update score display
            scoreText.text = "Score: 0";

            // show high score display
            highScoreText.text = "High Score: " + highScore.ToString();

            // hide high score banner
            highScoreBanner.SetActive(false);

            // start game
            spawner.enabled = false;
            enemies1 = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies1)
                Destroy(enemy);
            gameStarted = true;
        }
        else
            return;
    }

    public void EnemyKilled()
    {
        score++;
        SpawnEnemies(1);
        scoreText.text = "Score: " + score.ToString();
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(5);
        audioSource.PlayOneShot(countdown5);
        yield return new WaitForSeconds(1);
        audioSource.PlayOneShot(countdown4);
        yield return new WaitForSeconds(1);
        audioSource.PlayOneShot(countdown3);
        yield return new WaitForSeconds(1);
        audioSource.PlayOneShot(countdown2);
        yield return new WaitForSeconds(1);
        audioSource.PlayOneShot(countdown1);
    }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(10);
        highScoreBanner.SetActive(false);
        scoreBanner.SetActive(false);
    }
}

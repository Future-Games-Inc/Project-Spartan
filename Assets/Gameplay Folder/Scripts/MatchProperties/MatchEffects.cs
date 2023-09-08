using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using Umbrace.Unity.PurePool;

public class MatchEffects : MonoBehaviour
{

    public int matchCountdown = 10;
    public int currentMatchTime;

    public int currentExtractionTimer = 300;

    public GameObject spawnManager;
    public GameObject uiCanvas;
    //public GameObject[] artifacts;
    //public Transform[] artifactLocations;

    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI extractionTimer;

    private Coroutine timerCoroutine;

    public AudioSource audioSource;
    public AudioClip[] countdownClips;
    public AudioClip matchBegan;
    public AudioClip supplyShip1;
    public AudioClip supplyShip2;

    public GameObject supplyDropShipPrefab;
    public float spawnInterval; // 5 minutes in seconds
    public float lastSpawnTime;

    public bool startMatchBool = false;
    public bool spawnReactor = false;
    public bool spawned = false;
    public bool DE_supplyDrop;

    public string numSequence;

    public float spawnRadius = 300.0f;

    public TextMeshProUGUI nexusCodePanel;

    public GameObjectPoolManager PoolManager;


    // Start is called before the first frame update
    void Start()
    {
        PoolManager = GameObject.FindGameObjectWithTag("Pool").GetComponent<GameObjectPoolManager>();

        InitializeTimer(); // Only the Master Client will initialize the timer
        StartCoroutine(SpawnCheckCoroutine()); // Only the Master Client will handle supply drops

        // Only the Master Client will initialize the sequence        numSequence = GenerateRandomSequence(4);
        numSequence = GenerateRandomSequence(4);
        nexusCodePanel.text = numSequence.ToString();
    }

    IEnumerator SpawnCheckCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (spawned == false && DE_supplyDrop == true)
            {
                lastSpawnTime = Time.time;
                // Create an array to store the valid positions
                Vector3[] spawnPositions = new Vector3[10];
                int validPositionsCount = 0;


                // Generate multiple random positions within the spawn radius
                for (int i = 0; i < spawnPositions.Length; i++)
                {
                    Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
                    randomPosition += transform.position;

                    // Find the nearest point on the NavMesh to the random position
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomPosition, out hit, spawnRadius, NavMesh.AllAreas))
                    {
                        // Add the valid position to the array
                        spawnPositions[validPositionsCount] = hit.position;
                        validPositionsCount++;
                    }
                }

                // If there are valid positions, choose one randomly for spawning the enemy
                if (validPositionsCount > 0)
                {
                    Vector3 spawnPosition = spawnPositions[Random.Range(0, validPositionsCount)];

                    // Adjust the y value to be 100 units above the original spawn position
                    spawnPosition.y += 100;

                    this.PoolManager.Acquire(supplyDropShipPrefab, spawnPosition, Quaternion.identity);
                    StartCoroutine(SupplyShipAudio());
                    spawned = true;
                }
            }
        }
    }
    private void Update()
    {
        RefreshTimerUI();
        if (startMatchBool)
        {
            RefreshCountdownTimer();
        }
    }

    IEnumerator SupplyShipAudio()
    {
        yield return new WaitForSeconds(0);
        audioSource.PlayOneShot(supplyShip1);
        StartCoroutine(PlaySupplyDropAudioDelayed());
    }

    private void RefreshTimerUI()
    {
        string seconds = (currentMatchTime % 60).ToString("00");
        countdownText.text = $"{seconds}";
    }

    void RefreshCountdownTimer()
    {
        string seconds = (currentExtractionTimer % 60).ToString("00");
    }

    private void InitializeTimer()
    {
        currentMatchTime = matchCountdown;

        timerCoroutine = StartCoroutine(TimerEvent());
    }

    IEnumerator TimerEvent()
    {
        yield return new WaitForSeconds(1f);

        if (!startMatchBool)
        {
            currentMatchTime -= 1;

            if (currentMatchTime >= 1 && currentMatchTime <= countdownClips.Length)
            {
                int clipIndex = currentMatchTime - 1;
                audioSource.PlayOneShot(countdownClips[clipIndex]);
            }

            if (currentMatchTime <= 0)
            {
                audioSource.PlayOneShot(matchBegan);
                uiCanvas.SetActive(false);
                startMatchBool = true;
                StartCoroutine(SpawnCheckCoroutine());
                currentExtractionTimer -= 1;
                //StartCoroutine(Artifacts());
            }
            timerCoroutine = StartCoroutine(TimerEvent());
        }
        else
        {
            currentExtractionTimer -= 1;
            timerCoroutine = StartCoroutine(TimerEvent());
        }

        if(currentMatchTime <= 0 && currentExtractionTimer <= 0)
        {
            timerCoroutine = null;
        }

    }

    //IEnumerator Artifacts()
    //{
    //    yield return new WaitForSeconds(1f);
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        for (int i = 0; i < artifacts.Length; i++)
    //        {
    //            PhotonNetwork.this.PoolManager.Acquire(artifacts[i].name, artifactLocations[i].position, Quaternion.identity);
    //        }
    //    }
    //}

    IEnumerator PlaySupplyDropAudioDelayed()
    {
        yield return new WaitForSeconds(supplyShip1.length);
        audioSource.PlayOneShot(supplyShip2);
    }

    private string GenerateRandomSequence(int length)
    {
        string sequence = "";
        System.Random rnd = new System.Random();

        for (int i = 0; i < length; i++)
        {
            sequence += rnd.Next(0, 10);
        }

        return sequence;
    }
}


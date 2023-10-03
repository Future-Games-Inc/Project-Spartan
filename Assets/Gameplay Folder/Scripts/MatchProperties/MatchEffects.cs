using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using LootLocker.Requests;

public class MatchEffects : MonoBehaviour
{

    public int matchCountdown = 10;
    public int currentMatchTime;

    public int currentExtractionTimer = 300;

    public GameObject spawnManager;
    public GameObject uiCanvas;

    public TextMeshProUGUI countdownText;

    private Coroutine timerCoroutine;

    public AudioSource audioSource;
    public AudioClip[] countdownClips;
    public AudioClip matchBegan;
    public AudioClip supplyShip1;
    public AudioClip supplyShip2;

    public float spawnInterval; // 5 minutes in seconds
    public float lastSpawnTime;

    public bool startMatchBool = false;
    public bool spawnReactor = false;
    public bool codeFound = false;
    public bool spawned = false;
    public bool DE_supplyDrop;

    public string numSequence;

    public float spawnRadius = 300.0f;

    public TextMeshProUGUI[] nexusCodePanel;
    public VirtualWorldManager worldManager;
    public SpawnManager1 spawner;

    public bool active = true;
    public string owner;
    public string scene;

    public GameObject[] gameObjects; // assuming you have 4 gameObjects corresponding to 4 owner strings
    public GameObject[] codePanels;
    public float actualExtractionTime;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "WeaponTest")
        {
            int panel = Random.Range(0, codePanels.Length);
            codePanels[panel].SetActive(true);
            InitializeTimer(); // Only the Master Client will initialize the timer
            StartCoroutine(SpawnCheckCoroutine()); // Only the Master Client will handle supply drops

            // Only the Master Client will initialize the sequence        numSequence = GenerateRandomSequence(4);
            numSequence = GenerateRandomSequence(4);
            foreach(TextMeshProUGUI text in nexusCodePanel)
                text.text = numSequence.ToString();
        }

        LootLockerSDKManager.GetMemberRank(scene.ToString(), scene.ToString(), (response) =>
        {
            if (response.success)
            {
                owner = response.metadata;
                ActivateCorrespondingGameObject();
            }
        });
        actualExtractionTime = currentExtractionTimer;
    }

    void ActivateCorrespondingGameObject()
    {
        // Deactivate all GameObjects first
        foreach (var obj in gameObjects)
        {
            obj.SetActive(false);
        }

        // Activate the corresponding GameObject based on the value of owner
        if (owner == "Cyber SK Gang") gameObjects[0].SetActive(true);
        else if (owner == "Muerte De Dios") gameObjects[1].SetActive(true);
        else if (owner == "Chaos Cartel") gameObjects[2].SetActive(true);
        else if (owner == "CintSix Cartel") gameObjects[3].SetActive(true);
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
                StartCoroutine(SupplyShipAudio());
                spawned = true;
                StartCoroutine(spawner.SpawnSupplyDrop());

            }
        }
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "WeaponTest")
        {
            RefreshTimerUI();
            if (startMatchBool)
            {
                RefreshCountdownTimer();
            }
        }

        if (currentExtractionTimer <= 0)
            currentExtractionTimer = 0;
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

        if(currentExtractionTimer == 45)
        {
            PlayerHealth player = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerHealth>();
            PlayerVoiceover voice = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<PlayerVoiceover>();

            StartCoroutine(voice.VoiceOvers(player.faction, 0));
        }

        if (currentMatchTime <= 0 && currentExtractionTimer <= 0 && active)
        {
            worldManager.TimesUP();
            timerCoroutine = null;
            active = false;
        }

    }

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


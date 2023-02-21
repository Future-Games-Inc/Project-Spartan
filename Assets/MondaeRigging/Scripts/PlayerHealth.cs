using CSCore;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
using LootLocker.Requests;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerHealth : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject player;
    public GameObject toxicEffect;
    public GameObject bubbleShield;
    public SpawnManager spawnManager;
    public SceneFader sceneFader;
    public XRDirectInteractor[] directInteractors;
    public XRRayInteractor[] rayInteractors;
    public PlayerMovement movement;
    public GameObject deathToken;
    public GameObject leechBubble;
    public GameObject[] minimapSymbol;
    public GameObject bomb;
    public GameObject bombDeath;
    public GameObject smoke;
    public Transform bombDropLocation;
    public Color minimapStart;
    public Color minimapStealth;
    public GameObject aiCompanionDrone;
    public GameObject decoySpawner;
    public GameObject primaryActive;
    public GameObject secondaryActive;
    public Transform tokenDropLocation;
    public Supercharge superCharge;
    public GameObject fedIcon;
    public GameObject cyberIcon;
    public GameObject cintIcon;
    public GameObject muerteIcon;
    public GameObject chaosIcon;
    public GameObject reactorIcon;
    public GameObject factionIcon;
    public GameObject shipIcon;
    public GameObject[] cyberEmblem;
    public GameObject[] cintEmblem;
    public GameObject[] fedEmblem;
    public GameObject[] chaosEmblem;
    public GameObject[] muerteEmblem;
    public GameObject[] playerObjects;
    public GameObject Artifact1Obj;
    public GameObject Artifact2Obj;
    public GameObject Artifact3Obj;
    public GameObject Artifact4Obj;
    public GameObject Artifact5Obj;
    public GameObject Artifact1Drp;
    public GameObject Artifact2Drp;
    public GameObject Artifact3Drp;
    public GameObject Artifact4Drp;
    public GameObject Artifact5Drp;

    public Transform artifactDrop1;
    public Transform artifactDrop2;
    public Transform artifactDrop3;
    public Transform artifactDrop4;
    public Transform artifactDrop5;

    public int Health = 100;
    public int reactorExtraction;
    public int factionExtractionCount;
    public float reactorTimer = 0;
    public float factionTimer = 0;
    public float toxicTimer = 0;
    public float upgradeTimer = 0;
    public float shieldTimer = 0;
    public float leechEffectTimer = 0;
    public float leechEffectDuration = 20;
    public float activeCamoTimer;
    public float stealthTimer;
    public float stealthDuration = 30;
    public float toxicEffectTimer;
    public float activeCamoDuration = 15;
    public float bulletXPTimer;
    public float shieldEffectTimer;
    public float primaryPowerupEffectTimer = 30;
    public float secondaryPowerupEffectTimer = 40;
    public float doubleAgentTimer;
    public float doubleAgentDuration = 30;
    public float bombRespawnTimer = 15;
    public float berserkerFuryDuration = 20;
    public float startingSpeed;
    public float aiCompanionDuration = 30;
    public float decoyDeployDuration = 30;

    public int playerLives = 3;
    public int playersKilled;
    public int enemiesKilled;
    public int startingBulletModifier;
    public int playerCints;
    public int proxBombCount = 3;
    public int smokeBombCount = 3;
    public int characterInt;
    public int damageTaken;
    public int healthAdded;

    public bool alive;
    public bool reactorHeld;
    public bool extractionWinner;
    public bool playerWinner;
    public bool enemyWinner;
    public bool toxicEffectActive;
    public bool bulletImproved;
    public bool shieldActive;
    public bool leechEffect;
    public bool activeCamo;
    public bool stealth;
    public bool doubleAgent;
    public bool slotAvailable = true;
    public bool decoyDeploy;
    public bool aiCompanion;
    public bool male;
    public bool primaryPowerupTimer;
    public bool secondaryPowerupTimer;
    public bool CyberGangDatacard = false;
    public bool MuerteDeDatacard = false;
    public bool ChaosDatacard = false;
    public bool CintSixDatacard = false;
    public bool FedZoneDatacard = false;
    public bool factionExtraction = false;
    public bool Artifact1;
    public bool Artifact2;
    public bool Artifact3;
    public bool Artifact4;
    public bool Artifact5;

    [SerializeField] private bool primaryButtonPressed;
    [SerializeField] private bool secondaryButtonPressed;

    public int maxHealth;
    public int currentHelath;
    public int healthLevel;
    public int bulletModifier;
    public int bulletXPModifier;
    public int maxAmmo;
    public int factionScore;

    public GameObject winCanvas;
    public TextMeshProUGUI messageText;

    public AudioSource audioSource;
    public AudioClip bulletHit;
    public AudioClip xpClip;
    public AudioClip[] winClipsMale;
    public AudioClip[] winClipsFemale;
    public AudioClip roundWonClip;
    public AudioClip reactorTaken;
    public AudioClip downloading;

    public TextMeshProUGUI reactorText;
    public TextMeshProUGUI factionText;

    public MultiplayerHealth multiplayerHealth;
    public ReactorUI reactorUI;
    public RespawnUI respawnUI;
    public EnemyKillUI enemyKillUI;
    public PlayerKillUI playerKillUI;
    public PlayerHealthBar healthBar;

    public SkinnedMeshRenderer[] characterSkins;

    public string characterFaction;

    [Header("Left Controller ButtonSource")]
    public XRNode left_HandButtonSource;

    public static readonly byte ExtractionGameMode = 1;
    public static readonly byte PlayerGameMode = 2;
    public static readonly byte EnemyGameMode = 3;

    public int leaderboardID = 10220;
    // Start is called before the first frame update
    void Start()
    {
        object storedPlayerHealth;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.PLAYER_HEALTH, out storedPlayerHealth) && (int)storedPlayerHealth >= 1)
            Health = 100 + ((int)storedPlayerHealth * 10);
        else
            Health = 100;

        primaryPowerupTimer = false;
        secondaryPowerupTimer = false;

        object avatarSelectionNumber;
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out avatarSelectionNumber);
        characterInt = (int)avatarSelectionNumber;
        if (characterInt <= 4)
        {
            male = true;
        }
        else
            male = false;

        spawnManager = GameObject.FindGameObjectWithTag("playerSpawnManager").GetComponent<SpawnManager>();
        playerLives = 3;

        StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
        StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));

        reactorExtraction = 0;
        playersKilled = 0;
        enemiesKilled = 0;
        alive = true;
        extractionWinner = false;
        playerWinner = false;
        enemyWinner = false;
        toxicEffectActive = false;
        leechEffect = false;
        activeCamo = false;
        stealth = false;
        doubleAgent = false;
        winCanvas.SetActive(false);
        shipIcon.SetActive(false);
        photonView.RPC("RPC_SetMaxHealth", RpcTarget.AllBuffered, Health);
        //maxHealth = SetMaxHealthFromHealthLevel();
        //multiplayerHealth.SetMaxHealth(maxHealth);

        object storedBulletModifier;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BULLET_MODIFIER, out storedBulletModifier) && (int)storedBulletModifier >= 1)
        {
            bulletModifier = (1 + (int)storedBulletModifier);
        }
        else
        {
            bulletModifier = 1;
        }

        object storedAmmoOverload;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AMMO_OVERLOAD, out storedAmmoOverload) && (int)storedAmmoOverload >= 1)
        {
            maxAmmo = ((int)storedBulletModifier * 5);
        }
        else
        {
            maxAmmo = 0;
        }

        object storedHealthRegen;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_REGEN, out storedHealthRegen) && (int)storedHealthRegen >= 1)
            photonView.RPC("RPC_HealthRegen", RpcTarget.All);
        //InvokeRepeating("HealthRegen", 10, 5);

        object cints;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTS, out cints))
            playerCints = (int)cints;

        object primaryImplant;
        object primaryNode;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE, out primaryImplant) && (int)primaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE_SLOT, out primaryNode) && (int)primaryNode == 1)
        {
            playerLives += 1;
        }

        object secondaryImplant;
        object secondaryNode;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE_SLOT, out secondaryNode) && (int)secondaryNode == 2)
        {
            playerLives += 1;
        }

        startingBulletModifier = bulletModifier;

        if (photonView.IsMine)
        {
            object faction;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CYBER_SK_GANG, out faction) && (int)faction >= 1)
            {
                characterFaction = "Cyber SK Gang".ToString();
                foreach (GameObject emblem in cyberEmblem)
                    emblem.SetActive(true);
            }
            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.MUERTE_DE_DIOS, out faction) && (int)faction >= 1)
            {
                characterFaction = "Muerte De Dios".ToString();
                foreach (GameObject emblem in muerteEmblem)
                    emblem.SetActive(true);
            }
            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CHAOS_CARTEL, out faction) && (int)faction >= 1)
            {
                characterFaction = "Chaos Cartel".ToString();
                foreach (GameObject emblem in chaosEmblem)
                    emblem.SetActive(true);
            }
            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTSIX_CARTEL, out faction) && (int)faction >= 1)
            {
                characterFaction = "CintSix Cartel".ToString();
                foreach (GameObject emblem in cintEmblem)
                    emblem.SetActive(true);
            }
            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.FEDZONE_AUTHORITY, out faction) && (int)faction >= 1)
            {
                characterFaction = "Federation Zone Authority".ToString();
                foreach (GameObject emblem in fedEmblem)
                    emblem.SetActive(true);
            }            
        }
    }

    private int SetMaxHealthFromHealthLevel()
    {
        // TODO: Create Formula to improve health upon level up of character. int 10 can be changed. 
        maxHealth = Health;
        return maxHealth;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        Artifact1Obj.SetActive(Artifact1);
        Artifact2Obj.SetActive(Artifact2);
        Artifact3Obj.SetActive(Artifact3);
        Artifact4Obj.SetActive(Artifact4);
        Artifact5Obj.SetActive(Artifact5);

        foreach (GameObject playerObject in playerObjects)
        {
            if (!photonView.IsMine)
                playerObject.SetActive(false);
        }
        object assignment;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BUTTON_ASSIGN, out assignment) && (int)assignment >= 1)
            primaryActive.SetActive(primaryPowerupTimer);
        else
            primaryActive.SetActive(false);

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BUTTON_ASSIGN, out assignment) && (int)assignment >= 2)
            secondaryActive.SetActive(secondaryPowerupTimer);
        else
            secondaryActive.SetActive(false);

        if (reactorHeld == true)
        {
            reactorText.enabled = true;
            reactorTimer += Time.deltaTime;
            if (reactorTimer > 5f)
            {
                StartCoroutine(ReactorExtraction());
            }
        }
        else
        {
            reactorText.enabled = false;
        }

        if (reactorExtraction >= 100 && spawnManager.gameOver == false)
        {
            extractionWinner = true;
            photonView.RPC("RPC_SpawnManagerTrue", RpcTarget.AllBuffered);
            //spawnManager.gameOver = true;
            //spawnManager.winnerPlayer = this.gameObject;
            StartCoroutine(WinMessage("200 skill points awarded for winning the round"));
            UpdateSkills(200);
            StartCoroutine(SubmitScoreRoutine(characterFaction, 200));
            ExtractionGame();
        }

        if (playersKilled >= 15 && spawnManager.gameOver == false)
        {
            playerWinner = true;
            photonView.RPC("RPC_SpawnManagerTrue", RpcTarget.AllBuffered);
            //spawnManager.gameOver = true;
            //spawnManager.winnerPlayer = this.gameObject;
            StartCoroutine(WinMessage("250 skill points awarded for winning the round"));
            UpdateSkills(250);
            StartCoroutine(SubmitScoreRoutine(characterFaction, 250));
            PlayerGame();
        }

        if (enemiesKilled >= 25 && spawnManager.gameOver == false)
        {
            enemyWinner = true;
            photonView.RPC("RPC_SpawnManagerTrue", RpcTarget.AllBuffered);
            //spawnManager.gameOver = true;
            //spawnManager.winnerPlayer = this.gameObject;
            StartCoroutine(WinMessage("150 skill points awarded for winning the round"));
            UpdateSkills(150);
            StartCoroutine(SubmitScoreRoutine(characterFaction, 150));
            EnemyGame();
        }

        if (toxicTimer <= toxicEffectTimer && toxicEffectActive == true)
        {
            photonView.RPC("RPC_Abilities1True", RpcTarget.AllBuffered);
        }
        else if (toxicTimer > toxicEffectTimer && toxicEffectActive == true)
        {
            photonView.RPC("RPC_Abilities1False", RpcTarget.AllBuffered);
        }

        if (shieldTimer <= shieldEffectTimer && shieldActive == true)
        {
            photonView.RPC("RPC_Abilities2True", RpcTarget.AllBuffered);
        }
        else if (shieldTimer > shieldEffectTimer && shieldActive == true)
        {
            photonView.RPC("RPC_Abilities2False", RpcTarget.AllBuffered);
        }

        if (bulletImproved == true)
        {
            photonView.RPC("RPC_Abilities3True", RpcTarget.AllBuffered);
        }
        else if (upgradeTimer > bulletXPTimer && bulletImproved == true)
        {
            photonView.RPC("RPC_Abilities3False", RpcTarget.AllBuffered);
        }

        if (leechEffect == true && leechEffectTimer <= leechEffectDuration)
        {
            photonView.RPC("RPC_Abilities4True", RpcTarget.AllBuffered);
        }
        else if (leechEffectTimer > leechEffectDuration || leechEffect == false)
        {
            photonView.RPC("RPC_Abilities4False", RpcTarget.AllBuffered);
        }

        if (activeCamo == true && activeCamoTimer <= activeCamoDuration)
        {
            photonView.RPC("RPC_Abilities5True", RpcTarget.AllBuffered);
        }
        else if (activeCamoTimer > activeCamoDuration || activeCamo == false)
        {
            photonView.RPC("RPC_Abilities5False", RpcTarget.AllBuffered);
        }

        if (stealth == true && stealthTimer <= stealthDuration)
        {
            photonView.RPC("RPC_Abilities6True", RpcTarget.AllBuffered);
        }
        else if (stealthTimer > stealthDuration || stealth == false)
        {
            photonView.RPC("RPC_Abilities6False", RpcTarget.AllBuffered);
        }

        if (doubleAgent == true && doubleAgentTimer <= doubleAgentDuration)
        {
            photonView.RPC("RPC_Abilities7True", RpcTarget.AllBuffered);
        }
        else if (doubleAgentTimer > doubleAgentDuration || doubleAgent == false)
        {
            photonView.RPC("RPC_Abilities7False", RpcTarget.AllBuffered);
        }

        photonView.RPC("RPC_Abilities8", RpcTarget.AllBuffered);

        photonView.RPC("RPC_Abilities9", RpcTarget.AllBuffered);

        if (factionExtraction == true)
        {
            factionText.enabled = true;
            factionTimer += Time.deltaTime;
            if (factionTimer > 3f)
            {
                StartCoroutine(FactionExtractionRoutine());
            }
        }
        else
        {
            factionText.enabled = false;
        }

        factionText.text = "Faction Bank Extraction: " + factionExtractionCount + "%";

        InputDevice primaryImplant = InputDevices.GetDeviceAtXRNode(left_HandButtonSource);
        primaryImplant.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonPressed);

        InputDevice secondaryImplant = InputDevices.GetDeviceAtXRNode(left_HandButtonSource);
        secondaryImplant.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonPressed);

        cyberIcon.SetActive(CyberGangDatacard);
        fedIcon.SetActive(FedZoneDatacard);
        muerteIcon.SetActive(MuerteDeDatacard);
        chaosIcon.SetActive(ChaosDatacard);
        cintIcon.SetActive(CintSixDatacard);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("XP") || other.CompareTag("Health") || other.CompareTag("ExtraXP") || other.CompareTag("MinorHealth") || other.CompareTag("toxicDropNormal") || other.CompareTag("toxicDropExtra") || other.CompareTag("bulletModifierNormal")
            || other.CompareTag("bulletModifierExtra") || other.CompareTag("MPShield") || other.CompareTag("deathToken"))
        {
            audioSource.PlayOneShot(xpClip);
        }
    }

    public void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(bulletHit);

        object storedDamageTaken;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DAMAGAE_TAKEN, out storedDamageTaken) && (int)storedDamageTaken >= 1)
            damageTaken = (damage - ((int)storedDamageTaken / 4));
        else
            damageTaken = damage;
        photonView.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, damageTaken);
        CheckHealthStatus();
    }

    public void AddHealth(int health)
    {
        audioSource.PlayOneShot(bulletHit);

        object storedHealthPowerup;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_POWERUP, out storedHealthPowerup) && (int)storedHealthPowerup >= 1)
            healthAdded = (health + (int)storedHealthPowerup);
        else
            healthAdded = health;
        photonView.RPC("RPC_GainHealth", RpcTarget.AllBuffered, healthAdded);
        CheckHealthStatus();
    }

    public void CheckHealthStatus()
    {
        multiplayerHealth.SetCurrentHealth(Health);
    }

    IEnumerator PlayerDeath()
    {
        yield return new WaitForSeconds(0);
        StartCoroutine(sceneFader.ScreenFade());
        GameObject playerDeathTokenObject = PhotonNetwork.Instantiate(deathToken.name, tokenDropLocation.position, Quaternion.identity, 0);
        playerDeathTokenObject.GetComponent<playerDeathToken>().tokenValue = (playerCints / 4);
        playerDeathTokenObject.GetComponent<playerDeathToken>().faction = characterFaction.ToString();

        object implant;
        object node;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.EXPLOSIVE_DEATH, out implant) && (int)implant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.EXPLOSIVE_DEATH_SLOT, out node) && (int)node >= 1)
        {
            PhotonNetwork.Instantiate(bombDeath.name, tokenDropLocation.position, Quaternion.identity, 0);
        }

        if (Artifact1)
        {
            PhotonNetwork.Instantiate(Artifact1Drp.name, artifactDrop1.position, Quaternion.identity, 0);
            Artifact1 = false;
        }
        if (Artifact2)
        {
            PhotonNetwork.Instantiate(Artifact2Drp.name, artifactDrop2.position, Quaternion.identity, 0);
            Artifact2 = false;
        }
        if (Artifact3)
        {
            PhotonNetwork.Instantiate(Artifact3Drp.name, artifactDrop3.position, Quaternion.identity, 0);
            Artifact3 = false;
        }
        if (Artifact4)
        {
            PhotonNetwork.Instantiate(Artifact4Drp.name, artifactDrop4.position, Quaternion.identity, 0);
            Artifact4 = false;
        }
        if (Artifact5)
        {
            PhotonNetwork.Instantiate(Artifact5Drp.name, artifactDrop5.position, Quaternion.identity, 0);
            Artifact5 = false;
        }
        yield return new WaitForSeconds(.75f);
        VirtualWorldManager.Instance.LeaveRoomAndLoadHomeScene();
    }

    IEnumerator PlayerRespawn()
    {
        yield return new WaitForSeconds(0);

        foreach (XRRayInteractor ray in rayInteractors)
        {
            ray.enabled = false;
        }
        foreach (XRDirectInteractor direct in directInteractors)
        {
            direct.enabled = false;
        }

        StartCoroutine(sceneFader.Respawn());
        photonView.RPC("RPC_Respawn", RpcTarget.AllBuffered);

        //player.transform.position = spawnManager.spawnPosition;
        //playerLives -= 1;

        foreach (XRRayInteractor ray in rayInteractors)
        {
            ray.enabled = true;
        }
        foreach (XRDirectInteractor direct in directInteractors)
        {
            direct.enabled = true;
        }

        respawnUI.UpdateRespawnUI();
    }

    IEnumerator ReactorExtraction()
    {
        yield return new WaitForSeconds(0);

        object storedReactorExtraction;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.REACTOR_EXTRACTION, out storedReactorExtraction) && (int)storedReactorExtraction >= 1)
            reactorExtraction += (2 + (int)storedReactorExtraction);
        else
            reactorExtraction += 2;

        Hashtable hash = new Hashtable();
        hash.Add("reactorExtraction", reactorExtraction);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        reactorUI.UpdateReactorUI();
        reactorTimer = 0;
    }

    IEnumerator FactionExtractionRoutine()
    {
        yield return new WaitForSeconds(0);
        factionExtractionCount += 5;

        factionTimer = 0;
    }

    [System.Obsolete]
    public void EnemyKilled()
    {
        enemiesKilled++;

        Hashtable hash = new Hashtable();
        hash.Add("enemyKills", enemiesKilled);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        enemyKillUI.CheckEnemiesKilled();

        int playAudio = Random.Range(0, 100);
        if (!audioSource.isPlaying && playAudio <= 50)
        {
            if (male)
            {
                audioSource.PlayOneShot(winClipsMale[Random.Range(0, winClipsMale.Length)]);
            }
            else
                audioSource.PlayOneShot(winClipsFemale[Random.Range(0, winClipsFemale.Length)]);
        }

        superCharge.IncreaseKillCount();

        StartCoroutine(SubmitScoreRoutine(characterFaction, 20));
        StartCoroutine(GetXP(2));
    }

    [System.Obsolete]
    public void PlayersKilled()
    {
        playersKilled++;

        Hashtable hash = new Hashtable();
        hash.Add("playerKills", playersKilled);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        playerKillUI.CheckEnemiesKilled();

        int playAudio = Random.Range(0, 100);
        if (!audioSource.isPlaying && playAudio <= 50)
        {
            if (male)
            {
                audioSource.PlayOneShot(winClipsMale[Random.Range(0, winClipsMale.Length)]);
            }
            else
                audioSource.PlayOneShot(winClipsFemale[Random.Range(0, winClipsFemale.Length)]);
        }
        superCharge.IncreaseKillCount();

        StartCoroutine(SubmitScoreRoutine(characterFaction, 50));
        StartCoroutine(GetXP(5));
    }

    void ExtractionGame()
    {
        StartCoroutine(GetXP(50));
        if (Artifact1 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact1 = false;
        }
        if (Artifact2 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact2 = false;
        }
        if (Artifact3 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact3 = false;
        }
        if (Artifact4 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact4 = false;
        }
        if (Artifact5 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact5 = false;
        }

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(ExtractionGameMode, null, raiseEventOptions, sendOptions);
    }

    void PlayerGame()
    {
        StartCoroutine(GetXP(30));

        if (Artifact1 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact1 = false;
        }
        if (Artifact2 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact2 = false;
        }
        if (Artifact3 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact3 = false;
        }
        if (Artifact4 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact4 = false;
        }
        if (Artifact5 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact5 = false;
        }

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(PlayerGameMode, null, raiseEventOptions, sendOptions);
    }

    void EnemyGame()
    {
        StartCoroutine(GetXP(10));

        if (Artifact1 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact1 = false;
        }
        if (Artifact2 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact2 = false;
        }
        if (Artifact3 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact3 = false;
        }
        if (Artifact4 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact4 = false;
        }
        if (Artifact5 == true)
        {
            StartCoroutine(GetXP(100));
            Artifact5 = false;
        }

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(EnemyGameMode, null, raiseEventOptions, sendOptions);
    }

    IEnumerator DisplayMessage(string message)
    {
        Debug.Log("Display Start");
        yield return new WaitForSeconds(3);
        winCanvas.SetActive(true);
        messageText.text = message;
        yield return new WaitForSeconds(5);
        messageText.text = "";
        VirtualWorldManager.Instance.LeaveRoomAndLoadHomeScene();
        Debug.Log("Display End");
    }

    IEnumerator WinMessage(string message)
    {
        yield return new WaitForSeconds(0);
        audioSource.PlayOneShot(roundWonClip);
        winCanvas.SetActive(true);
        messageText.text = message;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == ExtractionGameMode)
        {
            string name = spawnManager.winnerPlayer.GetComponent<PhotonView>().Owner.NickName;
            StartCoroutine(DisplayMessage($"{name} has extracted the reactor for their faction. Returning to Faction Base."));
        }

        if (photonEvent.Code == PlayerGameMode)
        {
            string name = spawnManager.winnerPlayer.GetComponent<PhotonView>().Owner.NickName;
            StartCoroutine(DisplayMessage($"{name} has defeated {playersKilled} players and won the territory. Returning to Faction Base."));
        }

        if (photonEvent.Code == EnemyGameMode)
        {
            string name = spawnManager.winnerPlayer.GetComponent<PhotonView>().Owner.NickName;
            StartCoroutine(DisplayMessage($"{name} has defeated {enemiesKilled} enemies and won the territory. Returning to Faction Base."));
        }

        if (photonEvent.Code == ReactorGrab.ReactorExtractionTrue)
        {
            reactorIcon.SetActive(true);
            audioSource.PlayOneShot(reactorTaken);
        }

        if (photonEvent.Code == FactionExtraction.FactionExtractionTrue)
        {
            factionIcon.SetActive(true);
            audioSource.PlayOneShot(downloading);
        }

        if (photonEvent.Code == ReactorGrab.ReactorExtractionFalse)
        {
            reactorIcon.SetActive(false);

        }

        if (photonEvent.Code == FactionExtraction.FactionExtractionFalse)
        {
            factionIcon.SetActive(false);
        }

        if (photonEvent.Code == SupplyDropShip.SupplyShipArrive)
        {
            shipIcon.SetActive(true);
        }

        if (photonEvent.Code == SupplyDropShip.SupplyShipDestroy)
        {
            shipIcon.SetActive(false);
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void Toxicity(float toxicTime)
    {
        toxicTimer = 0;
        toxicEffectTimer = toxicTime;
    }

    public void BulletImprove(float bulletTimer, int newModifier)
    {
        upgradeTimer = 0;
        bulletXPTimer = bulletTimer;
        bulletXPModifier = newModifier;
        startingBulletModifier = bulletModifier;
    }

    public void HealthRegen()
    {

        if (Health < maxHealth)
            AddHealth(2);
    }

    public void Shield(float shieldTime)
    {
        shieldTimer = 0;

        object storedShieldDuration;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SHIELD_DURATION, out storedShieldDuration) && (int)storedShieldDuration >= 1)
            shieldEffectTimer = shieldTime + ((int)storedShieldDuration * (int)1.5);
        else
            shieldEffectTimer = shieldTime;
    }

    public void UpdateSkills(int cintsEarned)
    {
        playerCints += cintsEarned;

        ExitGames.Client.Photon.Hashtable cintsUpdate = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTS, playerCints } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cintsUpdate);
        Debug.Log("Cints Updated");
    }

    IEnumerator PrimaryTimer(float time)
    {
        yield return new WaitForSeconds(time);
        primaryPowerupTimer = true;
    }

    IEnumerator SecondaryTimer(float time)
    {
        yield return new WaitForSeconds(time);
        secondaryPowerupTimer = true;
    }

    void PrimaryImplantActivation()
    {
        if (primaryButtonPressed && primaryPowerupTimer == true)
        {
            primaryPowerupTimer = false;

            object primaryImplant;
            object primaryNode;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_STIM, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_STIM_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                AddHealth(25);
                StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.LEECH, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.LEECH_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                leechEffect = true;
                leechEffectTimer = 0;
                StartCoroutine(PrimaryPowerupDelay(leechEffectDuration));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ACTIVE_CAMO, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ACTIVE_CAMO_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                activeCamo = true;
                activeCamoTimer = 0;
                StartCoroutine(PrimaryPowerupDelay(activeCamoDuration));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.STEALTH, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.STEALTH_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                stealth = true;
                stealthTimer = 0;
                StartCoroutine(PrimaryPowerupDelay(stealthDuration));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DOUBLE_AGENT, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DOUBLE_AGENT_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                doubleAgent = true;
                doubleAgentTimer = 0;
                StartCoroutine(PrimaryPowerupDelay(doubleAgentDuration));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.PROXIMITY_BOMB, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.PROXIMITY_BOMB_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                if (slotAvailable == true)
                {
                    if (proxBombCount > 1)
                        proxBombCount--;
                    if (proxBombCount == 1)
                        proxBombCount = 3;
                    PhotonNetwork.Instantiate(bomb.name, bombDropLocation.position, Quaternion.identity, 0);
                }
                StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SMOKE_BOMB, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SMOKE_BOMB_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                if (slotAvailable == true)
                {
                    if (smokeBombCount > 1)
                        smokeBombCount--;
                    if (smokeBombCount == 1)
                        smokeBombCount = 3;
                    PhotonNetwork.Instantiate(smoke.name, bombDropLocation.position, Quaternion.identity, 0);
                }
                StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BERSERKER_FURY, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BERSERKER_FURY_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                photonView.RPC("RPC_SetMaxHealth", RpcTarget.AllBuffered, ((int)maxHealth + 100));

                startingSpeed = movement.movementSpeed;
                movement.movementSpeed += 2;

                startingBulletModifier = bulletModifier;
                bulletModifier += 4;

                StartCoroutine(PrimaryBerserkerDelay(berserkerFuryDuration));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                aiCompanion = true;
                StartCoroutine(PrimaryPowerupDelay(aiCompanionDuration));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                decoyDeploy = true;
                StartCoroutine(PrimaryPowerupDelay(decoyDeployDuration));
            }
        }
    }

    void SecondayImplantActivation()
    {
        if (secondaryButtonPressed && secondaryPowerupTimer == true)
        {
            object secondaryImplant;
            object secondaryNode;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_STIM, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_STIM_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                AddHealth(25);
                StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.LEECH, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.LEECH_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                leechEffect = true;
                leechEffectTimer = 0;
                StartCoroutine(SecondaryPowerupDelay(leechEffectDuration));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ACTIVE_CAMO, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ACTIVE_CAMO_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                activeCamo = true;
                activeCamoTimer = 0;
                StartCoroutine(SecondaryPowerupDelay(activeCamoDuration));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.STEALTH, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.STEALTH_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                stealth = true;
                stealthTimer = 0;
                StartCoroutine(SecondaryPowerupDelay(stealthDuration));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DOUBLE_AGENT, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DOUBLE_AGENT_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                doubleAgent = true;
                doubleAgentTimer = 0;
                StartCoroutine(SecondaryPowerupDelay(doubleAgentDuration));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.PROXIMITY_BOMB, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.PROXIMITY_BOMB_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                if (slotAvailable == true)
                {
                    if (proxBombCount > 1)
                        proxBombCount--;
                    if (proxBombCount == 1)
                        proxBombCount = 3;
                    PhotonNetwork.Instantiate(bomb.name, bombDropLocation.position, Quaternion.identity, 0);
                }
                StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SMOKE_BOMB, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SMOKE_BOMB_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                if (slotAvailable == true)
                {
                    if (smokeBombCount > 1)
                        smokeBombCount--;
                    if (smokeBombCount == 1)
                        smokeBombCount = 3;
                    PhotonNetwork.Instantiate(smoke.name, bombDropLocation.position, Quaternion.identity, 0);
                }
                StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BERSERKER_FURY, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BERSERKER_FURY_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                photonView.RPC("RPC_SetMaxHealth", RpcTarget.AllBuffered, ((int)maxHealth + 100));

                startingSpeed = movement.movementSpeed;
                movement.movementSpeed += 2;

                startingBulletModifier = bulletModifier;
                bulletModifier += 4;

                StartCoroutine(SecondaryBerserkerDelay(berserkerFuryDuration));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                aiCompanion = true;
                StartCoroutine(SecondaryPowerupDelay(aiCompanionDuration));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                decoyDeploy = true;
                StartCoroutine(SecondaryPowerupDelay(decoyDeployDuration));
            }
        }
    }

    IEnumerator PrimaryPowerupDelay(float time)
    {
        yield return new WaitForSeconds(time);
        {
            object secondaryImplant;
            object secondaryNode;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION_SLOT, out secondaryNode) && (int)secondaryNode == 1)
                aiCompanion = false;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION_SLOT, out secondaryNode) && (int)secondaryNode == 1)
                decoyDeploy = false;
            StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
        }
    }

    IEnumerator PrimaryBerserkerDelay(float time)
    {
        yield return new WaitForSeconds(time);
        {
            photonView.RPC("RPC_SetMaxHealth", RpcTarget.AllBuffered, ((int)maxHealth));
            movement.movementSpeed = startingSpeed;
            bulletModifier = startingBulletModifier;
            StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
        }
    }
    IEnumerator SecondaryPowerupDelay(float time)
    {
        yield return new WaitForSeconds(time);
        {
            object secondaryImplant;
            object secondaryNode;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION_SLOT, out secondaryNode) && (int)secondaryNode == 2)
                aiCompanion = false;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION_SLOT, out secondaryNode) && (int)secondaryNode == 2)
                decoyDeploy = false;
            StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
        }
    }

    IEnumerator SecondaryBerserkerDelay(float time)
    {
        yield return new WaitForSeconds(time);
        {
            photonView.RPC("RPC_SetMaxHealth", RpcTarget.AllBuffered, ((int)maxHealth));
            movement.movementSpeed = startingSpeed;
            bulletModifier = startingBulletModifier;
            StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
        }
    }

    public void FactionDataCard(string faction)
    {
        if (faction == "Cyber SK Gang".ToString() && characterFaction != faction.ToString() && CyberGangDatacard == false)
            CyberGangDatacard = true;
        if (faction == "Muerte De Dios".ToString() && characterFaction != faction.ToString() && MuerteDeDatacard == false)
            MuerteDeDatacard = true;
        if (faction == "Chaos Cartel".ToString() && characterFaction != faction.ToString() && ChaosDatacard == false)
            ChaosDatacard = true;
        if (faction == "CintSix Cartel".ToString() && characterFaction != faction.ToString() && CintSixDatacard == false)
            CintSixDatacard = true;
        if (faction == "Federation Zone Authority".ToString() && characterFaction != faction.ToString() && FedZoneDatacard == false)
            FedZoneDatacard = true;
    }

    [System.Obsolete]
    public IEnumerator SubmitScoreRoutine(string faction, int scoreToUpload)
    {
        LootLockerSDKManager.GetMemberRank("faction_leaderboard", faction, (response) =>
        {
            if (response.statusCode == 200)
            {
                factionScore = response.score;
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });

        bool done = false;
        LootLockerSDKManager.SubmitScore(characterFaction, (factionScore + scoreToUpload), leaderboardID, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully uploaded score");
                done = true;
            }
            else
            {
                Debug.Log("Failed" + response.Error);
                done = true;
            }
        });
        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator GetXP(int XP)
    {
        yield return new WaitForSeconds(0);
        LootLockerSDKManager.SubmitXp((XP), (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully uploaded score");
            }
            else
            {
                Debug.Log("Failed" + response.Error);
            }
        });
    }

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        if (!photonView.IsMine)
        { return; }

        Health -= damage;
        healthBar.SetCurrentHealth(Health);

        if (Health <= 0 && playerLives > 1 && alive == true)
        {
            alive = false;
            StartCoroutine(PlayerRespawn());
        }

        else if (Health <= 0 && playerLives == 1 && alive == true)
        {
            alive = false;
            StartCoroutine(PlayerDeath());
        }
    }

    [PunRPC]
    void RPC_GainHealth(int health)
    {
        if (!photonView.IsMine)
        { return; }

        Health += health;
        healthBar.SetCurrentHealth(Health);
    }

    [PunRPC]
    void RPC_Respawn()
    {
        if (!photonView.IsMine)
        { return; }

        player.transform.position = spawnManager.spawnPosition;
        playerLives -= 1;
        Health = 125;
        healthBar.SetMaxHealth(Health);
        CheckHealthStatus();
        alive = true;
    }

    [PunRPC]
    void RPC_SetMaxHealth(int Health)
    {
        if (!photonView.IsMine)
        { return; }

        maxHealth = Health;
        multiplayerHealth.SetMaxHealth(maxHealth);
        healthBar.SetMaxHealth(maxHealth);
    }

    [PunRPC]
    void RPC_SpawnManagerTrue()
    {
        spawnManager.gameOver = true;
        spawnManager.winnerPlayer = this.gameObject;
    }

    [PunRPC]
    void RPC_Abilities1True()
    {
        if (!photonView.IsMine)
        { return; }

        toxicEffect.SetActive(true);
        toxicTimer += Time.deltaTime;
    }

    [PunRPC]
    void RPC_Abilities1False()
    {
        if (!photonView.IsMine)
        { return; }

        toxicEffect.SetActive(false);
        toxicEffectActive = false;
    }

    [PunRPC]
    void RPC_Abilities2True()
    {
        if (!photonView.IsMine)
        { return; }

        bubbleShield.SetActive(true);
        shieldTimer += Time.deltaTime;
    }

    [PunRPC]
    void RPC_Abilities2False()
    {
        if (!photonView.IsMine)
        { return; }

        bubbleShield.SetActive(false);
        shieldActive = false;
    }

    [PunRPC]
    void RPC_Abilities3True()
    {
        if (!photonView.IsMine)
        { return; }

        bulletModifier = startingBulletModifier + bulletXPModifier;
        upgradeTimer += Time.deltaTime;
    }

    [PunRPC]
    void RPC_Abilities3False()
    {
        if (!photonView.IsMine)
        { return; }

        bulletModifier = startingBulletModifier;
        bulletImproved = false;
    }

    [PunRPC]
    void RPC_Abilities4True()
    {
        if (!photonView.IsMine)
        { return; }

        leechEffectTimer += Time.deltaTime;
        leechBubble.SetActive(true);
    }

    [PunRPC]
    void RPC_Abilities4False()
    {
        if (!photonView.IsMine)
        { return; }

        leechBubble.SetActive(false);
        leechEffect = false;
    }

    [PunRPC]
    void RPC_Abilities5True()
    {
        activeCamoTimer += Time.deltaTime;
        if (!photonView.IsMine)
        {
            foreach (SkinnedMeshRenderer skin in characterSkins)
            {
                skin.enabled = false;
            }
        }
    }

    [PunRPC]
    void RPC_Abilities5False()
    {
        activeCamo = false;
        if (!photonView.IsMine || photonView.IsMine)
        {
            foreach (SkinnedMeshRenderer skin in characterSkins)
            {
                skin.enabled = true;
            }
        }
    }

    [PunRPC]
    void RPC_Abilities6True()
    {
        stealthTimer += Time.deltaTime;
        if (!photonView.IsMine)
        {
            foreach (GameObject minimap in minimapSymbol)
            {
                minimap.SetActive(false);
            }
        }
    }

    [PunRPC]
    void RPC_Abilities6False()
    {
        stealth = false;
        foreach (GameObject minimap in minimapSymbol)
        {
            minimap.SetActive(true);
        }
    }

    [PunRPC]
    void RPC_Abilities7True()
    {
        doubleAgentTimer += Time.deltaTime;
        if (!photonView.IsMine)
        {
            foreach (GameObject minimap in minimapSymbol)
            {
                minimap.GetComponent<SpriteRenderer>().color = minimapStealth;
            }
        }
    }

    [PunRPC]
    void RPC_Abilities7False()
    {
        if (!photonView.IsMine)
        { return; }

        foreach (GameObject minimap in minimapSymbol)
        {
            minimap.GetComponent<SpriteRenderer>().color = minimapStart;
        }
        doubleAgent = false;
    }

    [PunRPC]
    void RPC_Abilities8()
    {
        if (!photonView.IsMine)
        { return; }

        if (aiCompanion == true)
        {
            aiCompanionDrone.SetActive(true);
        }
        else
            aiCompanionDrone.SetActive(false);
    }

    [PunRPC]
    void RPC_Abilities9()
    {
        if (!photonView.IsMine)
        { return; }

        if (decoyDeploy == true)
        {
            decoySpawner.SetActive(true);
        }
        else
            decoySpawner.SetActive(false);
    }

    [PunRPC]
    void RPC_HealthRegen()
    {
        if (!photonView.IsMine)
        { return; }

        InvokeRepeating("HealthRegen", 0f, 3f);
    }
}
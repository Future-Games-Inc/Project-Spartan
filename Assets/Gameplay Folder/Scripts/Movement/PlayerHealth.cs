using LootLocker.Requests;
using PathologicalGames;
using System.Collections;
using TMPro;
using Umbrace.Unity.PurePool;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerHealth : MonoBehaviour
{
    public enum States
    {
        Normal,
        Shocked
    }

    [Header("Player Characteristics ------------------------------------")]
    public States activeState = States.Normal;
    public GameObject player;

    public SpawnManager spawnManager;
    public PlayerMovement movement;
    public SceneFader sceneFader;

    public XRDirectInteractor[] directInteractors;

    public bool male;
    public bool alive;
    public bool reactorHeld;
    public bool extractionWinner;
    public bool playerWinner;
    public bool enemyWinner;

    public string characterFaction;

    public float startingSpeed;
    public float reactorTimer = 0;

    public int playerLives = 3;
    public int datacards = 0;
    public int playersKilled;
    public int enemiesKilled;
    public int startingBulletModifier;
    public int playerCints;
    public int characterInt;
    public int damageTaken;
    public int healthAdded;
    public int maxHealth;
    public int currentHelath;
    public int healthLevel;
    public int Health = 100;
    public int armorAdded;
    public int maxArmor;
    public int currentArmor;
    public int armorLevel;
    public int Armor = 100;
    public int reactorExtraction;

    [Header("Player State Effects ------------------------------------")]
    public GameObject shockEffect;

    [Header("Player UI ------------------------------------")]
    public GameObject reactorIcon;
    public GameObject shipIcon;
    public GameObject[] playerObjects;
    public GameObject[] shieldObjects;
    public GameObject healthBarObject;
    public GameObject armorBarObject;
    public GameObject criticalHealth;
    public GameObject crackedScreen;
    public GameObject[] minimapSymbol;
    public GameObject primaryActive;
    public GameObject secondaryActive;
    public GameObject deathToken;
    public GameObject bombDeath;
    public GameObject model;
    public Transform meleeAttach;

    public Transform bombDropLocation;
    public Transform tokenDropLocation;

    public ActivateWristUI activateWristUI;

    public MultiplayerHealth multiplayerHealth;
    public RespawnUI respawnUI;

    [Header("Player Audio ------------------------------------")]
    public AudioSource audioSource;
    public AudioClip bulletHit;
    public AudioClip xpClip;
    public AudioClip[] winClipsMale;
    public AudioClip[] winClipsFemale;
    public AudioClip roundWonClip;
    public AudioClip reactorTaken;
    public AudioClip downloading;

    [Header("Player Abilities ------------------------------------")]
    public GameObject toxicEffect;
    public GameObject leechBubble;

    public int bulletModifier;
    public int bulletXPModifier;
    public int maxAmmo;

    public float toxicTimer = 0;
    public float upgradeTimer = 0;
    public float leechEffectTimer = 0;
    public float leechEffectDuration = 20;
    public float toxicEffectTimer;
    public float bulletXPTimer;
    public float berserkerEffectTimer;
    public float primaryPowerupEffectTimer = 30;
    public float secondaryPowerupEffectTimer = 40;

    public bool toxicEffectActive;
    public bool bulletImproved;
    public bool leechEffect;
    public bool primaryPowerupTimer;
    public TextMeshProUGUI primaryPowerupText;
    public TextMeshProUGUI secondaryPowerupText;
    public bool secondaryPowerupTimer;
    public float activeCamoTimer;
    public float stealthTimer;
    public float activeCamoDuration = 15;
    bool shouldCallAbilities1True;
    bool shouldCallAbilities1False;
    bool shouldCallAbilities3True;
    bool shouldCallAbilities3False;
    bool shouldCallAbilities4True;
    bool shouldCallAbilities4False;
    bool shouldCallAbilities5True;
    bool shouldCallAbilities5False;
    bool shouldCallAbilities6True;
    bool shouldCallAbilities6False;
    bool shouldCallAbilities8;
    bool shouldCallAbilities9;
    bool shouldCallAbilities10True;
    bool shouldCallAbilities10False;
    bool hasButtonAssignment;
    bool hasButtonAssignment2;

    [Header("Player Inputs ------------------------------------")]
    public XRNode left_HandButtonSource;
    [SerializeField] private bool primaryButtonPressed;
    [SerializeField] private bool secondaryButtonPressed;

    [Header("Player Leaderboard Data ------------------------------------")]
    public string leaderboardID = "react_leaderboard";
    public string progressionKey = "cent_prog";

    [Header("Contract Tracking ------------------------------------")]
    public int bossesKilled;
    public int bossesRequired;

    public int guardiansDestroyed;
    public int guardianRequired;

    public int collectorsDestroyed;
    public int collectorsRequired;

    public int artifactsRecovered;
    public int artifactsRequired;

    public int intelCollected;
    public int intelRequired;

    public int bombsDestroyed;
    public int bombsRequired;

    public int expReward;
    public int cintReward;
    public bool decoyDeploy;
    public bool aiCompanion;
    public bool activeCamo;
    public bool stealth;
    public bool berserker;
    public bool berserkerActivated = false;
    public float stealthDuration = 30;
    public float aiCompanionDuration = 30;
    public float decoyDeployDuration = 30;
    public float berserkerFuryDuration = 20;
    public SkinnedMeshRenderer[] characterSkins;
    public GameObject aiCompanionDrone;
    public GameObject decoySpawner;
    private object retrurn;

    public GameObjectPoolManager PoolManager;


    // Start is called before the first frame update
    void OnEnable()
    {
        PoolManager = GameObject.FindGameObjectWithTag("Pool").GetComponent<GameObjectPoolManager>();

        criticalHealth.SetActive(false);

        InitHealth();

        InitArmor();

        primaryPowerupTimer = false;
        secondaryPowerupTimer = false;

        InitAvatarSelection();

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
        shipIcon.SetActive(false);

        InitBulletModifier();

        InitMaxAmmo();

        InitHealthRegen();

        InitPlayerCints();

        InitSavingGrace();

        InitBossesKilled();

        InitCollectorsKilled();

        InitGuardiansKilled();

        InitBombsDestroyed();

        InitArtifactsFound();

        InitIntelGathered();

        startingBulletModifier = bulletModifier;

        healthBarObject.SetActive(true);
        armorBarObject.SetActive(true);

        CheckHealthStatus();
        CheckArmorStatus();

        if (PlayerPrefs.HasKey("BUTTON_ASSIGN") && PlayerPrefs.GetInt("BUTTON_ASSIGN") >= 1)
            hasButtonAssignment = true;
        if (PlayerPrefs.HasKey("BUTTON_ASSIGN") && PlayerPrefs.GetInt("BUTTON_ASSIGN") >= 2)
            hasButtonAssignment2 = true;

        if (PlayerPrefs.HasKey("BossQuestTarget"))
            bossesRequired = PlayerPrefs.GetInt("BossQuestTarget");
        else
            bossesRequired = 0;

        if (PlayerPrefs.HasKey("GuardianQuestTarget"))
            guardianRequired = PlayerPrefs.GetInt("GuardianQuestTarget");
        else
            guardianRequired = 0;

        if (PlayerPrefs.HasKey("CollectorQuestTarget"))
            collectorsRequired = PlayerPrefs.GetInt("CollectorQuestTarget");
        else
            collectorsRequired = 0;

        if (PlayerPrefs.HasKey("IntelQuestTarget"))
            intelRequired = PlayerPrefs.GetInt("IntelQuestTarget");
        else
            intelRequired = 0;

        if (PlayerPrefs.HasKey("ArtifactQuestTarget"))
            artifactsRequired = PlayerPrefs.GetInt("ArtifactQuestTarget");
        else
            artifactsRequired = 0;

        if (PlayerPrefs.HasKey("BombQuestTarget"))
            bombsRequired = PlayerPrefs.GetInt("BombQuestTarget");
        else
            bombsRequired = 0;

        UpdatePrimaryText();
        UpdateSecondaryText();
    }

    void UpdateSecondaryText()
    {
        if (PlayerPrefs.HasKey("HEALTH_STIM") && PlayerPrefs.GetInt("HEALTH_STIM") >= 1 &&
                PlayerPrefs.HasKey("HEALTH_STIM_SLOT") && PlayerPrefs.GetInt("HEALTH_STIM_SLOT") == 2)
        {
            secondaryPowerupText.text = "HEALTH STIM";
        }

        else if (PlayerPrefs.HasKey("LEECH") && PlayerPrefs.GetInt("LEECH") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 2)
        {
            secondaryPowerupText.text = "LEECH";
        }

        else if (PlayerPrefs.HasKey("ACTIVE_CAMO") && PlayerPrefs.GetInt("ACTIVE_CAMO") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 2)
        {
            secondaryPowerupText.text = "ACTIVE CAMO";
        }

        else if (PlayerPrefs.HasKey("STEALTH") && PlayerPrefs.GetInt("STEALTH") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 2)
        {
            secondaryPowerupText.text = "STEALTH";
        }

        else if (PlayerPrefs.HasKey("AI_COMPANION") && PlayerPrefs.GetInt("AI_COMPANION") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 2)
        {
            secondaryPowerupText.text = "AI COMPANION";
        }

        else if (PlayerPrefs.HasKey("DECOY_DEPLOYMENT") && PlayerPrefs.GetInt("DECOY_DEPLOYMENT") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 2)
        {
            secondaryPowerupText.text = "DECOY DEPLOYMENT";
        }

        else if (PlayerPrefs.HasKey("BERSERKER_FURY") && PlayerPrefs.GetInt("BERSERKER_FURY") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") >= 2)
        {
            secondaryPowerupText.text = "BERSERKER FURY";
        }

        else if (PlayerPrefs.HasKey("SAVING_GRACE") && PlayerPrefs.GetInt("SAVING_GRACE") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 2)
        {
            secondaryPowerupText.text = "SAVING GRACE APPLIED";
        }

        else if (PlayerPrefs.HasKey("EXPLOSIVE_DEATH") && PlayerPrefs.GetInt("EXPLOSIVE_DEATH") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 2)
        {
            secondaryPowerupText.text = "EXPLOSIVE DEATH APPLIED";
        }
    }

    void UpdatePrimaryText()
    {
        if (PlayerPrefs.HasKey("HEALTH_STIM") && PlayerPrefs.GetInt("HEALTH_STIM") >= 1 &&
                PlayerPrefs.HasKey("HEALTH_STIM_SLOT") && PlayerPrefs.GetInt("HEALTH_STIM_SLOT") == 1)
        {
            primaryPowerupText.text = "HEALTH STIM";
        }

        else if (PlayerPrefs.HasKey("LEECH") && PlayerPrefs.GetInt("LEECH") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 1)
        {
            primaryPowerupText.text = "LEECH";
        }

        else if (PlayerPrefs.HasKey("ACTIVE_CAMO") && PlayerPrefs.GetInt("ACTIVE_CAMO") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 1)
        {
            primaryPowerupText.text = "ACTIVE CAMO";
        }

        else if (PlayerPrefs.HasKey("STEALTH") && PlayerPrefs.GetInt("STEALTH") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 1)
        {
            primaryPowerupText.text = "STEALTH";
        }

        else if (PlayerPrefs.HasKey("AI_COMPANION") && PlayerPrefs.GetInt("AI_COMPANION") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 1)
        {
            primaryPowerupText.text = "AI COMPANION";
        }

        else if (PlayerPrefs.HasKey("DECOY_DEPLOYMENT") && PlayerPrefs.GetInt("DECOY_DEPLOYMENT") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 1)
        {
            primaryPowerupText.text = "DECOY DEPLOYMENT";
        }

        else if (PlayerPrefs.HasKey("BERSERKER_FURY") && PlayerPrefs.GetInt("BERSERKER_FURY") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 1)
        {
            primaryPowerupText.text = "BERSERKER FURY";
        }

        else if (PlayerPrefs.HasKey("SAVING_GRACE") && PlayerPrefs.GetInt("SAVING_GRACE") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 1)
        {
            primaryPowerupText.text = "SAVING GRACE APPLIED";
        }

        else if (PlayerPrefs.HasKey("EXPLOSIVE_DEATH") && PlayerPrefs.GetInt("EXPLOSIVE_DEATH") >= 1 &&
                PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 1)
        {
            primaryPowerupText.text = "EXPLOSIVE DEATH APPLIED";
        }
    }

    private void InitHealth()
    {
        Health = PlayerPrefs.HasKey("PLAYER_HEALTH") && PlayerPrefs.GetInt("PLAYER_HEALTH") >= 1
            ? 100 + (PlayerPrefs.GetInt("PLAYER_HEALTH") * 10) : 100;

        multiplayerHealth.SetMaxHealth(Health);
    }

    private void InitArmor()
    {
        Armor = PlayerPrefs.HasKey("PLAYER_ARMOR") && PlayerPrefs.GetInt("PLAYER_HEALTH") >= 1
            ? 100 + (PlayerPrefs.GetInt("PLAYER_ARMOR") * 10) : 100;

        multiplayerHealth.SetMaxArmor(Armor);
    }

    private void InitAvatarSelection()
    {
        // To Retrieve
        if (PlayerPrefs.HasKey("AvatarSelectionNumber"))
        {
            characterInt = PlayerPrefs.GetInt("AvatarSelectionNumber");
            male = characterInt <= 4 ? true : false;
        }
    }

    private void InitBulletModifier()
    {
        bulletModifier = PlayerPrefs.HasKey("BULLET_MODIFIER") && PlayerPrefs.GetInt("BULLET_MODIFIER") >= 1
            ? 1 + PlayerPrefs.GetInt("BULLET_MODIFIER") : 1;
    }

    private void InitMaxAmmo()
    {
        maxAmmo = PlayerPrefs.HasKey("AMMO_OVERLOAD") && PlayerPrefs.GetInt("AMMO_OVERLOAD") >= 1
            ? bulletModifier * 5 : 0;
    }

    private void InitHealthRegen()
    {
        if (PlayerPrefs.HasKey("HEALTH_REGEN") && PlayerPrefs.GetInt("HEALTH_REGEN") >= 1)
        {
            InvokeRepeating("HealthRegen", 0f, 3f);
        }
    }

    private void InitPlayerCints()
    {
        if (PlayerPrefs.HasKey("CINTS") && PlayerPrefs.GetInt("CINTS") >= 1)
        {
            playerCints = PlayerPrefs.GetInt("CINTS");
        }
    }

    private void InitBossesKilled()
    {
        if (PlayerPrefs.HasKey("BossKilled") && PlayerPrefs.GetInt("BossKilled") >= 1)
        {
            bossesKilled = PlayerPrefs.GetInt("BossKilled");
        }
    }

    private void InitIntelGathered()
    {
        if (PlayerPrefs.HasKey("IntelFound") && PlayerPrefs.GetInt("IntelFound") >= 1)
        {
            intelCollected = PlayerPrefs.GetInt("IntelFound");
        }
    }

    private void InitArtifactsFound()
    {
        if (PlayerPrefs.HasKey("ArtifactFound") && PlayerPrefs.GetInt("ArtifactFound") >= 1)
        {
            artifactsRecovered = PlayerPrefs.GetInt("ArtifactFound");
        }
    }

    private void InitBombsDestroyed()
    {
        if (PlayerPrefs.HasKey("BombDestroyed") && PlayerPrefs.GetInt("BombDestroyed") >= 1)
        {
            bombsDestroyed = PlayerPrefs.GetInt("BombDestroyed");
        }
    }

    private void InitCollectorsKilled()
    {
        if (PlayerPrefs.HasKey("CollectorsDestroyed") && PlayerPrefs.GetInt("CollectorsDestroyed") >= 1)
        {
            collectorsDestroyed = PlayerPrefs.GetInt("CollectorsDestroyed");
        }
    }

    private void InitGuardiansKilled()
    {
        if (PlayerPrefs.HasKey("GuardianDestroyed") && PlayerPrefs.GetInt("GuardianDestroyed") >= 1)
        {
            guardiansDestroyed = PlayerPrefs.GetInt("GuardianDestroyed");
        }
    }

    private void InitSavingGrace()
    {
        if (PlayerPrefs.HasKey("SAVING_GRACE") && PlayerPrefs.GetInt("SAVING_GRACE") >= 1)
        {
            playerLives += 1;
        }
    }
    private int SetMaxHealthFromHealthLevel()
    {
        // TODO: Create Formula to improve health upon level up of character. int 10 can be changed. 
        maxHealth = Health;
        return maxHealth;
    }

    private int SetMaxArmorFromArmorLevel()
    {
        // TODO: Create Formula to improve health upon level up of character. int 10 can be changed. 
        maxArmor = Armor;
        return maxArmor;
    }

    // Update is called once per frame
    void Update()
    {
        // Perform time-based actions
        if (reactorHeld)
            UpdateReactor();
        UpdatePowerups();

        // Check win conditions
        if (reactorExtraction >= 100 && !spawnManager.gameOver)
            CheckExtractionWinCondition();

        // Call abilities
        if (toxicEffectActive)
            CheckAbility1();
        //if (shieldActive)
        //    CheckAbility2();
        if (bulletImproved)
            CheckAbility3();
        CheckAbility4();
        CheckAbility5();
        CheckAbility6();
        CheckAbility8();
        CheckAbility9();
        CheckAbility10();

        //Get input device values
        GetPrimaryButtonState();
        GetSecondaryButtonState();

        activateWristUI.BossIcon.GetComponentInChildren<TextMeshProUGUI>().text = bossesKilled.ToString() + "/" + bossesRequired.ToString();
        activateWristUI.CollectorIcon.GetComponentInChildren<TextMeshProUGUI>().text = collectorsDestroyed.ToString() + "/" + collectorsRequired.ToString();
        activateWristUI.GuardianIcon.GetComponentInChildren<TextMeshProUGUI>().text = guardiansDestroyed.ToString() + "/" + guardianRequired.ToString();
        activateWristUI.IntelIcon.GetComponentInChildren<TextMeshProUGUI>().text = intelCollected.ToString() + "/" + intelRequired.ToString();
        activateWristUI.BombIcon.GetComponentInChildren<TextMeshProUGUI>().text = bombsDestroyed.ToString() + "/" + bombsRequired.ToString();
        activateWristUI.ArtifactIcon.GetComponentInChildren<TextMeshProUGUI>().text = artifactsRecovered.ToString() + "/" + artifactsRequired.ToString();

        if (bossesKilled >= bossesRequired)
        {
            if (PlayerPrefs.HasKey("BossQuestCompleted") && PlayerPrefs.GetInt("BossQuestCompleted") == 1)
            {
                PlayerPrefs.SetInt("BossQuestCompleted", 0);

                expReward = PlayerPrefs.GetInt("BossQuestExpTarget");
                GetXP(expReward);
                cintReward = PlayerPrefs.GetInt("BossQuestCintTarget");
                UpdateSkills(cintReward);
            }
        }

        if (collectorsDestroyed >= collectorsRequired)
        {
            if (PlayerPrefs.HasKey("CollectorQuestCompleted") && PlayerPrefs.GetInt("CollectorQuestCompleted") == 1)
            {
                PlayerPrefs.SetInt("CollectorQuestCompleted", 0);

                expReward = PlayerPrefs.GetInt("CollectorQuestExpTarget");
                GetXP(expReward);
                cintReward = PlayerPrefs.GetInt("CollectorQuestCintTarget");
                UpdateSkills(cintReward);
            }
        }

        if (guardiansDestroyed >= guardianRequired)
        {
            if (PlayerPrefs.HasKey("GuardianQuestCompleted") && PlayerPrefs.GetInt("GuardianQuestCompleted") == 1)
            {
                PlayerPrefs.SetInt("GuardianQuestCompleted", 0);

                expReward = PlayerPrefs.GetInt("GuardianQuestExpTarget");
                GetXP(expReward);
                cintReward = PlayerPrefs.GetInt("GuardianQuestCintTarget");
                UpdateSkills(cintReward);
            }
        }

        if (intelCollected >= intelRequired)
        {
            if (PlayerPrefs.HasKey("IntelQuestCompleted") && PlayerPrefs.GetInt("IntelQuestCompleted") == 1)
            {
                PlayerPrefs.SetInt("IntelQuestCompleted", 0);

                expReward = PlayerPrefs.GetInt("IntelQuestExpTarget");
                GetXP(expReward);
                cintReward = PlayerPrefs.GetInt("IntelQuestCintTarget");
                UpdateSkills(cintReward);
            }
        }

        if (bombsDestroyed >= bombsRequired)
        {
            if (PlayerPrefs.HasKey("BombQuestCompleted") && PlayerPrefs.GetInt("BombQuestCompleted") == 1)
            {
                PlayerPrefs.SetInt("BombQuestCompleted", 0);

                expReward = PlayerPrefs.GetInt("BombQuestExpTarget");
                GetXP(expReward);
                cintReward = PlayerPrefs.GetInt("BombQuestCintTarget");
                UpdateSkills(cintReward);
            }
        }

        if (artifactsRecovered >= artifactsRequired)
        {
            if (PlayerPrefs.HasKey("ArtifactQuestCompleted") && PlayerPrefs.GetInt("ArtifactQuestCompleted") == 1)
            {
                PlayerPrefs.SetInt("ArtifactQuestCompleted", 0);

                expReward = PlayerPrefs.GetInt("ArtifactQuestExpTarget");
                GetXP(expReward);
                cintReward = PlayerPrefs.GetInt("CArtifactQuestCintTarget");
                UpdateSkills(cintReward);
            }
        }
    }

    void GetPrimaryButtonState()
    {
        InputDevice primaryImplant = InputDevices.GetDeviceAtXRNode(left_HandButtonSource);
        primaryImplant.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonPressed);

        if (primaryButtonPressed && primaryPowerupTimer)
            PrimaryImplantActivation();
    }

    void GetSecondaryButtonState()
    {
        InputDevice secondaryImplant = InputDevices.GetDeviceAtXRNode(left_HandButtonSource);
        secondaryImplant.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonPressed);

        if (secondaryButtonPressed && secondaryPowerupTimer)
            SecondaryImplantActivation();
    }

    void UpdateReactor()
    {
        reactorTimer += Time.deltaTime;
        if (reactorTimer > 5f)
        {
            StartCoroutine(ReactorExtraction());
        }
    }

    void UpdatePowerups()
    {
        primaryActive.SetActive(hasButtonAssignment && primaryPowerupTimer);
        secondaryActive.SetActive(hasButtonAssignment2 && secondaryPowerupTimer);
    }

    void CheckExtractionWinCondition()
    {
        extractionWinner = true;
        spawnManager.gameOver = true;
        spawnManager.winnerPlayer = this.gameObject;
        UpdateSkills(200);
        ExtractionGame();
    }

    public void DroneKilled(string type, GameObject drone)
    {
        if (type == "Collector" && drone.GetComponent<LootDrone>().attachedCache != null)
        {
            if (PlayerPrefs.HasKey("CollectorQuest") && PlayerPrefs.GetInt("CollectorQuest") == 1)
            {
                collectorsDestroyed++;

                PlayerPrefs.SetInt("CollectorQuestTarget", collectorsDestroyed);
            }
        }
    }

    public void GuardianKilled()
    {
        if (PlayerPrefs.HasKey("GuardianQuest") && PlayerPrefs.GetInt("GuardianQuest") == 1)
        {
            guardiansDestroyed++;

            PlayerPrefs.SetInt("GuardianQuestTarget", guardiansDestroyed);
        }
    }

    public void ArtifactFound()
    {
        if (PlayerPrefs.HasKey("ArtifactQuest") && PlayerPrefs.GetInt("ArtifactQuest") == 1)
        {
            artifactsRecovered++;

            PlayerPrefs.SetInt("ArtifactQuestTarget", artifactsRecovered);
        }
        else
            GetXP(100);
    }

    public void IntelFound()
    {
        if (PlayerPrefs.HasKey("IntelQuest") && PlayerPrefs.GetInt("IntelQuest") == 1)
        {
            intelCollected++;

            PlayerPrefs.SetInt("IntelQuestTarget", intelCollected);
        }
        else
            GetXP(100);
    }

    public void BombNeutralized()
    {
        if (PlayerPrefs.HasKey("BombQuest") && PlayerPrefs.GetInt("BombQuest") == 1)
        {
            bombsDestroyed++;

            PlayerPrefs.SetInt("BombQuestTarget", bombsDestroyed);
        }
        else
            GetXP(100);
    }

    void CheckAbility1()
    {
        if (toxicTimer <= toxicEffectTimer && toxicEffectActive == true && !shouldCallAbilities1True)
        {
            shouldCallAbilities1True = true;
            toxicEffect.SetActive(true);
            toxicTimer += Time.deltaTime;
            shouldCallAbilities1True = false;
        }
        else if (toxicTimer > toxicEffectTimer && toxicEffectActive == true && !shouldCallAbilities1False)
        {
            shouldCallAbilities1False = true;
            toxicEffect.SetActive(false);
            toxicEffectActive = false;
            shouldCallAbilities1False = false;
        }
    }

    void CheckAbility3()
    {
        if (bulletImproved == true && !shouldCallAbilities3True)
        {
            shouldCallAbilities3True = true;
            bulletModifier = startingBulletModifier + bulletXPModifier;
            upgradeTimer += Time.deltaTime;
            shouldCallAbilities3True = false;
        }
        else if (upgradeTimer > bulletXPTimer && bulletImproved == true && !shouldCallAbilities3False)
        {
            shouldCallAbilities3False = true;
            bulletModifier = startingBulletModifier;
            bulletImproved = false;
            shouldCallAbilities3False = false;
        }
    }

    void CheckAbility4()
    {
        if (leechEffect == true && leechEffectTimer <= leechEffectDuration && !shouldCallAbilities4True)
        {
            shouldCallAbilities4True = true;
            leechEffectTimer += Time.deltaTime;
            leechBubble.SetActive(true);
            shouldCallAbilities4True = false;
        }
        else if (leechEffectTimer > leechEffectDuration && !shouldCallAbilities4False && leechEffect == false)
        {
            shouldCallAbilities4False = true;
            leechBubble.SetActive(false);
            leechEffect = false;
            shouldCallAbilities4False = false;
        }
    }

    void CheckAbility5()
    {
        if (activeCamo == true && activeCamoTimer <= activeCamoDuration && !shouldCallAbilities5True)
        {
            shouldCallAbilities5True = true;
            activeCamoTimer += Time.deltaTime;
            foreach (SkinnedMeshRenderer skin in characterSkins)
            {
                skin.enabled = false;
            }
            shouldCallAbilities5True = false;
        }
        else if (activeCamoTimer > activeCamoDuration && !shouldCallAbilities5False || activeCamo == false && !shouldCallAbilities5False)
        {
            shouldCallAbilities5False = true;
            activeCamo = false;
            foreach (SkinnedMeshRenderer skin in characterSkins)
            {
                skin.enabled = true;
            }
            shouldCallAbilities5False = false;
        }
    }

    void CheckAbility6()
    {
        if (stealth == true && stealthTimer <= stealthDuration && !shouldCallAbilities6True)
        {
            shouldCallAbilities6True = true;
            stealthTimer += Time.deltaTime;
            foreach (GameObject minimap in minimapSymbol)
            {
                minimap.SetActive(false);
            }
            shouldCallAbilities6True = false;
        }
        else if (stealthTimer > stealthDuration && !shouldCallAbilities6False || stealth == false && !shouldCallAbilities6False)
        {
            shouldCallAbilities6False = true;
            stealth = false;
            foreach (GameObject minimap in minimapSymbol)
            {
                minimap.SetActive(true);
            }
            shouldCallAbilities6False = false;
        }
    }

    void CheckAbility8()
    {
        if (!shouldCallAbilities8)
        {
            shouldCallAbilities8 = true;
            aiCompanionDrone.SetActive(aiCompanion);
            shouldCallAbilities8 = false;
        }
    }

    void CheckAbility9()
    {
        if (!shouldCallAbilities9)
        {
            shouldCallAbilities9 = true;
            decoySpawner.SetActive(decoyDeploy);
            shouldCallAbilities9 = false;
        }
    }

    void CheckAbility10()
    {
        if (berserker == true && berserkerEffectTimer <= berserkerFuryDuration && !shouldCallAbilities10True)
        {
            shouldCallAbilities10True = true;
            berserkerEffectTimer += Time.deltaTime;
            if (!berserkerActivated)
            {
                berserkerActivated = true;
                Armor = maxArmor;
                Health = maxHealth;
                movement.currentSpeed += 4;
                bulletModifier = startingBulletModifier * 2;
            }
            shouldCallAbilities10True = false;
        }
        else if (berserkerEffectTimer > berserkerFuryDuration && !shouldCallAbilities10False || berserker == false && !shouldCallAbilities10False)
        {
            shouldCallAbilities10False = true;
            if (berserkerActivated)
            {
                berserker = false;
                berserkerActivated = false;
                Health = maxHealth;
                CheckHealthStatus();
                movement.currentSpeed = startingSpeed;
                bulletModifier = startingBulletModifier;
            }
            shouldCallAbilities10False = false;
        }
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
        StartCoroutine(Cracked());

        if (PlayerPrefs.HasKey("DAMAGAE_TAKEN") && PlayerPrefs.GetInt("DAMAGAE_TAKEN") == 1)
            damageTaken = (damage - (PlayerPrefs.GetInt("DAMAGAE_TAKEN") / 4));
        else
            damageTaken = damage;
        if (Armor >= damage)
        {
            Armor -= damage;
            StartCoroutine(ShieldBuffNormal());
        }
        else if (Armor < damage && Armor > 0)
        {
            Health -= (damage - Armor);
            Armor = 0;
            StartCoroutine(ShieldBuffCritical());
        }
        else if (Armor <= 0)
        {
            Health -= damage;
        }

        if (Armor <= 0 && Health <= 0 && playerLives > 1 && alive == true)
        {
            alive = false;
            StartCoroutine(PlayerRespawn());
        }

        else if (Armor <= 0 && Health <= 0 && playerLives == 1 && alive == true)
        {
            alive = false;
            StartCoroutine(PlayerDeath());
        }
        CheckArmorStatus();
        CheckHealthStatus();
    }

    public void AddHealth(int health)
    {
        audioSource.PlayOneShot(bulletHit);

        if (PlayerPrefs.HasKey("HEALTH_POWERUP") && PlayerPrefs.GetInt("HEALTH_POWERUP") == 1)
            healthAdded = (health + PlayerPrefs.GetInt("HEALTH_POWERUP"));
        else
            healthAdded = health;
        Health += health;
        CheckHealthStatus();
    }

    public void CheckHealthStatus()
    {
        multiplayerHealth.SetCurrentHealth(Health);
    }

    public void AddArmor(int armor)
    {
        audioSource.PlayOneShot(bulletHit);

        armorAdded = armor;
        Armor += armor;
        CheckArmorStatus();
    }

    public void CheckArmorStatus()
    {
        multiplayerHealth.SetCurrentArmor(Armor);
    }

    IEnumerator Cracked()
    {
        crackedScreen.SetActive(true);
        yield return new WaitForSeconds(.5f);
        crackedScreen.SetActive(false);
    }

    IEnumerator PlayerDeath()
    {
        yield return new WaitForSeconds(0);
        int cintUpdate = (playerCints / 10);
        StartCoroutine(sceneFader.ScreenFade());
        alive = false;
        GameObject playerDeathTokenObject = this.PoolManager.Acquire(deathToken, tokenDropLocation.position, Quaternion.identity);
        playerDeathTokenObject.GetComponent<playerDeathToken>().tokenValue = cintUpdate;
        playerDeathTokenObject.GetComponent<playerDeathToken>().faction = characterFaction.ToString();

        UpdateSkills(-cintUpdate);

        if (PlayerPrefs.HasKey("EXPLOSIVE_DEATH") && PlayerPrefs.GetInt("EXPLOSIVE_DEATH") >= 1 &&
                PlayerPrefs.HasKey("EXPLOSIVE_DEATH_SLOT") && PlayerPrefs.GetInt("EXPLOSIVE_DEATH_SLOT") >= 1)
        {
            GameObject bomb = this.PoolManager.Acquire(bombDeath, tokenDropLocation.position, Quaternion.identity);
            bomb.GetComponent<Rigidbody>().isKinematic = false;
            bomb.GetComponent<Rigidbody>().useGravity = true;
            NetworkGrenade grenade = bomb.GetComponent<NetworkGrenade>();
            grenade.StartCoroutine(grenade.ExplodeDelayed());
        }
        yield return new WaitForSeconds(.15f);
        // Leave the room
        VirtualWorldManager.Instance.LeaveRoomAndLoadHomeScene();
    }

    IEnumerator PlayerRespawn()
    {
        yield return new WaitForSeconds(0);

        foreach (XRDirectInteractor direct in directInteractors)
        {
            direct.enabled = false;
        }

        StartCoroutine(sceneFader.Respawn());
        model.SetActive(false);
        player.transform.position = spawnManager.respawnPosition;
        playerLives -= 1;
        Armor = 125;
        Health = 125;
        CheckArmorStatus();
        CheckHealthStatus();
        alive = true;
        model.SetActive(true);

        foreach (XRDirectInteractor direct in directInteractors)
        {
            direct.enabled = true;
        }

        respawnUI.UpdateRespawnUI();
    }

    IEnumerator ShieldBuffNormal()
    {
        yield return new WaitForSeconds(0);
        foreach (GameObject shield in shieldObjects)
        {
            shield.SetActive(true);
        }
        yield return new WaitForSeconds(.75f);
        foreach (GameObject shield in shieldObjects)
        {
            shield.SetActive(false);
        }
    }

    IEnumerator ShieldBuffCritical()
    {
        yield return new WaitForSeconds(0);
        foreach (GameObject shield in shieldObjects)
        {
            shield.SetActive(true);
        }
        yield return new WaitForSeconds(.75f);
        foreach (GameObject shield in shieldObjects)
        {
            shield.SetActive(false);
        }
    }

    public void ApplyBlindEffect(float duration)
    {
        StartCoroutine(sceneFader.BlackOut(duration));
    }

    IEnumerator ReactorExtraction()
    {
        yield return new WaitForSeconds(0);

        if (PlayerPrefs.HasKey("REACTOR_EXTRACTION") && PlayerPrefs.GetInt("REACTOR_EXTRACTION") >= 1)
            reactorExtraction += (2 + PlayerPrefs.GetInt("REACTOR_EXTRACTION"));
        else
            reactorExtraction += 2;

        PlayerPrefs.SetInt("ReactorExtraction", reactorExtraction);

        reactorTimer = 0;
    }

    public void EnemyKilled(string type)
    {
        if (type == "Normal")
        {
            enemiesKilled++;

            PlayerPrefs.SetInt("EnemyKills", enemiesKilled);

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
            StartCoroutine(GetXP(5));
        }

        else if (type == "Boss")
        {
            if (PlayerPrefs.HasKey("BossQuest") && PlayerPrefs.GetInt("BossQuest") == 1)
            {
                bossesKilled++;

                PlayerPrefs.SetInt("BossQuestTarget", bossesKilled);
            }
            enemiesKilled++;

            PlayerPrefs.SetInt("EnemyKills", enemiesKilled);

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
            StartCoroutine(GetXP(10));
        }
    }

    public void PlayersKilled()
    {
        playersKilled++;

        PlayerPrefs.SetInt("PlayersKilled", playersKilled);

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
        StartCoroutine(GetXP(15));
    }

    void ExtractionGame()
    {
        StartCoroutine(GetXP(100));

        StartCoroutine(DisplayMessage());
    }

    IEnumerator DisplayMessage()
    {
        yield return new WaitForSeconds(8);
        VirtualWorldManager.Instance.LeaveRoomAndLoadHomeScene();
    }

    public void Toxicity(float toxicTime)
    {
        toxicTimer = 0;
        toxicEffectTimer = toxicTime;
        toxicEffectActive = true;
    }

    public void BulletImprove(float bulletTimer, int newModifier)
    {
        upgradeTimer = 0;
        bulletXPTimer = bulletTimer;
        bulletXPModifier = newModifier;
        startingBulletModifier = bulletModifier;
        bulletImproved = true;
    }

    public void HealthRegen()
    {

        if (Health < maxHealth)
            AddHealth(2);
    }

    public void UpdateSkills(int cintsEarned)
    {
        playerCints += cintsEarned;

        PlayerPrefs.SetInt("CINTS", playerCints);

    }

    IEnumerator PrimaryTimer(float time)
    {
        yield return new WaitForSeconds(time / 2);
        if (PlayerPrefs.HasKey("AI_COMPANION") && PlayerPrefs.GetInt("AI_COMPANION") >= 1 &&
                    PlayerPrefs.HasKey("AI_COMPANION_SLOT") && PlayerPrefs.GetInt("AI_COMPANION_SLOT") == 1)
        {
            aiCompanion = false;
        }

        if (PlayerPrefs.HasKey("DECOY_DEPLOYMENT") && PlayerPrefs.GetInt("DECOY_DEPLOYMENT") >= 1 &&
                    PlayerPrefs.HasKey("DECOY_DEPLOYMENT_SLOT") && PlayerPrefs.GetInt("DECOY_DEPLOYMENT_SLOT") == 1)
        {
            decoyDeploy = false;
        }

        yield return new WaitForSeconds(time / 2);
        primaryPowerupTimer = true;
    }

    IEnumerator SecondaryTimer(float time)
    {
        yield return new WaitForSeconds(time / 2);
        if (PlayerPrefs.HasKey("AI_COMPANION") && PlayerPrefs.GetInt("AI_COMPANION") >= 1 &&
                    PlayerPrefs.HasKey("AI_COMPANION_SLOT") && PlayerPrefs.GetInt("AI_COMPANION_SLOT") == 2)
        {
            aiCompanion = false;
        }

        if (PlayerPrefs.HasKey("DECOY_DEPLOYMENT") && PlayerPrefs.GetInt("DECOY_DEPLOYMENT") >= 1 &&
                    PlayerPrefs.HasKey("DECOY_DEPLOYMENT_SLOT") && PlayerPrefs.GetInt("DECOY_DEPLOYMENT_SLOT") == 2)
        {
            decoyDeploy = false;
        }

        yield return new WaitForSeconds(time / 2);
        secondaryPowerupTimer = true;
    }

    void PrimaryImplantActivation()
    {
        primaryPowerupTimer = false;

        if (PlayerPrefs.HasKey("HEALTH_STIM") && PlayerPrefs.GetInt("HEALTH_STIM") >= 1 &&
                    PlayerPrefs.HasKey("HEALTH_STIM_SLOT") && PlayerPrefs.GetInt("HEALTH_STIM_SLOT") == 1)
        {
            AddHealth(25);
            StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
        }

        if (PlayerPrefs.HasKey("LEECH") && PlayerPrefs.GetInt("LEECH") >= 1 &&
                    PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 1)
        {
            leechEffect = true;
            leechEffectTimer = 0;
            StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
        }

        if (PlayerPrefs.HasKey("ACTIVE_CAMO") && PlayerPrefs.GetInt("ACTIVE_CAMO") >= 1 &&
                    PlayerPrefs.HasKey("ACTIVE_CAMO_SLOT") && PlayerPrefs.GetInt("ACTIVE_CAMO_SLOT") == 1)
        {
            activeCamo = true;
            activeCamoTimer = 0;
            StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
        }

        if (PlayerPrefs.HasKey("STEALTH") && PlayerPrefs.GetInt("STEALTH") >= 1 &&
                    PlayerPrefs.HasKey("STEALTH_SLOT") && PlayerPrefs.GetInt("STEALTH_SLOT") == 1)
        {
            stealth = true;
            stealthTimer = 0;
            StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
        }

        if (PlayerPrefs.HasKey("AI_COMPANION") && PlayerPrefs.GetInt("AI_COMPANION") >= 1 &&
                    PlayerPrefs.HasKey("AI_COMPANION_SLOT") && PlayerPrefs.GetInt("AI_COMPANION_SLOT") == 1)
        {
            aiCompanion = true;
            StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
        }

        if (PlayerPrefs.HasKey("DECOY_DEPLOYMENT") && PlayerPrefs.GetInt("DECOY_DEPLOYMENT") >= 1 &&
                    PlayerPrefs.HasKey("DECOY_DEPLOYMENT_SLOT") && PlayerPrefs.GetInt("DECOY_DEPLOYMENT_SLOT") == 1)
        {
            decoyDeploy = true;
            StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
        }

        if (PlayerPrefs.HasKey("BERSERKER_FURY") && PlayerPrefs.GetInt("BERSERKER_FURY") >= 1 &&
                    PlayerPrefs.HasKey("BERSERKER_FURY_SLOT") && PlayerPrefs.GetInt("BERSERKER_FURY_SLOT") == 1)
        {
            berserker = true;
            berserkerEffectTimer = 0;
            StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
        }
    }

    void SecondaryImplantActivation()
    {
        secondaryPowerupTimer = false;

        if (PlayerPrefs.HasKey("HEALTH_STIM") && PlayerPrefs.GetInt("HEALTH_STIM") >= 1 &&
                    PlayerPrefs.HasKey("HEALTH_STIM_SLOT") && PlayerPrefs.GetInt("HEALTH_STIM_SLOT") == 2)
        {
            AddHealth(25);
            StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
        }

        if (PlayerPrefs.HasKey("LEECH") && PlayerPrefs.GetInt("LEECH") >= 1 &&
                    PlayerPrefs.HasKey("LEECH_SLOT") && PlayerPrefs.GetInt("LEECH_SLOT") == 2)
        {
            leechEffect = true;
            leechEffectTimer = 0;
            StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
        }

        if (PlayerPrefs.HasKey("ACTIVE_CAMO") && PlayerPrefs.GetInt("ACTIVE_CAMO") >= 1 &&
                    PlayerPrefs.HasKey("ACTIVE_CAMO_SLOT") && PlayerPrefs.GetInt("ACTIVE_CAMO_SLOT") == 2)
        {
            activeCamo = true;
            activeCamoTimer = 0;
            StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
        }

        if (PlayerPrefs.HasKey("STEALTH") && PlayerPrefs.GetInt("STEALTH") >= 1 &&
                    PlayerPrefs.HasKey("STEALTH_SLOT") && PlayerPrefs.GetInt("STEALTH_SLOT") == 2)
        {
            stealth = true;
            stealthTimer = 0;
            StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
        }

        if (PlayerPrefs.HasKey("AI_COMPANION") && PlayerPrefs.GetInt("AI_COMPANION") >= 1 &&
                    PlayerPrefs.HasKey("AI_COMPANION_SLOT") && PlayerPrefs.GetInt("AI_COMPANION_SLOT") == 2)
        {
            aiCompanion = true;
            StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
        }

        if (PlayerPrefs.HasKey("DECOY_DEPLOYMENT") && PlayerPrefs.GetInt("DECOY_DEPLOYMENT") >= 1 &&
                    PlayerPrefs.HasKey("DECOY_DEPLOYMENT_SLOT") && PlayerPrefs.GetInt("DECOY_DEPLOYMENT_SLOT") == 2)
        {
            decoyDeploy = true;
            StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
        }

        if (PlayerPrefs.HasKey("BERSERKER_FURY") && PlayerPrefs.GetInt("BERSERKER_FURY") >= 1 &&
                    PlayerPrefs.HasKey("BERSERKER_FURY_SLOT") && PlayerPrefs.GetInt("BERSERKER_FURY_SLOT") == 2)
        {
            berserker = true;
            berserkerEffectTimer = 0;
            StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
        }
    }

    public void EMPShock()
    {
        IEnumerator shock()
        {
            //States preState = currentState;
            activeState = States.Shocked;
            movement.enabled = false;

            // apply damage
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect = this.PoolManager.Acquire(shockEffect, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1);
            this.PoolManager.Release(effect);
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect2 = this.PoolManager.Acquire(shockEffect, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1);
            this.PoolManager.Release(effect2);
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect3 = this.PoolManager.Acquire(shockEffect, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1);
            this.PoolManager.Release(effect3);
            // enable movement

            activeState = States.Normal;
            movement.enabled = true;
        }
        // if already shocked, ignore effects
        if (activeState == States.Shocked) return;
        StartCoroutine(shock());
    }

    public IEnumerator GetXP(int XP)
    {
        yield return new WaitForSeconds(0);
        LootLockerSDKManager.AddPointsToPlayerProgression(progressionKey, (ulong)XP, response =>
        {
        });
    }
}
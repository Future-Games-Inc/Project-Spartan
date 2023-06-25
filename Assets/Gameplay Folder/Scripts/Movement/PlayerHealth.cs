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
    public XRRayInteractor[] rayInteractors;

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
    public GameObject winCanvas;
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
    public GameObject energyBlade;
    public Transform meleeAttach;

    public Transform bombDropLocation;
    public Transform tokenDropLocation;

    public TextMeshProUGUI messageText;
    public TextMeshProUGUI reactorText;

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
    InputDevice primaryImplant;
    InputDevice secondaryImplant;

    [Header("Player Static Byte Data ------------------------------------")]
    public static readonly byte ExtractionGameMode = 1;
    public static readonly byte PlayerGameMode = 2;
    public static readonly byte EnemyGameMode = 3;

    [Header("Player Leaderboard Data ------------------------------------")]
    public int leaderboardID = 10220;

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

    // Start is called before the first frame update
    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
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
        winCanvas.SetActive(false);
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

        if (photonView.IsMine)
        {
            healthBarObject.SetActive(true);
            armorBarObject.SetActive(true);
        }

        CheckHealthStatus();
        CheckArmorStatus();

        GameObject blade = PhotonNetwork.Instantiate(energyBlade.name, meleeAttach.position, transform.rotation);
        blade.GetComponent<EnergyBladeNet>().playerHealth = this.gameObject.GetComponent<PlayerHealth>();
        meleeAttach.GetComponent<SocketedObjectController>().targetSocketedObject = blade;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BUTTON_ASSIGN, out object assignment) && (int)assignment >= 1)
            hasButtonAssignment = true;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BUTTON_ASSIGN, out object assignment2) && (int)assignment2 >= 2)
            hasButtonAssignment2 = true;

        InputDevice primaryImplant = InputDevices.GetDeviceAtXRNode(left_HandButtonSource);
        InputDevice secondaryImplant = InputDevices.GetDeviceAtXRNode(left_HandButtonSource);

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BossQuestTarget, out object storedBossesTarget))
            bossesRequired = (int)storedBossesTarget;
        else
            bossesRequired = 0;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.GuardianQuestTarget, out object storedGuardianTarget))
            guardianRequired = (int)storedGuardianTarget;
        else
            guardianRequired = 0;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CollectorQuestTarget, out object storedCollectorsTarget))
            collectorsRequired = (int)storedCollectorsTarget;
        else
            collectorsRequired = 0;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.IntelQuestTarget, out object storedIntelTarget))
            intelRequired = (int)storedIntelTarget;
        else
            intelRequired = 0;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ArtifactQuestTarget, out object storedArtifactTarget))
            artifactsRequired = (int)storedArtifactTarget;
        else
            artifactsRequired = 0;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BombQuestTarget, out object storedBombTarget))
            bombsRequired = (int)storedBombTarget;
        else
            bombsRequired = 0;

        UpdatePrimaryText();
        UpdateSecondaryText();
    }

    void UpdateSecondaryText()
    {
        object secondaryImplant;
        object secondaryNode;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_STIM, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_STIM_SLOT, out secondaryNode) && (int)secondaryNode == 2)
        {
            secondaryPowerupText.text = "HEALTH STIM";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.LEECH, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.LEECH_SLOT, out secondaryNode) && (int)secondaryNode == 2)
        {
            secondaryPowerupText.text = "LEECH";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ACTIVE_CAMO, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ACTIVE_CAMO_SLOT, out secondaryNode) && (int)secondaryNode == 2)
        {
            secondaryPowerupText.text = "ACTIVE CAMO";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.STEALTH, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.STEALTH_SLOT, out secondaryNode) && (int)secondaryNode == 2)
        {
            secondaryPowerupText.text = "STEALTH";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION_SLOT, out secondaryNode) && (int)secondaryNode == 2)
        {
            secondaryPowerupText.text = "AI COMPANION";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT_SLOT, out secondaryNode) && (int)secondaryNode == 2)
        {
            secondaryPowerupText.text = "DECOY DEPLOYMENT";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BERSERKER_FURY, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BERSERKER_FURY_SLOT, out secondaryNode) && (int)secondaryNode == 2)
        {
            secondaryPowerupText.text = "BERSERKER FURY";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE, out secondaryImplant) && (int)secondaryImplant >= 1 &&
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE_SLOT, out secondaryNode) && (int)secondaryNode == 2)
        {
            secondaryPowerupText.text = "SAVING GRACE APPLIED";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.EXPLOSIVE_DEATH, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.EXPLOSIVE_DEATH_SLOT, out secondaryNode) && (int)secondaryNode == 2)
        {
            secondaryPowerupText.text = "EXPLOSIVE DEATH APPLIED";
        }
    }

    void UpdatePrimaryText()
    {
        object primaryImplant;
        object primaryNode;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_STIM, out primaryImplant) && (int)primaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_STIM_SLOT, out primaryNode) && (int)primaryNode == 1)
        {
            primaryPowerupText.text = "HEALTH STIM";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.LEECH, out primaryImplant) && (int)primaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.LEECH_SLOT, out primaryNode) && (int)primaryNode == 1)
        {
            primaryPowerupText.text = "LEECH";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ACTIVE_CAMO, out primaryImplant) && (int)primaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ACTIVE_CAMO_SLOT, out primaryNode) && (int)primaryNode == 1)
        {
            primaryPowerupText.text = "ACTIVE CAMO";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.STEALTH, out primaryImplant) && (int)primaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.STEALTH_SLOT, out primaryNode) && (int)primaryNode == 1)
        {
            primaryPowerupText.text = "STEALTH";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION, out primaryImplant) && (int)primaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION_SLOT, out primaryNode) && (int)primaryNode == 1)
        {
            primaryPowerupText.text = "AI COMPANION";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT, out primaryImplant) && (int)primaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT_SLOT, out primaryNode) && (int)primaryNode == 1)
        {
            primaryPowerupText.text = "DECOY DEPLOYMENT";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BERSERKER_FURY, out primaryImplant) && (int)primaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BERSERKER_FURY_SLOT, out primaryNode) && (int)primaryNode == 1)
        {
            primaryPowerupText.text = "BERSERKER FURY";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE, out primaryImplant) && (int)primaryImplant >= 1 &&
        PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE_SLOT, out primaryNode) && (int)primaryNode == 1)
        {
            primaryPowerupText.text = "SAVING GRACE APPLIED";
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.EXPLOSIVE_DEATH, out primaryImplant) && (int)primaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.EXPLOSIVE_DEATH_SLOT, out primaryNode) && (int)primaryNode == 1)
        {
            primaryPowerupText.text = "EXPLOSIVE DEATH APPLIED";
        }
    }

    private void InitHealth()
    {
        Health = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.PLAYER_HEALTH, out object storedPlayerHealth) && (int)storedPlayerHealth >= 1
            ? 100 + ((int)storedPlayerHealth * 10) : 100;

        multiplayerHealth.SetMaxHealth(Health);
    }

    private void InitArmor()
    {
        Armor = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.PLAYER_ARMOR, out object storedPlayerArmor) && (int)storedPlayerArmor >= 1
            ? 100 + ((int)storedPlayerArmor * 10) : 100;

        multiplayerHealth.SetMaxArmor(Armor);
    }

    private void InitAvatarSelection()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AVATAR_SELECTION_NUMBER, out object avatarSelectionNumber))
        {
            characterInt = (int)avatarSelectionNumber;
            male = characterInt <= 4 ? true : false;
        }
    }

    private void InitBulletModifier()
    {
        bulletModifier = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BULLET_MODIFIER, out object storedBulletModifier) && (int)storedBulletModifier >= 1
            ? 1 + (int)storedBulletModifier : 1;
    }

    private void InitMaxAmmo()
    {
        maxAmmo = PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AMMO_OVERLOAD, out object storedAmmoOverload) && (int)storedAmmoOverload >= 1
            ? bulletModifier * 5 : 0;
    }

    private void InitHealthRegen()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_REGEN, out object storedHealthRegen) && (int)storedHealthRegen >= 1)
        {
            photonView.RPC("RPC_HealthRegen", RpcTarget.All);
        }
    }

    private void InitPlayerCints()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTS, out object storedPlayerCints) && (int)storedPlayerCints >= 1)
        {
            playerCints = (int)storedPlayerCints;
        }
    }

    private void InitBossesKilled()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BossKilled, out object storedBossesKilled) && (int)storedBossesKilled >= 1)
        {
            bossesKilled = (int)storedBossesKilled;
        }
    }

    private void InitIntelGathered()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.IntelFound, out object storedIntelFound) && (int)storedIntelFound >= 1)
        {
            intelCollected = (int)storedIntelFound;
        }
    }

    private void InitArtifactsFound()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ArtifactFound, out object storedArtifactsFound) && (int)storedArtifactsFound >= 1)
        {
            artifactsRecovered = (int)storedArtifactsFound;
        }
    }

    private void InitBombsDestroyed()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BombDestroyed, out object storedBombsDestroyed) && (int)storedBombsDestroyed >= 1)
        {
            bombsDestroyed = (int)storedBombsDestroyed;
        }
    }

    private void InitCollectorsKilled()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CollectorDestroyed, out object storedCollectorsKilled) && (int)storedCollectorsKilled >= 1)
        {
            collectorsDestroyed = (int)storedCollectorsKilled;
        }
    }

    private void InitGuardiansKilled()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.GuardianDestroyed, out object storedGuardiansKilled) && (int)storedGuardiansKilled >= 1)
        {
            guardiansDestroyed = (int)storedGuardiansKilled;
        }
    }

    private void InitSavingGrace()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE, out object storedSavingGrace) && (int)storedSavingGrace >= 1)
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
    [System.Obsolete]
    void Update()
    {
        if(Health > maxHealth)
        {
            Health = maxHealth;
            CheckHealthStatus();
        }
        // Perform time-based actions
        if (reactorHeld)
            UpdateReactor();
        else if (!reactorHeld)
            reactorText.enabled = false;
        UpdatePowerups();

        // Check win conditions
        if (reactorExtraction >= 100 && !spawnManager.gameOver)
            CheckExtractionWinCondition();
        if (playersKilled >= 15 && !spawnManager.gameOver)
            CheckPlayerWinCondition();
        if (enemiesKilled >= 25 && !spawnManager.gameOver)
            CheckEnemyWinCondition();

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
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BossQuestCompleted, out object contractState) && (bool)contractState == true)
            {
                contractState = false;

                ExitGames.Client.Photon.Hashtable bossContract = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BossQuest, contractState } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(bossContract);

                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BossQuestExpTarget, out object storedBossesTargetEXP))
                {
                    expReward = (int)storedBossesTargetEXP;
                    GetXP(expReward);
                }
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BossQuestCintTarget, out object storedBossesTargetCint))
                {
                    cintReward = (int)storedBossesTargetCint;
                    UpdateSkills(cintReward);
                }
            }
        }

        if (collectorsDestroyed >= collectorsRequired)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CollectorQuest, out object contractState) && (bool)contractState == true)
            {
                contractState = false;

                ExitGames.Client.Photon.Hashtable Contract = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CollectorQuest, contractState } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(Contract);

                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CollectorQuestExpTarget, out object storedTargetEXP))
                {
                    expReward = (int)storedTargetEXP;
                    GetXP(expReward);
                }
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CollectorQuestCintTarget, out object storedTargetCint))
                {
                    cintReward = (int)storedTargetCint;
                    UpdateSkills(cintReward);
                }
            }
        }

        if (guardiansDestroyed >= guardianRequired)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.GuardianQuest, out object contractState) && (bool)contractState == true)
            {
                contractState = false;

                ExitGames.Client.Photon.Hashtable Contract = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.GuardianQuest, contractState } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(Contract);

                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.GuardianQuestExpTarget, out object storedTargetEXP))
                {
                    expReward = (int)storedTargetEXP;
                    GetXP(expReward);
                }
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.GuardianQuestCintTarget, out object storedTargetCint))
                {
                    cintReward = (int)storedTargetCint;
                    UpdateSkills(cintReward);
                }
            }
        }

        if (intelCollected >= intelRequired)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.IntelQuest, out object contractState) && (bool)contractState == true)
            {
                contractState = false;

                ExitGames.Client.Photon.Hashtable Contract = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.IntelQuest, contractState } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(Contract);

                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.IntelQuestExpTarget, out object storedTargetEXP))
                {
                    expReward = (int)storedTargetEXP;
                    GetXP(expReward);
                }
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.IntelQuestCintTarget, out object storedTargetCint))
                {
                    cintReward = (int)storedTargetCint;
                    UpdateSkills(cintReward);
                }
            }
        }

        if (bombsDestroyed >= bombsRequired)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BombQuest, out object contractState) && (bool)contractState == true)
            {
                contractState = false;

                ExitGames.Client.Photon.Hashtable Contract = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BombQuest, contractState } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(Contract);

                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BombQuestExpTarget, out object storedTargetEXP))
                {
                    expReward = (int)storedTargetEXP;
                    GetXP(expReward);
                }
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BombQuestCintTarget, out object storedTargetCint))
                {
                    cintReward = (int)storedTargetCint;
                    UpdateSkills(cintReward);
                }
            }
        }

        if (artifactsRecovered >= artifactsRequired)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ArtifactQuest, out object contractState) && (bool)contractState == true)
            {
                contractState = false;

                ExitGames.Client.Photon.Hashtable Contract = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.ArtifactQuest, contractState } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(Contract);

                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ArtifactQuestExpTarget, out object storedTargetEXP))
                {
                    expReward = (int)storedTargetEXP;
                    GetXP(expReward);
                }
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ArtifactQuestCintTarget, out object storedTargetCint))
                {
                    cintReward = (int)storedTargetCint;
                    UpdateSkills(cintReward);
                }
            }
        }
    }

    void GetPrimaryButtonState()
    {
        primaryImplant.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButtonPressed);
    }

    void GetSecondaryButtonState()
    {
        secondaryImplant.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonPressed);
    }

    void UpdateReactor()
    {
        reactorText.enabled = true;
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
        photonView.RPC("RPC_SpawnManagerTrue", RpcTarget.All);
        StartCoroutine(WinMessage("200 skill points awarded for winning the round"));
        UpdateSkills(200);
        ExtractionGame();
    }

    public void DroneKilled(string type, GameObject drone)
    {
        if (type == "Collector" && drone.GetComponent<LootDrone>().attachedCache != null)
        {
            object questActive;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CollectorQuest, out questActive) && (bool)questActive == true)
            {
                collectorsDestroyed++;

                ExitGames.Client.Photon.Hashtable questUpdate = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CollectorQuest, collectorsDestroyed } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(questUpdate);
            }
        }
    }

    public void GuardianKilled()
    {
        object questActive;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.GuardianQuest, out questActive) && (bool)questActive == true)
        {
            guardiansDestroyed++;

            ExitGames.Client.Photon.Hashtable questUpdate = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.GuardianQuest, guardiansDestroyed } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(questUpdate);
        }
    }

    public void ArtifactFound()
    {
        object questActive;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ArtifactQuest, out questActive) && (bool)questActive == true)
        {
            artifactsRecovered++;

            ExitGames.Client.Photon.Hashtable questUpdate = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.ArtifactQuest, artifactsRecovered } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(questUpdate);
        }
        else
            GetXP(100);
    }

    public void IntelFound()
    {
        object questActive;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.IntelQuest, out questActive) && (bool)questActive == true)
        {
            intelCollected++;

            ExitGames.Client.Photon.Hashtable questUpdate = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.IntelQuest, intelCollected } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(questUpdate);
        }
        else
            GetXP(100);
    }

    public void BombNeutralized()
    {
        object questActive;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BombQuest, out questActive) && (bool)questActive == true)
        {
            bombsDestroyed++;

            ExitGames.Client.Photon.Hashtable questUpdate = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BombQuest, bombsDestroyed } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(questUpdate);
        }
        else
            GetXP(100);
    }

    void CheckPlayerWinCondition()
    {
        playerWinner = true;
        photonView.RPC("RPC_SpawnManagerTrue", RpcTarget.All);
        StartCoroutine(WinMessage("250 skill points awarded for winning the round"));
        UpdateSkills(250);
        PlayerGame();
    }

    void CheckEnemyWinCondition()
    {
        enemyWinner = true;
        photonView.RPC("RPC_SpawnManagerTrue", RpcTarget.All);
        StartCoroutine(WinMessage("150 skill points awarded for winning the round"));
        UpdateSkills(150);
        EnemyGame();
    }

    void CheckAbility1()
    {
        if (toxicTimer <= toxicEffectTimer && toxicEffectActive == true && !shouldCallAbilities1True)
        {
            shouldCallAbilities1True = true;
            photonView.RPC("RPC_Abilities1True", RpcTarget.All);
            shouldCallAbilities1True = false;
        }
        else if (toxicTimer > toxicEffectTimer && toxicEffectActive == true && !shouldCallAbilities1False)
        {
            shouldCallAbilities1False = true;
            photonView.RPC("RPC_Abilities1False", RpcTarget.All);
            shouldCallAbilities1False = false;
        }
    }

    void CheckAbility3()
    {
        if (bulletImproved == true && !shouldCallAbilities3True)
        {
            shouldCallAbilities3True = true;
            photonView.RPC("RPC_Abilities3True", RpcTarget.All);
            shouldCallAbilities3True = false;
        }
        else if (upgradeTimer > bulletXPTimer && bulletImproved == true && !shouldCallAbilities3False)
        {
            shouldCallAbilities3False = true;
            photonView.RPC("RPC_Abilities3False", RpcTarget.All);
            shouldCallAbilities3False = false;
        }
    }

    void CheckAbility4()
    {
        if (leechEffect == true && leechEffectTimer <= leechEffectDuration && !shouldCallAbilities4True)
        {
            shouldCallAbilities4True = true;
            photonView.RPC("RPC_Abilities4True", RpcTarget.All);
            shouldCallAbilities4True = false;
        }
        else if (leechEffectTimer > leechEffectDuration && !shouldCallAbilities4False && leechEffect == false)
        {
            shouldCallAbilities4False = true;
            photonView.RPC("RPC_Abilities4False", RpcTarget.All);
            shouldCallAbilities4False = false;
        }
    }

    void CheckAbility5()
    {
        if (activeCamo == true && activeCamoTimer <= activeCamoDuration && !shouldCallAbilities5True)
        {
            shouldCallAbilities5True = true;
            photonView.RPC("RPC_Abilities5True", RpcTarget.All);
            shouldCallAbilities5True = false;
        }
        else if (activeCamoTimer > activeCamoDuration && !shouldCallAbilities5False || activeCamo == false && !shouldCallAbilities5False)
        {
            shouldCallAbilities5False = true;
            photonView.RPC("RPC_Abilities5False", RpcTarget.All);
            shouldCallAbilities5False = false;
        }
    }

    void CheckAbility6()
    {
        if (stealth == true && stealthTimer <= stealthDuration && !shouldCallAbilities6True)
        {
            shouldCallAbilities6True = true;
            photonView.RPC("RPC_Abilities6True", RpcTarget.All);
            shouldCallAbilities6True = false;
        }
        else if (stealthTimer > stealthDuration && !shouldCallAbilities6False || stealth == false && !shouldCallAbilities6False)
        {
            shouldCallAbilities6False = true;
            photonView.RPC("RPC_Abilities6False", RpcTarget.All);
            shouldCallAbilities6False = false;
        }
    }

    void CheckAbility8()
    {
        if (!shouldCallAbilities8)
        {
            shouldCallAbilities8 = true;
            photonView.RPC("RPC_Abilities8", RpcTarget.All);
            shouldCallAbilities8 = false;
        }
    }

    void CheckAbility9()
    {
        if (!shouldCallAbilities9)
        {
            shouldCallAbilities9 = true;
            photonView.RPC("RPC_Abilities9", RpcTarget.All);
            shouldCallAbilities9 = false;
        }
    }

    void CheckAbility10()
    {
        if (berserker == true && berserkerEffectTimer <= berserkerFuryDuration && !shouldCallAbilities10True)
        {
            shouldCallAbilities10True = true;
            photonView.RPC("RPC_Abilities10True", RpcTarget.All);
            shouldCallAbilities10True = false;
        }
        else if (berserkerEffectTimer > berserkerFuryDuration && !shouldCallAbilities10False || berserker == false && !shouldCallAbilities10False)
        {
            shouldCallAbilities10False = true;
            photonView.RPC("RPC_Abilities10False", RpcTarget.All);
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

        object storedDamageTaken;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DAMAGAE_TAKEN, out storedDamageTaken) && (int)storedDamageTaken >= 1)
            damageTaken = (damage - ((int)storedDamageTaken / 4));
        else
            damageTaken = damage;
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damageTaken);
        CheckArmorStatus();
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
        photonView.RPC("RPC_GainHealth", RpcTarget.All, healthAdded);
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
        photonView.RPC("RPC_GainArmor", RpcTarget.All, armorAdded);
        CheckArmorStatus();
    }

    public void CheckArmorStatus()
    {
        multiplayerHealth.SetCurrentArmor(Armor);
    }

    IEnumerator Cracked()
    {
        yield return new WaitForSeconds(0);
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
        GameObject playerDeathTokenObject = PhotonNetwork.InstantiateRoomObject(deathToken.name, tokenDropLocation.position, Quaternion.identity, 0, null);
        playerDeathTokenObject.GetComponent<playerDeathToken>().tokenValue = cintUpdate;
        playerDeathTokenObject.GetComponent<playerDeathToken>().faction = characterFaction.ToString();

        UpdateSkills(-cintUpdate);

        object implant;
        object node;

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.EXPLOSIVE_DEATH, out implant) && (int)implant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.EXPLOSIVE_DEATH_SLOT, out node) && (int)node >= 1)
        {
            GameObject bomb = PhotonNetwork.InstantiateRoomObject(bombDeath.name, tokenDropLocation.position, Quaternion.identity, 0, null);
            bomb.GetComponent<Rigidbody>().isKinematic = false;
            bomb.GetComponent<Rigidbody>().useGravity = true;
            NetworkGrenade grenade = bomb.GetComponent<NetworkGrenade>();
            grenade.StartCoroutine(grenade.ExplodeDelayed());
        }
        yield return new WaitForSeconds(.15f);
        if (photonView.IsMine)
        {
            // Leave the room
            VirtualWorldManager.Instance.LeaveRoomAndLoadHomeScene();
        }
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
        photonView.RPC("RPC_Respawn", RpcTarget.All);

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

    IEnumerator ShieldBuffNormal()
    {
        yield return new WaitForSeconds(0);
        photonView.RPC("RPC_ShieldNomral", RpcTarget.All, true);
        yield return new WaitForSeconds(.75f);
        photonView.RPC("RPC_ShieldNomral", RpcTarget.All, false);
    }

    IEnumerator ShieldBuffCritical()
    {
        yield return new WaitForSeconds(0);
        photonView.RPC("RPC_ShieldCritical", RpcTarget.All, true);
        yield return new WaitForSeconds(.75f);
        photonView.RPC("RPC_ShieldCritical", RpcTarget.All, false);
    }

    public void ApplyBlindEffect(float duration)
    {
        StartCoroutine(sceneFader.BlackOut(duration));
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

        reactorTimer = 0;
    }

    [System.Obsolete]
    public void EnemyKilled(string type)
    {
        if (type == "Normal")
        {
            enemiesKilled++;

            Hashtable hash = new Hashtable();
            hash.Add("enemyKills", enemiesKilled);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

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
            StartCoroutine(GetXP(2));
        }

        else if (type == "Boss")
        {
            object questActive;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BossQuest, out questActive) && (bool)questActive == true)
            {
                bossesKilled++;

                ExitGames.Client.Photon.Hashtable bossUpdate = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.BossKilled, bossesKilled } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(bossUpdate);
            }
            enemiesKilled++;

            Hashtable hash = new Hashtable();
            hash.Add("enemyKills", enemiesKilled);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

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
            StartCoroutine(GetXP(2));
        }
    }

    [System.Obsolete]
    public void PlayersKilled()
    {
        playersKilled++;

        Hashtable hash = new Hashtable();
        hash.Add("playerKills", playersKilled);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

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

    void ExtractionGame()
    {
        StartCoroutine(GetXP(50));

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(ExtractionGameMode, null, raiseEventOptions, sendOptions);
    }

    void PlayerGame()
    {
        StartCoroutine(GetXP(30));

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(PlayerGameMode, null, raiseEventOptions, sendOptions);
    }

    void EnemyGame()
    {
        StartCoroutine(GetXP(10));

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(EnemyGameMode, null, raiseEventOptions, sendOptions);
    }

    IEnumerator DisplayMessage(string message)
    {
        yield return new WaitForSeconds(3);
        winCanvas.SetActive(true);
        messageText.text = message;
        yield return new WaitForSeconds(5);
        messageText.text = "";
        VirtualWorldManager.Instance.LeaveRoomAndLoadHomeScene();
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

        if (photonEvent.Code == ReactorGrab.ReactorExtractionFalse)
        {
            reactorIcon.SetActive(false);

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

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
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

        ExitGames.Client.Photon.Hashtable cintsUpdate = new ExitGames.Client.Photon.Hashtable() { { MultiplayerVRConstants.CINTS, playerCints } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(cintsUpdate);
    }

    IEnumerator PrimaryTimer(float time)
    {
        yield return new WaitForSeconds(time);
        primaryPowerupTimer = true;

        object primaryImplant;
        object primaryNode;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION_SLOT, out primaryNode) && (int)primaryNode == 1)
        {
            aiCompanion = false;
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT, out primaryImplant) && (int)primaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT_SLOT, out primaryNode) && (int)primaryNode == 1)
        {
            decoyDeploy = false;
        }
    }

    IEnumerator SecondaryTimer(float time)
    {
        yield return new WaitForSeconds(time);
        secondaryPowerupTimer = true;

        object secondaryImplant;
        object secondaryNode;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION_SLOT, out secondaryNode) && (int)secondaryNode == 2)
        {
            aiCompanion = false;
        }

        else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT_SLOT, out secondaryNode) && (int)secondaryNode == 2)
        {
            decoyDeploy = false;
        }
    }

    void PrimaryImplantActivation()
    {
        object primaryImplant;
        object primaryNode;

        if (primaryButtonPressed && primaryPowerupTimer == true && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE, out primaryImplant) && (int)primaryImplant !>= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE_SLOT, out primaryNode) && (int)primaryNode != 1 || primaryButtonPressed && primaryPowerupTimer == true && 
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.EXPLOSIVE_DEATH, out primaryImplant) && (int)primaryImplant !>= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.EXPLOSIVE_DEATH_SLOT, out primaryNode) && (int)primaryNode != 1)
        {
            primaryPowerupTimer = false;

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
                StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ACTIVE_CAMO, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ACTIVE_CAMO_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                activeCamo = true;
                activeCamoTimer = 0;
                StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.STEALTH, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.STEALTH_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                stealth = true;
                stealthTimer = 0;
                StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                aiCompanion = true;
                StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                decoyDeploy = true;
                StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BERSERKER_FURY, out primaryImplant) && (int)primaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BERSERKER_FURY_SLOT, out primaryNode) && (int)primaryNode == 1)
            {
                berserker = true;
                berserkerEffectTimer = 0;
                StartCoroutine(PrimaryTimer(primaryPowerupEffectTimer));
            }
        }
        else
            return;
    }

    void SecondayImplantActivation()
    {
        object secondaryImplant;
        object secondaryNode;

        if (secondaryButtonPressed && secondaryPowerupTimer == true && PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE, out secondaryImplant) && (int)secondaryImplant! >= 2 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.SAVING_GRACE_SLOT, out secondaryNode) && (int)secondaryNode != 2 || primaryButtonPressed && primaryPowerupTimer == true &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.EXPLOSIVE_DEATH, out secondaryImplant) && (int)secondaryImplant! >= 2 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.EXPLOSIVE_DEATH_SLOT, out secondaryNode) && (int)secondaryNode != 2)
        {
            secondaryPowerupTimer = false;

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
                StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ACTIVE_CAMO, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.ACTIVE_CAMO_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                activeCamo = true;
                activeCamoTimer = 0;
                StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.STEALTH, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.STEALTH_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                stealth = true;
                stealthTimer = 0;
                StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.AI_COMPANION_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                aiCompanion = true;
                StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DECOY_DEPLOYMENT_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                decoyDeploy = true;
                StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
            }

            else if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BERSERKER_FURY, out secondaryImplant) && (int)secondaryImplant >= 1 &&
                    PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.BERSERKER_FURY_SLOT, out secondaryNode) && (int)secondaryNode == 2)
            {
                berserker = true;
                berserkerEffectTimer = 0;
                StartCoroutine(SecondaryTimer(secondaryPowerupEffectTimer));
            }
        }
        else
            return;
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
            GameObject effect = PhotonNetwork.InstantiateRoomObject(shockEffect.name, transform.position, Quaternion.identity, 0, null);
            yield return new WaitForSeconds(1);
            PhotonNetwork.Destroy(effect);
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect2 = PhotonNetwork.InstantiateRoomObject(shockEffect.name, transform.position, Quaternion.identity, 0, null);
            yield return new WaitForSeconds(1);
            PhotonNetwork.Destroy(effect2);
            yield return new WaitForSeconds(1);
            TakeDamage(5);
            // play shock effect
            GameObject effect3 = PhotonNetwork.InstantiateRoomObject(shockEffect.name, transform.position, Quaternion.identity, 0, null);
            yield return new WaitForSeconds(1);
            PhotonNetwork.Destroy(effect3);
            // enable movement

            activeState = States.Normal;
            movement.enabled = true;
        }
        // if already shocked, ignore effects
        if (activeState == States.Shocked) return;
        StartCoroutine(shock());
    }

    public void FactionDataCard()
    {
        datacards++;
    }

    public IEnumerator GetXP(int XP)
    {
        yield return new WaitForSeconds(0);
        LootLockerSDKManager.SubmitXp((XP), (response) =>
        {
        });
    }

    [PunRPC]
    void RPC_ShieldNomral(bool state)
    {
        if (!photonView.IsMine)
        { return; }

        foreach (GameObject shield in shieldObjects)
        {
            shield.SetActive(state);
        }
    }


    [PunRPC]
    void RPC_ShieldCritical(bool state)
    {
        if (!photonView.IsMine)
        { return; }

        foreach (GameObject shield in shieldObjects)
        {
            shield.SetActive(state);
        }
    }

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        if (!photonView.IsMine)
        { return; }

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
    }

    [PunRPC]
    void RPC_GainHealth(int health)
    {
        if (!photonView.IsMine)
        { return; }

        Health += health;
    }


    [PunRPC]
    void RPC_GainArmor(int armor)
    {
        if (!photonView.IsMine)
        { return; }

        Armor += armor;
    }

    [PunRPC]
    void RPC_Respawn()
    {
        if (!photonView.IsMine)
        { return; }

        model.SetActive(false);
        player.transform.position = spawnManager.spawnPosition[Random.Range(0, spawnManager.spawnPosition.Length)].position;
        playerLives -= 1;
        Armor = 125;
        Health = 125;
        CheckArmorStatus();
        CheckHealthStatus();
        alive = true;
        model.SetActive(true);
    }

    [PunRPC]
    void RPC_SetMaxHealth(int Health)
    {
        if (!photonView.IsMine)
        { return; }

        maxHealth = Health;
        multiplayerHealth.SetMaxHealth(maxHealth);
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
    void RPC_HealthRegen()
    {
        if (!photonView.IsMine)
        { return; }

        InvokeRepeating("HealthRegen", 0f, 3f);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // Check if this is the object's current owner and if the new master client exists
        if (photonView.IsMine && newMasterClient != null)
        {
            // Transfer ownership of the object to the new master client
            photonView.TransferOwnership(newMasterClient.ActorNumber);
        }
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
    void RPC_Abilities8()
    {
        if (!photonView.IsMine)
        { return; }

        aiCompanionDrone.SetActive(aiCompanion);
    }

    [PunRPC]
    void RPC_Abilities9()
    {
        if (!photonView.IsMine)
        { return; }

        decoySpawner.SetActive(decoyDeploy);
    }


    [PunRPC]
    void RPC_Abilities10True()
    {
        if (!photonView.IsMine)
        { return; }

        berserkerEffectTimer += Time.deltaTime;
        if(!berserkerActivated)
        {
            berserkerActivated = true;
            Armor = maxArmor;
            Health = maxHealth;
            movement.currentSpeed = movement.maxSpeed;
            bulletModifier = startingBulletModifier * 2;
        }
    }

    [PunRPC]
    void RPC_Abilities10False()
    {
        if (!photonView.IsMine)
        { return; }

        if (berserkerActivated)
        {
            berserker = false;
            berserkerActivated = false;
            Health = maxHealth;
            CheckHealthStatus();
            movement.currentSpeed = startingSpeed;
            bulletModifier = startingBulletModifier;
        }
    }
}
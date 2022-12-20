using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor.Rendering.Universal;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public GameObject toxicEffect;
    public GameObject bubbleShield;
    public SpawnManager spawnManager;
    public SceneFader sceneFader;
    public XRDirectInteractor[] directInteractors;
    public XRRayInteractor[] rayInteractors;
    public GameObject deathToken;

    public int Health = 100;
    public int reactorExtraction;
    public float reactorTimer = 0;
    public float toxicTimer = 0;
    public float upgradeTimer = 0;
    public float shieldTimer = 0;
    public float toxicEffectTimer;
    public float bulletXPTimer;
    public float shieldEffectTimer;

    public int playerLives = 3;
    public int playersKilled;
    public int enemiesKilled;
    public int startingBulletModifier;
    public int playerCints;

    public bool alive;
    public bool reactorHeld;
    public bool extractionWinner;
    public bool playerWinner;
    public bool enemyWinner;
    public bool toxicEffectActive;
    public bool bulletImproved;
    public bool shieldActive;

    public int maxHealth;
    public int currentHelath;
    public int healthLevel;
    public int bulletModifier;
    public int bulletXPModifier;
    public int maxAmmo;

    public GameObject winCanvas;
    public TextMeshProUGUI messageText;

    public AudioSource audioSource;
    public AudioClip bulletHit;
    public AudioClip xpClip;

    public TextMeshProUGUI reactorText;

    public MultiplayerHealth multiplayerHealth;
    public ReactorUI reactorUI;
    public RespawnUI respawnUI;
    public EnemyKillUI enemyKillUI;
    public PlayerKillUI playerKillUI;

    public static readonly byte ExtractionGameMode = 1;
    public static readonly byte PlayerGameMode = 2;
    public static readonly byte EnemyGameMode = 3;
    // Start is called before the first frame update
    void Start()
    {
        spawnManager = GameObject.FindGameObjectWithTag("playerSpawnManager").GetComponent<SpawnManager>();
        playerLives = 3;

        object storedPlayerHealth;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.PLAYER_HEALTH, out storedPlayerHealth) && (int)storedPlayerHealth >= 1)
            Health = 100 + ((int)storedPlayerHealth * 10);
        else
            Health = 100;

        reactorExtraction = 0;
        playersKilled = 0;
        enemiesKilled = 0;
        alive = true;
        extractionWinner = false;
        playerWinner = false;
        enemyWinner = false;
        toxicEffectActive = false;
        winCanvas.SetActive(false);
        maxHealth = SetMaxHealthFromHealthLevel();
        multiplayerHealth.SetMaxHealth(maxHealth);

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
            maxAmmo = (25 + ((int)storedBulletModifier * 25));
        }
        else
        {
            maxAmmo = 25;
        }

        object storedHealthRegen;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_REGEN, out storedHealthRegen) && (int)storedHealthRegen >= 1)
            InvokeRepeating("HealthRegen", 10, 5);

        object cints;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.CINTS, out cints))
            playerCints = (int)cints;
    }

    private int SetMaxHealthFromHealthLevel()
    {
        // TODO: Create Formula to improve health upon level up of character. int 10 can be changed. 
        maxHealth = Health;
        return maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0 && playerLives > 1 && alive == true)
        {
            alive = false;
            StartCoroutine(PlayerRespawn());
        }

        if (Health <= 0 && playerLives == 1 && alive == true)
        {
            alive = false;
            StartCoroutine(PlayerDeath());
        }

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
            spawnManager.gameOver = true;
            spawnManager.winnerPlayer = this.gameObject;
            StartCoroutine(WinMessage("200 skill points awarded for winning the round"));
            UpdateSkills(250);
            ExtractionGame();
        }

        if (playersKilled >= 10 && spawnManager.gameOver == false)
        {
            playerWinner = true;
            spawnManager.gameOver = true;
            spawnManager.winnerPlayer = this.gameObject;
            StartCoroutine(WinMessage("250 skill points awarded for winning the round"));
            UpdateSkills(250);
            PlayerGame();
        }

        if (enemiesKilled >= 25 && spawnManager.gameOver == false)
        {
            enemyWinner = true;
            spawnManager.gameOver = true;
            spawnManager.winnerPlayer = this.gameObject;
            StartCoroutine(WinMessage("150 skill points awarded for winning the round"));
            UpdateSkills(250);
            EnemyGame();
        }

        if (toxicTimer <= toxicEffectTimer && toxicEffectActive == true)
        {
            toxicEffect.SetActive(true);
            toxicTimer += Time.deltaTime;
        }
        if (toxicTimer > toxicEffectTimer && toxicEffectActive == true)
        {
            toxicEffect.SetActive(false);
            toxicEffectActive = false;
        }

        if (shieldTimer <= shieldEffectTimer && shieldActive == true)
        {
            bubbleShield.SetActive(true);
            shieldTimer += Time.deltaTime;
        }
        if (shieldTimer > shieldEffectTimer && shieldActive == true)
        {
            bubbleShield.SetActive(false);
            shieldActive = false;
        }

        if (bulletImproved == true)
        {
            bulletModifier = startingBulletModifier + bulletXPModifier;
            upgradeTimer += Time.deltaTime;
        }
        if (upgradeTimer > bulletXPTimer && bulletImproved == true)
        {
            bulletModifier = startingBulletModifier;
            bulletImproved = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("XP") || other.CompareTag("Health") || other.CompareTag("ExtraXP") || other.CompareTag("MinorHealth") || other.CompareTag("toxicDropNormal") || other.CompareTag("toxicDropExtra") || other.CompareTag("bulletModifierNormal")
            || other.CompareTag("bulletModifierExtra") || other.CompareTag("MPShield"))
        {
            audioSource.PlayOneShot(xpClip);
        }
    }

    public void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(bulletHit);

        object storedDamageTaken;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.DAMAGAE_TAKEN, out storedDamageTaken) && (int)storedDamageTaken >= 1)
            Health -= (damage - ((int)storedDamageTaken / 4));
        else
            Health -= damage;
        CheckHealthStatus();
    }

    public void AddHealth(int health)
    {
        audioSource.PlayOneShot(bulletHit);

        object storedHealthPowerup;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerVRConstants.HEALTH_POWERUP, out storedHealthPowerup) && (int)storedHealthPowerup >= 1)
            Health += (health + (int)storedHealthPowerup);
        else
            Health += health;
        CheckHealthStatus();
    }

    public void CheckHealthStatus()
    {
        multiplayerHealth.SetCurrentHealth(Health);
    }

    IEnumerator PlayerDeath()
    {
        yield return new WaitForSeconds(0);
        sceneFader.ScreenFade();
        var playerDeathTokenObject = PhotonNetwork.Instantiate(deathToken.name, transform.position, Quaternion.identity);
        playerDeathTokenObject.GetComponent<playerDeathToken>().tokenValue = playerCints / 2;
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

        sceneFader.ScreenFade();
        sceneFader.ScreenFadeIn();

        player.transform.position = spawnManager.spawnPosition;
        playerLives -= 1;

        foreach (XRRayInteractor ray in rayInteractors)
        {
            ray.enabled = true;
        }
        foreach (XRDirectInteractor direct in directInteractors)
        {
            direct.enabled = true;
        }

        respawnUI.UpdateRespawnUI();
        Health = 125;
        CheckHealthStatus();
        alive = true;
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

    public void EnemyKilled()
    {
        enemiesKilled++;

        Hashtable hash = new Hashtable();
        hash.Add("enemyKills", enemiesKilled);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        enemyKillUI.CheckEnemiesKilled();
    }

    public void PlayersKilled()
    {
        playersKilled++;

        Hashtable hash = new Hashtable();
        hash.Add("playerKills", playersKilled);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        playerKillUI.CheckEnemiesKilled();
    }

    void ExtractionGame()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(ExtractionGameMode, null, raiseEventOptions, sendOptions);
    }

    void PlayerGame()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(PlayerGameMode, null, raiseEventOptions, sendOptions);
    }

    void EnemyGame()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(EnemyGameMode, null, raiseEventOptions, sendOptions);
    }

    IEnumerator DisplayMessage(string message)
    {
        yield return new WaitForSeconds(3);
        winCanvas.SetActive(true);
        messageText.text = message;
        yield return new WaitForSeconds(3);
        messageText.text = "";
        sceneFader.ScreenFade();
        VirtualWorldManager.Instance.LeaveRoomAndLoadHomeScene();
    }

    IEnumerator WinMessage(string message)
    {
        yield return new WaitForSeconds(0);
        winCanvas.SetActive(true);
        messageText.text = message;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == PlayerHealth.ExtractionGameMode)
        {
            string name = spawnManager.winnerPlayer.GetComponentInParent<PhotonView>().Owner.NickName;
            StartCoroutine(DisplayMessage(name + " has extracted the reactor and won the round. Loading Home Screen."));
        }

        if (photonEvent.Code == PlayerHealth.PlayerGameMode)
        {
            string name = spawnManager.winnerPlayer.GetComponentInParent<PhotonView>().Owner.NickName;
            StartCoroutine(DisplayMessage(name + " has defeated 25 players and won the round. Loading Home Screen."));
        }

        if (photonEvent.Code == PlayerHealth.EnemyGameMode)
        {
            string name = spawnManager.winnerPlayer.GetComponentInParent<PhotonView>().Owner.NickName;
            StartCoroutine(DisplayMessage(name + " has defeated 50 enemies and won the round. Loading Home Screen."));
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
            Health += 2;
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
    }
}
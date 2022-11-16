using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using Photon.Pun.Demo.Cockpit;
using Unity.VisualScripting;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.UIElements;

public class PlayerHealth : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject player;
    public SpawnManager spawnManager;
    public SaveData saveData;
    public SceneFader sceneFader;

    public int Health = 100;
    public int reactorExtraction = 0;
    public float reactorTimer = 0;

    public int playerLives = 3;
    public int playersKilled;
    public int enemiesKilled;

    public bool alive;
    public bool reactorHeld;
    public bool extractionWinner;
    public bool playerWinner;
    public bool enemyWinner;

    public int maxHealth;
    public int currentHelath;
    public int healthLevel;

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
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
        playerLives = 3;
        Health = 100;
        reactorExtraction = 0;
        playersKilled = 0;
        enemiesKilled = 0;
        alive = true;
        extractionWinner = false;
        playerWinner = false;
        enemyWinner = false;
        winCanvas.SetActive(false);
        maxHealth = SetMaxHealthFromHealthLevel();
        multiplayerHealth.SetMaxHealth(maxHealth);
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
                reactorExtraction += 2;

                Hashtable hash = new Hashtable();
                hash.Add("reactorExtraction", reactorExtraction);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

                reactorUI.UpdateReactorUI();
                reactorTimer = 0;
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
            saveData.UpdateSkills(250);
            ExtractionGame();
        }

        if (playersKilled >= 25 && spawnManager.gameOver == false)
        {
            playerWinner = true;
            spawnManager.gameOver = true;
            spawnManager.winnerPlayer = this.gameObject;
            StartCoroutine(WinMessage("250 skill points awarded for winning the round"));
            saveData.UpdateSkills(250);
            PlayerGame();
        }

        if (enemiesKilled >= 50 && spawnManager.gameOver == false)
        {
            enemyWinner = true;
            spawnManager.gameOver = true;
            spawnManager.winnerPlayer = this.gameObject;
            StartCoroutine(WinMessage("150 skill points awarded for winning the round"));
            saveData.UpdateSkills(250);
            EnemyGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("XP") || other.CompareTag("Health"))
        {
            audioSource.PlayOneShot(xpClip);
        }
    }

    public void TakeDamage(int damage)
    {
        audioSource.PlayOneShot(bulletHit);
        Health -= damage;
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
        VirtualWorldManager.Instance.LeaveRoomAndLoadHomeScene();
    }

    IEnumerator PlayerRespawn()
    {
        yield return new WaitForSeconds(0);
        alive = false;
        sceneFader.ScreenFade();
        yield return new WaitForSeconds(2);

        sceneFader.ScreenFadeIn();
        player.transform.position = new Vector3(-5.452f, 20.65f, -3.749f);
        this.transform.position = new Vector3(-5.452f, 3.5f, -3.749f);
        playerLives -= 1;

        respawnUI.UpdateRespawnUI();
        Health = 100;
        CheckHealthStatus();
        alive = true;
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
}

using PathologicalGames;
using System.Collections;
using Umbrace.Unity.PurePool;
using UnityEngine;
using static DroneHealth;

public class AIWeapon : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletTransform;
    public float shootForce;

    public int ammoLeft;
    public int maxAmmo;

    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioClip weaponReload;

    public bool fireWeaponBool = false;
    public bool canShoot = true;
    public int bulletModifier;
    public int adjustedBulletModifer;

    private EnemyType enemyType;

    public FollowAI aiScript;

    public int enemyLevel;
    public MatchEffects matchEffects;
    public GameObjectPoolManager PoolManager;

    public enum EnemyType
    {
        Enemy,
        BossEnemy
    }

    void OnEnable()
    {
        matchEffects = GameObject.FindGameObjectWithTag("Props").GetComponent<MatchEffects>();
        enemyLevel = matchEffects.level;

        enemyType = gameObject.CompareTag("Enemy") ? EnemyType.Enemy : EnemyType.BossEnemy;
        maxAmmo = (enemyType == EnemyType.Enemy) ? 10 : 15;
        ammoLeft = maxAmmo;
        bulletModifier = gameObject.CompareTag("Enemy") ? 2 : 3;
        adjustedBulletModifer = bulletModifier * Mathf.Min((enemyLevel / 10 + 1), 10);

        aiScript = GetComponent<FollowAI>();
        StartCoroutine(Fire());
        if (this.PoolManager == null)
        {
            this.PoolManager = Object.FindObjectOfType<GameObjectPoolManager>();

        }
    }

    IEnumerator Fire()
    {
        while (gameObject.activeSelf)
        {
            if (fireWeaponBool && aiScript.fireReady)
            {
                if (ammoLeft <= 0)
                {
                    canShoot = false;
                    StartCoroutine(ReloadWeapon());
                }
                else if (canShoot)
                {
                    yield return new WaitForSeconds(0.25f);
                    GameObject spawnedBullet = this.PoolManager.Acquire(bullet, bulletTransform.position, Quaternion.identity);
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = adjustedBulletModifer;
                    spawnedBullet.GetComponent<Rigidbody>().velocity = bulletTransform.forward * shootForce * GlobalSpeedManager.SpeedMultiplier;
                    ammoLeft--;
                }
            }
            yield return new WaitForSeconds(Random.Range(0.25f, 1f));
        }
    }

    IEnumerator ReloadWeapon()
    {
        audioSource2.PlayOneShot(weaponReload);
        yield return new WaitForSeconds(2);
        ammoLeft = maxAmmo;
        canShoot = true;
    }
}

using PathologicalGames;
using System.Collections;
using Umbrace.Unity.PurePool;
using UnityEngine;

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

    private EnemyType enemyType;
    public GameObjectPoolManager PoolManager;

    public FollowAI aiScript;
    public enum EnemyType
    {
        Enemy,
        BossEnemy
    }

    void OnEnable()
    {
        PoolManager = GameObject.FindGameObjectWithTag("Pool").GetComponent<GameObjectPoolManager>();

        enemyType = gameObject.CompareTag("Enemy") ? EnemyType.Enemy : EnemyType.BossEnemy;
        maxAmmo = (enemyType == EnemyType.Enemy) ? 10 : 15;
        ammoLeft = maxAmmo;
        bulletModifier = gameObject.CompareTag("Enemy") ? 2 : 3;

        aiScript = GetComponent<FollowAI>();
        StartCoroutine(Fire());
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
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = bulletModifier;
                    spawnedBullet.GetComponent<Rigidbody>().velocity = bulletTransform.forward * shootForce;
                    ammoLeft--;
                }
            }
            yield return new WaitForSeconds(0.25f);
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

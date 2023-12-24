using System.Collections;
using Umbrace.Unity.PurePool;
using UnityEngine;

public class ReinforcementWeapon : MonoBehaviour
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

    public ReinforcementAI aiScript;
    public GameObjectPoolManager PoolManager;

    public enum EnemyType
    {
        Enemy,
        BossEnemy
    }

    void OnEnable()
    {
        maxAmmo = 15;
        ammoLeft = maxAmmo;
        bulletModifier = 3;

        aiScript = GetComponent<ReinforcementAI>();
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
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = bulletModifier;
                    spawnedBullet.GetComponent<Rigidbody>().velocity = bulletTransform.forward * shootForce;
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

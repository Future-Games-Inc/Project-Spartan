using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Threading.Tasks;

public class AIWeapon : MonoBehaviourPunCallbacks
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

    private EnemyType enemyType;

    public enum EnemyType
    {
        Enemy,
        BossEnemy
    }

    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();
        if (gameObject.tag == "Enemy")
        {
            maxAmmo = 10;
            enemyType = EnemyType.Enemy;
        }
        else if (gameObject.tag == "BossEnemy")
        {
            maxAmmo = 15;
            enemyType = EnemyType.BossEnemy;
        }
        ammoLeft = maxAmmo;
        StartCoroutine(Fire());
    }

    IEnumerator Fire()
    {
        while (true)
        {
            while (!fireWeaponBool)
                yield return null;

            if (ammoLeft <= 0)
            {
                canShoot = false;
                ReloadWeapon();
                continue;
            }

            if (fireWeaponBool)
            {
                yield return new WaitForSeconds(0.25f);
                GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(bullet.name, bulletTransform.position, Quaternion.identity, 0, null);
                spawnedBullet.GetComponent<Bullet>().audioSource.PlayOneShot(spawnedBullet.GetComponent<Bullet>().clip);

                // Check with cached enemy type to save performance
                if (enemyType.Equals(EnemyType.Enemy))
                {
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = 2;
                }
                if (enemyType.Equals(EnemyType.BossEnemy))
                {
                    spawnedBullet.GetComponent<Bullet>().bulletModifier = 4;
                }
                shootForce = Random.Range(40, 75);
                spawnedBullet.GetComponent<Rigidbody>().velocity = bulletTransform.forward * shootForce;

                ammoLeft--;
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    // -------------------- OLD FIRE COROUTINE ------------------

    //IEnumerator Fire()
    //{
    //    while (true)
    //    {
    //        if (ammoLeft >= 1 && fireWeaponBool == true && canShoot == true)
    //        {
    //            yield return new WaitForSeconds(0.25f);
    //            if (!audioSource.isPlaying)
    //            {
    //                photonView.RPC("RPC_ShootAudio", RpcTarget.All);
    //            }
    //            GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(bullet.name, bulletTransform.position, Quaternion.identity, 0, null);
    //            if (this.gameObject.tag == "Enemy")
    //            {
    //                spawnedBullet.GetComponent<Bullet>().bulletModifier = 2;
    //            }
    //            else if (this.gameObject.tag == "BossEnemy")
    //            {
    //                spawnedBullet.GetComponent<Bullet>().bulletModifier = 4;
    //            }
    //            shootForce = (int)Random.Range(40, 75);
    //            spawnedBullet.GetComponent<Rigidbody>().velocity = bulletTransform.forward * shootForce;

    //            ammoLeft--;
    //            yield return new WaitForSeconds(.25f);
    //        }

    //        else if (ammoLeft <= 0)
    //        {
    //            canShoot = false;
    //            ReloadWeapon();
    //        }

    //        yield return null;
    //    }
    //}

    async void ReloadWeapon()
    {
        await Task.Delay(2000);
        audioSource2.PlayOneShot(weaponReload);
        ammoLeft = maxAmmo;
        canShoot = true;
    }

    // -------------------- OLD RELOAD WEAPON COROUTINE -----------------------

    //IEnumerator ReloadWeapon()
    //{
    //    yield return new WaitForSeconds(0);
    //    if (!audioSource2.isPlaying)
    //    {
    //        photonView.RPC("RPC_Reload1", RpcTarget.All);
    //    }
    //    yield return new WaitForSeconds(2);
    //    ammoLeft = maxAmmo;
    //    canShoot = true;
    //}
}

using System.Collections;
using UnityEngine;
using Photon.Pun;

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

    public override void OnEnable()
    {
        base.OnEnable();
        enemyType = gameObject.CompareTag("Enemy") ? EnemyType.Enemy : EnemyType.BossEnemy;
        maxAmmo = (enemyType == EnemyType.Enemy) ? 10 : 15;
        ammoLeft = maxAmmo;
        StartCoroutine(Fire());
    }

    [PunRPC]
    void FireBullet(Vector3 position, Quaternion rotation, int bulletModifier)
    {
        GameObject spawnedBullet = PhotonNetwork.InstantiateRoomObject(bullet.name, position, rotation, 0, null);
        spawnedBullet.GetComponent<Bullet>().bulletModifier = bulletModifier;
        spawnedBullet.GetComponent<Rigidbody>().velocity = bulletTransform.forward * shootForce;
    }

    IEnumerator Fire()
    {
        while (gameObject.activeSelf)
        {
            if (fireWeaponBool)
            {
                if (ammoLeft <= 0)
                {
                    canShoot = false;
                    StartCoroutine(ReloadWeapon());
                }
                else if (canShoot)
                {
                    yield return new WaitForSeconds(0.25f);
                    int bulletModifier = (enemyType == EnemyType.Enemy) ? 2 : 4;
                    photonView.RPC("FireBullet", RpcTarget.All, bulletTransform.position, Quaternion.identity, bulletModifier);
                    ammoLeft--;
                }
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    [PunRPC]
    void ReloadSound()
    {
        audioSource2.PlayOneShot(weaponReload);
    }

    IEnumerator ReloadWeapon()
    {
        photonView.RPC("ReloadSound", RpcTarget.All);
        yield return new WaitForSeconds(2);
        ammoLeft = maxAmmo;
        canShoot = true;
    }
}

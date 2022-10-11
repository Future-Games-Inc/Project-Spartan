using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.InputSystem;

public class ProjectileGun : MonoBehaviour
{

    private Transform target;
    [SerializeField][Tooltip("Range of the gun to identify the enemy target")]
    public float range = 15f;
    public string enemyTag = "Enemy";

    public InputActionReference Reload_Button;
    public InputActionReference Trigger_Button;
    public InputActionReference HoldingTrigger_Button;


    public GameObject bullet;

    public float shootForce;
    public float upwardForce;


    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletShot;

    bool shooting, readyToShoot, reloading;

    //public Camera fpsCam;
    public Transform attackPoint;


    public GameObject muzzleflash;
    public TextMeshProUGUI ammunitionDisplay;

    public bool allowInvoke = true;


    public void Awake()
    {
        Reload_Button.action.performed += reload_ButtonMethod;
        Trigger_Button.action.performed += Trigger_ButtonMethod;
        HoldingTrigger_Button.action.performed += HoldingTrigger_ButtonMethod;

        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void OnDestroy()
    {
        Reload_Button.action.performed -= reload_ButtonMethod;
        Trigger_Button.action.performed -= Trigger_ButtonMethod;
        HoldingTrigger_Button.action.performed -= HoldingTrigger_ButtonMethod;
    }

    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    private void Update()
    {
        myInput();

        if(ammunitionDisplay != null)
        {
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
        }

    }



    public void myInput()
    {
        if(allowButtonHold) // if pressing the button down on hold
        {
          //  shooting = HoldingTrigger_Button.action.IsPressed();
          //  Debug.Log("is pressed");
        }
        else
        {
            shooting = Trigger_Button.action.IsPressed();
            Debug.Log("Was pressed this frame");
        }

        if(allowButtonHold) // pressing the reload button do this
        {
            Reload();


        }
        if(readyToShoot && shooting && !reloading && bulletsLeft <= 0)

        {
            Reload();
        }

        //shooting
        if(readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletShot = 0;

            Shoot();

        }

    }

    private void Shoot()
    {
        readyToShoot = false;



        Ray ray = new Ray(origin: attackPoint.transform.position, attackPoint.transform.forward);
        RaycastHit hit;

        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;
        //calculating spread
        float x = UnityEngine.Random.Range(-spread, spread);
        float y = UnityEngine.Random.Range(-spread, spread);
        //new direction with spread
        Vector3 directionwithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        //Bullet_Amunition amu_bullet = currentBullet.GetComponent<Bullet_Amunition>();

        currentBullet.transform.forward = directionwithSpread.normalized;

        //add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionwithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(attackPoint.transform.up * upwardForce, ForceMode.Impulse);

        //if (amu_bullet != null)
        //{
        //    amu_bullet.Seek(target);
        //}


        if (muzzleflash !=null)
        {
            Instantiate(muzzleflash, attackPoint.position,  Quaternion.identity);
        }
        bulletsLeft--;
        bulletShot++;

        //Invoke resetShot function

        if(allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        if(bulletShot < bulletsPerTap && bulletsLeft >  0 )
        {
            Invoke("Shoot", timeBetweenShots);
        }

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);

    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;

    }

    // Start is called before the first frame update

    
    private void reload_ButtonMethod(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }


    private void Trigger_ButtonMethod(InputAction.CallbackContext obj)
    {

        throw new NotImplementedException();
    }


    private void HoldingTrigger_ButtonMethod(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();

    }

    //void UpdateTarget()
    //{
    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

    //    float shortedDistance = Mathf.Infinity;

    //    GameObject nearestEnemy = null;
    //    //For each enemy that we find
    //    foreach (GameObject enemy in enemies)
    //    {
    //        //get the distance to that enemy
    //        float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
    //        //if the distance we found is smaller than the distance we seen before
    //        if (distanceToEnemy < shortedDistance)
    //        {
    //            // if true then the shortest distance to this enemy is this enemy. 
    //            shortedDistance = distanceToEnemy;
    //            nearestEnemy = enemy;
    //        }



    //    }
    //    if (nearestEnemy != null && shortedDistance <= range)
    //    {
    //        target = nearestEnemy.transform;
    //    }
    //    else
    //    {
    //        target = null;
    //    }

    //}


}

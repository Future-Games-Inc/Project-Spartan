using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class EnemyLineOfSightCheck : MonoBehaviour
{
    public SphereCollider collider;
    public float fieldOfView;
    public LayerMask lineOfSightLayer;

    public delegate void GainSightEvent(PlayerStats player);
    public GainSightEvent onGainSight;
    public delegate  void LoseSightEvent(PlayerStats player);
    public LoseSightEvent onLoseSight;


    private Coroutine checkForLineOfSightCoroutine;
    private EnemyMovement enemymovement;

    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        enemymovement = GetComponent<EnemyMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "Player")
        //{
        //    Transform player_Transform = other.GetComponent<Transform>();
        //    enemymovement.FoundPlayer(player_Transform);

        //}


        PlayerStats player;
        if(other.TryGetComponent<PlayerStats>(out player))
        {
            if(!CheckLineOfSight(player))
            {
                checkForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(player));

            }
        }
    }

    private void OnTigerrExit(Collider other)
    {
        PlayerStats player;
        if(other.TryGetComponent<PlayerStats>(out player))
            
        {
            onLoseSight?.Invoke(player);

            if(checkForLineOfSightCoroutine != null)
            {
                StopCoroutine(checkForLineOfSightCoroutine);

            }
        }
    }

    private bool CheckLineOfSight(PlayerStats player)
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;

        if (Vector3.Dot(transform.forward, direction) >= Mathf.Cos(fieldOfView))
        {
            RaycastHit hit;

            if(Physics.Raycast(transform.position, direction, out hit, collider.radius, lineOfSightLayer))
            {
                if(hit.transform.GetComponent<PlayerStats>() != null)
                {

                    onGainSight?.Invoke(player);
                    return true;
                }
            }
        }
        return false;
    }

    private IEnumerator CheckForLineOfSight(PlayerStats player)
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while(!CheckLineOfSight(player))
        {
            yield return wait;
        }

    }



}

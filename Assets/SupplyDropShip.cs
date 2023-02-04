using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using ExitGames.Client.Photon;

public class SupplyDropShip : MonoBehaviourPunCallbacks
{
    public GameObject supplyDropCratePrefab;
    public Transform[] waypoints;
    public Transform dropPoint;
    public float randomTime;

    private int currentWaypoint;
    private float elapsedTime;
    private bool isCrateInstantiated;

    public static readonly byte SupplyShipArrive = 30;
    public static readonly byte SupplyShipDestroy = 31;

    float speed = 0.1f;
    float timeCount = 0.0f;

    void Awake()
    {
        GameObject waypointObject = GameObject.FindGameObjectWithTag("SupplyWaypoints");
        waypoints = waypointObject.GetComponentsInChildren<Transform>();

        randomTime = Random.Range(15, 30);
    }

    private void Start()
    {
        photonView.RPC("RaiseEvent1", RpcTarget.All, SupplyDropShip.SupplyShipArrive, null);
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= randomTime && !isCrateInstantiated)
        {
            StartCoroutine(InstantiateCrate());
            isCrateInstantiated = true;
        }

        if (!isCrateInstantiated)
        {
            Move();
        }
    }

    void Move()
    {
        Vector3 targetPosition = waypoints[currentWaypoint].position;
        targetPosition.y = transform.position.y;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position, Vector3.up), timeCount * speed); //Calculate and insert back in here
        //timeCount = timeCount + Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position, Vector3.up), Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            currentWaypoint++;
            timeCount = 0;
            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 1;
            }
        }
    }

    IEnumerator InstantiateCrate()
    {
        PhotonNetwork.Instantiate(supplyDropCratePrefab.name, dropPoint.position, Quaternion.identity, 0);
        photonView.RPC("RaiseEvent2", RpcTarget.All, SupplyDropShip.SupplyShipDestroy, null);
        yield return new WaitForSeconds(3f);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RaiseEvent1(byte eventCode, object content, PhotonMessageInfo info)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, sendOptions);
    }

    [PunRPC]
    void RaiseEvent2(byte eventCode, object content, PhotonMessageInfo info)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, sendOptions);
    }
}
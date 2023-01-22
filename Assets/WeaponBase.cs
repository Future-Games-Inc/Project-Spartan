using InfimaGames.LowPolyShooterPack;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WeaponBase : MonoBehaviour
{
    [SerializeField]

    public WeaponCrate weaponCrate;
    // Start is called before the first frame update
    void Start()
    {
        weaponCrate = GetComponentInParent<WeaponCrate>();
        StartCoroutine(SetPosition());
    }

    IEnumerator SetPosition()
    {
        yield return new WaitForSeconds(1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Attack Configuration", menuName = "ScriptableObject/Attack Configuration")]
public class AttackScriptableObject : ScriptableObject
{
    public bool isRanged = false;
    public int Damage = 5;
    public float AttackRadius = 1.5f;
    public float AttackDelay = 1.5f;


    //Ranged Configurations
    public Enemy_Bullet bulletPrefab;
    public Vector3 bulletSpawnOffset = new Vector3(0, 1, 0);
    public LayerMask lineOfSightLayers;

    public AttackScriptableObject ScaleUpForLevel(ScalingScriptableObject scaling, int level)
    {
        AttackScriptableObject scaledUpConfiguration = CreateInstance<AttackScriptableObject>();

        scaledUpConfiguration.isRanged = isRanged;
        scaledUpConfiguration.Damage = Mathf.FloorToInt(Damage * scaling.damageCurve.Evaluate(level));
        scaledUpConfiguration.AttackRadius = AttackRadius;
        scaledUpConfiguration.AttackDelay = AttackDelay;

        scaledUpConfiguration.bulletPrefab = bulletPrefab;
        scaledUpConfiguration.bulletSpawnOffset = bulletSpawnOffset;
        scaledUpConfiguration.lineOfSightLayers = lineOfSightLayers;


        return scaledUpConfiguration;

    }


    public void SetupEnemy(Enemigo enemy)
    {
        (enemy.AttackRadius.collider == null ? enemy.AttackRadius.GetComponent<SphereCollider>() : enemy.AttackRadius.collider).radius = AttackRadius;
        enemy.AttackRadius.AttackDelay = AttackDelay;
        enemy.AttackRadius.Damage = Damage;

        if(isRanged)
        {
            RangedAttackRadius rangedAttackRadius = enemy.AttackRadius.GetComponent<RangedAttackRadius>();

            rangedAttackRadius.bulletPrefab = bulletPrefab;
            rangedAttackRadius.bulletSpawnOffset = bulletSpawnOffset;
            rangedAttackRadius.layermask = lineOfSightLayers;



            rangedAttackRadius.CreateBulletPool();
        }



    }
}

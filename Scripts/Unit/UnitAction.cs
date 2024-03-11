using System;
using UnityEngine;

//Attach to Unit Prefab
//Logic of Unit's actions preformed.
//This can include making of bullets, perform events based on certain conditions
public class UnitAction : MonoBehaviour
{
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTranform;
    [SerializeField] private AudioSource audioSource;
    private Transform bulletProjectileTransform;
    private BulletProjectile bulletProjectile;

    private void Awake()
    {
        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }
        
    }


    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        //Make the bullet
        bulletProjectileTransform =
            Instantiate(bulletProjectilePrefab, shootPointTranform.position, e.shootingUnit.GetWorldRotation());

        //Play ActionSource sound for said action (WIP)
        audioSource.Play();

        //Check target unit for ragdoll spawner script to assign bullet's forward rotation to value
        if (e.targetUnit.TryGetComponent<UnitRagdollSpawner>(out UnitRagdollSpawner unitRagdoolSpawner))
        {
            unitRagdoolSpawner.lastKnownImpact = bulletProjectileTransform.forward;
        }
        
        //Get bullet's projectile
        bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        //Get target units location
        Vector3 targetUnitShootAtPos = e.targetUnit.GetWorldPos();

        //adjust height of location for bullet to land on unit
        targetUnitShootAtPos.y = shootPointTranform.position.y;

        //send bullet details to bulletProjectile
        bulletProjectile.Setup(targetUnitShootAtPos);
    }
}

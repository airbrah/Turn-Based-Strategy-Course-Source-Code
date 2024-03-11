using System;
using UnityEngine;

//Attached to Unit
//Sets up Ragdoll Prefab to take the place of current Unit Prefab
public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform ragdollPrefab;
    [SerializeField] private Transform orginalRootBone;

    public Vector3 lastKnownImpact; //Fix privacy level?
    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();

        healthSystem.OnDead += HealthSystem_OnDead;

        
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        //Setup Ragdoll
        Transform ragdollTransform = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        UnitRagdoll unitRagdoll = ragdollTransform.GetComponent<UnitRagdoll>();

        //begin Ragdoll proc
        unitRagdoll.Setup(orginalRootBone, lastKnownImpact);
    }
}

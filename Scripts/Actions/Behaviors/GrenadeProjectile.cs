using System;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    //Events
    public static event EventHandler OnAnyGrenadeExploded;
    //Vars
    [SerializeField] private Transform grenadeExplodeVfxPrefab;
    [SerializeField] private Transform grenadeExplodeAudioPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;
    

    private Vector3 targetPos;
    private Action onGrenadeBehaviourComplete;
    private float totalDistance;
    private Vector3 positionXZ;

    private void Update()
    {
        Vector3 moveDir = (targetPos - positionXZ).normalized;

        float moveSpeed = 15f;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPos);
        float distanceNormalized = 1 - distance / totalDistance;

        float maxHeight = totalDistance / 4f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float reachedTargetDistance = .2f;
        if(Vector3.Distance(positionXZ, targetPos) < reachedTargetDistance)
        {
            float damageRadius = 3f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPos, damageRadius);
            
            foreach(Collider collider in colliderArray)
            {
                if(collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(55f);
                }
                if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate destructibleCrate))
                {
                    destructibleCrate.Damage();
                }
            }

            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
            
            trailRenderer.transform.parent = null;

            Instantiate(grenadeExplodeVfxPrefab, targetPos + Vector3.up * 1, Quaternion.identity);
            Instantiate(grenadeExplodeAudioPrefab, targetPos + Vector3.up * 1, Quaternion.identity);
            Destroy(gameObject);

            onGrenadeBehaviourComplete();
        }
    }
    public void SetUp(GridPos targetGridPos, Action onGrenadeBehaviourComplete)
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        targetPos = LevelGrid.Instance.GetWorldPos(targetGridPos);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPos);
    }
}

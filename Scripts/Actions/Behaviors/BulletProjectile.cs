using UnityEngine;

//Attached to BulletProjectile (Prefab)
//Purpose of this script is to take bulletProjectile (Prefab)
//and move it to desired targetPos
//Spawns a VFX Prefab that's assigned in the inspector
public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVfxPrefab;
    private Vector3 targetPos;

    public void Setup(Vector3 targetPos)
    {
        this.targetPos = targetPos;
    }
    private void Awake()
    {
        
    }
    private void Update()
    {
        Vector3 moveDir = (targetPos - transform.position).normalized;

        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPos);
        
        float moveSpeed = 2000f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPos);

        if (distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = targetPos;
            trailRenderer.transform.parent = null;
            Destroy(gameObject);
            Instantiate(bulletHitVfxPrefab, targetPos, Quaternion.identity); 
        }
    }
}

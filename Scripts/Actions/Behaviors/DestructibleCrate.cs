using System;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    [SerializeField] private Transform crateDestroyedPrefab;

    public static event EventHandler OnAnyDestroyed;
    private GridPos gridPos;

    private void Start()
    {
        gridPos = LevelGrid.Instance.GetGridPos(transform.position);
    }

    public GridPos GetGridPos()
    {
        return gridPos;
    }

    public void Damage()
    {
        Transform crateDestroyedTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);
        ApplyExplosionToChldren(crateDestroyedTransform, 150f, transform.position, 10f);
        Destroy(gameObject);

        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }
    private void ApplyExplosionToChldren(Transform root, float explosionForce, Vector3 explosionPos, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPos, explosionRange);
            }
            ApplyExplosionToChldren(child, explosionForce, explosionPos, explosionRange);
        }
    }
}

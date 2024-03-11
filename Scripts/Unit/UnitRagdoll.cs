using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] deathSounds;

    public void Setup(Transform originalRootBone, Vector3 forceImpact) 
    {
        // make ragdoll prefab match what unit's limb pos was; avoid t-pose
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);

        //Apply random death sounds to ragdoll
        ApplyAudioDeathSounds();

        //Seperate method to do this only when killed by gunshot?
        //Apply force of bullet to physic ragdoll to go the direction the shot came from
        ApplyInverseForceToRagdoll(ragdollRootBone, forceImpact * 2000f); 

        //Explosion
        //Vector3 randomDir = new Vector3(Random.Range(-1f, +1f), 0, Random.Range(-1f, +1f));
        //ApplyExplosionToRagdoll(ragdollRootBone, 300f, transform.position + randomDir, 10f);
        
    }
    private void MatchAllChildTransforms(Transform root, Transform clone)
    {
        foreach(Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;
                MatchAllChildTransforms(child, cloneChild);
            }
        }
    }
    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPos, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if(child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPos, explosionRange);
            }
            ApplyExplosionToRagdoll(child, explosionForce, explosionPos, explosionRange);
        }
    }
    private void ApplyInverseForceToRagdoll(Transform root, Vector3 applyForce)
    {
        //Maybe look for head instead? for headshots?
        Transform child = root.GetChild(0); //0 Should be the Hips root
        
        if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
        {
            childRigidbody.AddForce(applyForce);
        }
    }
    private void ApplyAudioDeathSounds()
    {
        if (deathSounds == null)
        {
            return;
        }
        int arrayLength = deathSounds.Length;
        int randomNum = Random.Range(0, arrayLength);

        audioSource.clip = deathSounds[randomNum];
        audioSource.Play();
    }
}

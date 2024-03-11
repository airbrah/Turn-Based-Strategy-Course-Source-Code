using UnityEngine;

public class GrenadeAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        
    }
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}

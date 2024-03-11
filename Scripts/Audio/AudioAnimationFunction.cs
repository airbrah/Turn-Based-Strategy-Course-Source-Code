using UnityEngine;

public class AudioAnimationFunction : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footClips;
    [SerializeField] private AudioClip[] doorOpen;
    [SerializeField] private AudioClip[] doorClosed;
    //Animation Events
    private void OnAniFootstep(AnimationEvent animationEvent)
    {
        int random = UnityEngine.Random.Range(0, footClips.Length);
        audioSource.clip = footClips[random];
        audioSource.Play();
    }
    private void OnAniDoorOpen(AnimationEvent animationEvent)
    {
        int random = UnityEngine.Random.Range(0, doorOpen.Length);
        audioSource.clip = doorOpen[random];
        audioSource.Play();
    }
    private void OnAniDoorClosed(AnimationEvent animationEvent)
    {
        int random = UnityEngine.Random.Range(0, doorClosed.Length);
        audioSource.clip = doorClosed[random];
        audioSource.Play();
    }
}

using System;
using UnityEngine;

public class UnitAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] hurtSounds;
    [SerializeField] private AudioClip[] meleeSounds;

    private HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        if (TryGetComponent<MeleeAction>(out MeleeAction meleeAction))
        {
            meleeAction.OnMeleeActionStarted += MeleeAction_OnMeleeActionStarted;
        }
    }

    private void MeleeAction_OnMeleeActionStarted(object sender, EventArgs e)
    {
        PlayMeleeSound();
    }
    private void PlayMeleeSound()
    {
        if (meleeSounds == null)
        {
            return;
        }

        int arrayLength = meleeSounds.Length;
        int randomNum = UnityEngine.Random.Range(0, arrayLength);
        
        audioSource.clip = meleeSounds[randomNum];
        audioSource.Play();
    }
    private void PlayHurtSound()
    {
        if (hurtSounds == null)
        {
            return;
        }

        int arrayLength = hurtSounds.Length;
        int randomNum = UnityEngine.Random.Range(0, arrayLength);

        audioSource.clip = hurtSounds[randomNum];
        audioSource.Play();
    }
    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        PlayHurtSound();
    }
}

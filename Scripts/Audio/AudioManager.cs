using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Singleton
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource backgroundAudioSource;

    private void Awake()
    {
        //Singleton
        if (Instance != null)
        {
            Debug.LogError("There's more than one AudioManager! " + transform + " - " + Instance);
            Destroy(Instance);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
    }
    private void Update()
    {
        
    }
    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                backgroundAudioSource.pitch = .4f;
                break;
        }
    }
    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                backgroundAudioSource.pitch = 1f;
                break;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTES
//Should have all avaliable wav treated as data. consider using scriptable objects maybe in the future?
public class AudioScript : MonoBehaviour
{
    [SerializeField] private AudioClip[] onSelectClips;
    [SerializeField] private AudioClip[] onMoveClips;
    [SerializeField] private AudioClip[] onEngageClips;
    [SerializeField] private AudioClip[] footClips;
    [SerializeField] private AudioClip[] m4ShootClips;
    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private GameObject gunFlair;
    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnActionTaken += UnitActionSystem_OnActionTaken;
    }
    //Animation Events
    private void OnAniRightFootstep(AnimationEvent animationEvent)
    {
        int random = UnityEngine.Random.Range(0, footClips.Length);
        audioSources[0].clip = footClips[random];
        audioSources[0].Play();
    }
    private void OnAniLeftFootstep(AnimationEvent animationEvent)
    {
        int random = UnityEngine.Random.Range(0, footClips.Length);
        audioSources[1].clip = footClips[random];
        audioSources[1].Play();
    }
    private void OnShoot(AnimationEvent animationEvent)
    {
        int random = UnityEngine.Random.Range(0, m4ShootClips.Length);
        audioSources[2].clip = m4ShootClips[random];
        audioSources[2].Play();
        gunFlair.SetActive(true);
    }
    private void OnShootEnd(AnimationEvent animationEvent)
    {
        //you can do better than this...
        gunFlair.SetActive(false);
    }

    private void PlaySelectLine()
    {
        int random = UnityEngine.Random.Range(0, onSelectClips.Length);
        audioSources[3].clip = onSelectClips[random];
        audioSources[3].Play();
    }
    private void PlayMoveLine()
    {
        int random = UnityEngine.Random.Range(0, onMoveClips.Length);
        audioSources[3].clip = onMoveClips[random];
        audioSources[3].Play();
    }
    private void PlayEngageLine()
    {
        int random = UnityEngine.Random.Range(0, onMoveClips.Length);
        audioSources[3].clip = onMoveClips[random];
        audioSources[3].Play();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs eventArgs)
    {
        PlaySelectLine();
    }
    private void UnitActionSystem_OnActionTaken(object sender, EventArgs eventArgs)
    {
        PlayMoveLine();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips = null;
    private AudioSource audioSource;
    [SerializeField]
    [Range(0,3)]
    private float pitchRandomization = 0;
    private float originalPitch = 1;

    [SerializeField]
    private bool interruptable = true;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        originalPitch = audioSource.pitch;
    }

    public void PlaySound()
    {
        if (!interruptable && audioSource.isPlaying)
            return;
        
        audioSource.pitch = originalPitch + Random.Range(-pitchRandomization, +pitchRandomization);
        audioSource.clip = clips[Random.Range(0, clips.Length)];

        audioSource.Play();
    }

    public void PlaySoundOneShot()
    {
        audioSource.pitch = originalPitch + Random.Range(-pitchRandomization, +pitchRandomization);
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)], audioSource.volume);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInAudio : MonoBehaviour
{

    private AudioSource audioSource;
    [SerializeField]
    private float fadeInTime = 1f;
    private float timer;
    private float originalVolume;
    [SerializeField]
    private bool randomStartTime = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        originalVolume = audioSource.volume;
        timer = fadeInTime;
        if (randomStartTime)
            audioSource.time = Random.Range(0, audioSource.clip.length);
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            audioSource.volume = (1 - (timer / fadeInTime)) * originalVolume;
        }
        else
        {
            audioSource.volume = originalVolume;
            this.enabled = false;
        }
    }
}

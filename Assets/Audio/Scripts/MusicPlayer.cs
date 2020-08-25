using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    

    [SerializeField]
    private MusicLevel[] musicLevels = null;
    private int currentLevel, lastLevel = -1;
    private List<AudioSource> audioSources;

    [SerializeField]
    private float globalVolume = 1;


    [SerializeField]
    private float transitionTime = 1f;
    private bool transitioning = false;
    private float transTime, transCounter = 0;

    private void Awake()
    {
        audioSources = new List<AudioSource>();
        CreateAndPlayAudioSources();
        TransitionToMusicLevel(0);
    }


    private void CreateAndPlayAudioSources()
    {
        for (int i = 0; i < musicLevels.Length; i++)
        {
            musicLevels[i].audioSource = GetFreeAudioSource();
            musicLevels[i].audioSource.clip = musicLevels[i].clip;
            musicLevels[i].audioSource.loop = true;
            musicLevels[i].audioSource.volume = 0;//i == currentLevel ? globalVolume : 0;
            musicLevels[i].audioSource.Play();
        }
    }

    private void Update()
    {
        
        if (transitioning)
        {
            transCounter -= Time.deltaTime;
            if (transCounter >= 0)
            {
                if(lastLevel>=0)
                    musicLevels[lastLevel].audioSource.volume = (transCounter / transTime)*globalVolume;

                musicLevels[currentLevel].audioSource.volume = (1 - (transCounter / transTime))*globalVolume;
            }
            else
            {
                transitioning = false;
                musicLevels[lastLevel].audioSource.volume = 0;
                musicLevels[currentLevel].audioSource.volume = globalVolume;

            }
        }
    }

    public void TransitionToMusicLevel(int newLevel)
    {
        lastLevel = currentLevel;
        currentLevel = newLevel;

        transitioning = true;
        transTime = transCounter = transitionTime;
    }
    
    [ContextMenu("TransToNextLevel")]
    public void TransToNextLevel()
    {
        TransitionToMusicLevel((int) Mathf.Repeat(currentLevel+1,musicLevels.Length));
    }

    private AudioSource GetFreeAudioSource()
    {
        AudioSource res = audioSources.Where(aud => !aud.isPlaying).FirstOrDefault();
        if (!res)
        {
            GameObject go = gameObject;

            res = gameObject.AddComponent<AudioSource>();
            audioSources.Add(res);
        }
        return res;
    }
}



[Serializable]
public struct MusicLevel
{
    public AudioClip clip;
    [HideInInspector]
    public AudioSource audioSource;
}

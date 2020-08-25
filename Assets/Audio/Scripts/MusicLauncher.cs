using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLauncher : MonoBehaviour
{
    public void SetMusicLevel(int level)
    {
        MusicPlayer mp = FindObjectOfType<MusicPlayer>();
        mp.TransitionToMusicLevel(level);
    }
}

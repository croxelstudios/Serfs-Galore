using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteSwitch : MonoBehaviour
{


    public void SwitchMute()
    {
        AudioListener.volume = AudioListener.volume>0?0:1;
    }
}

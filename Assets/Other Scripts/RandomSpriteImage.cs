using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSpriteImage : MonoBehaviour
{
    public Image[] images = null;
    public Sprite[] sprites = null;
    

    private void Awake()
    {
        var sprite = sprites[Random.Range(0, sprites.Length)];
        
        if (images!=null && sprites!=null) { 

            foreach (var item in images)
            {
                item.sprite = sprite;
            }
        }
    }
}

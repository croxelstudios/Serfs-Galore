using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites = null;
    [SerializeField]
    private Image image = null;
    private int index = 0;



    public void Switch()
    {
        index = (int) Mathf.Repeat(index + 1, sprites.Length);


        image.sprite = sprites[index];
    }
}

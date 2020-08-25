using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHidder : MonoBehaviour
{
    public void Hide()
    {
        Cursor.visible = false;
    }

    public void Unhide()
    {
        Cursor.visible = true;
    }

    public void SwitchVisibility()
    {
        Cursor.visible = !Cursor.visible;
    }
}

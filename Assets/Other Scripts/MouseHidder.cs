using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHidder : MonoBehaviour
{
    public Texture2D cursorTexture = null;

    public void Hide()
    {
        Cursor.visible = false;
    }

    public void Unhide()
    {
        Cursor.visible = true;
        SetCursorSkin();
    }

    public void SwitchVisibility()
    {
        Cursor.visible = !Cursor.visible;
        SetCursorSkin();
    }

    public void SetCursorSkin()
    {
        if(cursorTexture)
            Cursor.SetCursor(cursorTexture, Vector2.zero ,CursorMode.Auto);
    }
}

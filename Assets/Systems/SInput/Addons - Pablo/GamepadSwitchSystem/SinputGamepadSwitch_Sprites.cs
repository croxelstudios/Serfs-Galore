using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SinputGamepadSwitch_Sprites : SinputGamepadSwitch
{
    SpriteRenderer spriteRenderer;
    
    [SerializeField]
    Sprite[] sprites = null;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void SwitchValue(int newValue)
    {
        base.SwitchValue(newValue);
        spriteRenderer.sprite = sprites[current];
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(SinputGamepadSwitch_Sprites))]
public class CustomEditor_SinputGamepadSwitch_Sprites : CustomEditor_SinputGamepadSwitch
{
    SerializedProperty sprites;

    protected override void OnEnable()
    {
        base.OnEnable();
        sprites = serializedObject.FindProperty("sprites");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(sprites, true);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

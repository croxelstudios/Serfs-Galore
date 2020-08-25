using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class SinputGamepadSwitch_UISprites : SinputGamepadSwitch
{
    Image image;
    
    [SerializeField]
    Sprite[] sprites = null;

    void OnEnable()
    {
        image = GetComponent<Image>();
    }

    public override void SwitchValue(int newValue)
    {
        base.SwitchValue(newValue);
        image.sprite = sprites[current];
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(SinputGamepadSwitch_UISprites))]
public class CustomEditor_SinputGamepadSwitch_UISprites : CustomEditor_SinputGamepadSwitch
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

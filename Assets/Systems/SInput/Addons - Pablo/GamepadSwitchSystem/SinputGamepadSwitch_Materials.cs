using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class SinputGamepadSwitch_Materials : SinputGamepadSwitch
{
    Renderer rend;

    [SerializeField]
    int materialIndex = 0;
    [SerializeField]
    Material[] materials = null;

    void OnEnable()
    {
        rend = GetComponent<Renderer>();
    }

    public override void SwitchValue(int newValue)
    {
        base.SwitchValue(newValue);
        Material[] array = rend.materials;
        array[materialIndex] = materials[current];
        rend.materials = array;
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(SinputGamepadSwitch_Materials))]
public class CustomEditor_SinputGamepadSwitch_Materials : CustomEditor_SinputGamepadSwitch
{
    SerializedProperty materialIndex;
    SerializedProperty materials;

    protected override void OnEnable()
    {
        base.OnEnable();
        materialIndex = serializedObject.FindProperty("materialIndex");
        materials = serializedObject.FindProperty("materials");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(materialIndex);
        EditorGUILayout.PropertyField(materials, true);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

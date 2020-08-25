using UnityEngine;
using SinputSystems;
using UnityEditor;

[ExecuteInEditMode]
public class SinputGamepadSwitch : MonoBehaviour
{
    [SerializeField]
    SinputControllersData controllersData = null;
    [SerializeField]
    [Min(0)]
    protected int current = 0;
    [SerializeField]
    [Min(0)]
    int keyboard = 4;
    [SerializeField]
    [Min(0)]
    int gamepadDefault = 3;
    //TO DO: Connect and disconnect events

    void Start()
    {
        string[] controllerNames = Sinput.gamepads;
        //foreach (string name in controllerNames) Debug.Log(name);
        if (controllersData != null) SwitchValue(IdentifyController(controllersData, controllerNames));
    }

    int IdentifyController(SinputControllersData possibleControllers, string[] currentControllers)
    {
        for (int i = 0; i < possibleControllers.controllers.Length; i++)
            for (int j = 0; j < currentControllers.Length; j++)
                if (ControllerContainsName(possibleControllers.controllers[i], currentControllers[j])) return i;
        return -1;
    }

    bool ControllerContainsName(SinputControllersData.Controller controller, string name)
    {
        foreach (CommonMapping mapping in controller.mappings)
            if (CommonMappingContainsName(mapping, name)) return true;
        return false;
    }

    bool CommonMappingContainsName(CommonMapping mapping, string name)
    {
        foreach (string mapName in mapping.names)
            if (name == mapName) return true;
        return false;
    }

    public virtual void SwitchValue(int newValue)
    {
        if (newValue < 0)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) current = keyboard;
            else
#endif
            current = (Sinput.gamepads.Length > 0) ? gamepadDefault : keyboard;
        }
        else current = newValue;
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(SinputGamepadSwitch))]
public class CustomEditor_SinputGamepadSwitch : Editor
{
    SerializedProperty controllersData;
    SerializedProperty current;
    SerializedProperty keyboard;
    SerializedProperty gamepadDefault;

    protected virtual void OnEnable()
    {
        controllersData = serializedObject.FindProperty("controllersData");
        current = serializedObject.FindProperty("current");
        keyboard = serializedObject.FindProperty("keyboard");
        gamepadDefault = serializedObject.FindProperty("gamepadDefault");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(controllersData);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(current);
        EditorGUILayout.PropertyField(keyboard);
        EditorGUILayout.PropertyField(gamepadDefault);
        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
            ((SinputGamepadSwitch)serializedObject.targetObject).SwitchValue(current.intValue);
    }
}
#endif

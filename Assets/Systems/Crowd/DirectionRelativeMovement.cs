using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DirectionRelativeMovement : MonoBehaviour
{
    CharacterController controller;
    Rigidbody rigid;
    Rigidbody2D rigid2D;

    [SerializeField]
    protected float speed = 4f;
    [SerializeField]
    protected float spdChangeAccel = 10f;
    [Tooltip("Always XY when using Rigidbody2D")]
    public Vector3 plane2DNormal = Vector3.up;
    [Tooltip("Always XY when using Rigidbody2D")]
    public Vector3 plane2DUp = Vector3.forward;
    [SerializeField]
    bool charControllerGravity = false;
    [SerializeField]
    TimeMode timeMode = TimeMode.Update;

    int moveMode = 0;
    Coroutine accelerator;

    void OnDisable()
    {
    }

    protected virtual void Awake()
    {
        plane2DNormal = plane2DNormal.normalized;
        plane2DUp = plane2DUp.normalized;
        controller = GetComponent<CharacterController>();
        if (controller != null) moveMode = 1;
        else
        {
            rigid = GetComponent<Rigidbody>();
            if (rigid != null) moveMode = 2;
            else
            {
                rigid2D = GetComponent<Rigidbody2D>();
                if (rigid2D != null) moveMode = 3;
            }
        }
    }

    Vector3 GetWorldVector(Vector3 relativeDirection)
    {
        return transform.rotation * relativeDirection * speed;
    }

    public void MoveRelative(Vector3 direction)
    {
        if (isActiveAndEnabled)
        {
            float deltaTime = TimeDelta(timeMode);
            Vector3 worldSpd = GetWorldVector(direction);
            switch (moveMode)
            {
                case 1:
                    if (controller && controller.enabled)
                    {
                        if (charControllerGravity) controller.SimpleMove(worldSpd); //TO DO: Should be using custom gravity
                        else controller.Move(worldSpd * deltaTime);
                        break;
                    }
                    goto default;
                case 2:
                    if (rigid) //TO DO: Doesn't work
                    {
                        Vector3 spd = worldSpd * deltaTime;
                        rigid.velocity += spd;
                        StartCoroutine(ResetRigid(spd));
                        break;
                    }
                    goto default;
                case 3:
                    if (rigid2D) //TO DO: Doesn't work
                    {
                        Vector2 spd = worldSpd * deltaTime;
                        rigid2D.velocity += spd;
                        StartCoroutine(ResetRigid2D(spd));
                        break;
                    }
                    goto default;
                default:
                    transform.Translate(worldSpd * deltaTime, Space.World);
                    break;
            }
        }
    }

    public void MoveRelative(Vector2 direction)
    {
        MoveRelative(InterpretVector2(direction, plane2DNormal, plane2DUp));
    }

    public void ChangeSpeed(float target)
    {
        if (accelerator != null) StopCoroutine(accelerator);
        accelerator = StartCoroutine(AccelerateTowards(target, spdChangeAccel));
    }

    IEnumerator AccelerateTowards(float targetSpeed, float accel)
    {
        while (speed != targetSpeed)
        {
            if (speed < targetSpeed) speed = Mathf.Min(targetSpeed, speed + accel);
            else speed = Mathf.Max(targetSpeed, speed - accel);
            yield return null;
        }
    }

    IEnumerator ResetRigid(Vector3 speed)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        rigid.velocity -= speed;
    }

    IEnumerator ResetRigid2D(Vector2 speed)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        rigid2D.velocity -= speed;
    }

    //TO DO: Theres probably a better mathematical way of doing this
    Vector3 InterpretVector2(Vector2 input, Vector3 planeNormal, Vector3 planeUp)
    {
        float vectorAngle = -Vector2.SignedAngle(Vector2.up, input);
        return Quaternion.AngleAxis(vectorAngle, planeNormal) * planeUp * input.magnitude;
    }

    float TimeDelta(TimeMode mode)
    {
        switch (mode)
        {
            case TimeMode.FixedUpdate:
                return Time.fixedDeltaTime;
            case TimeMode.Unscaled:
                return Time.unscaledDeltaTime;
            default:
                return Time.deltaTime;
        }
    }

    enum TimeMode { Update, FixedUpdate, Unscaled }
}

#if UNITY_EDITOR
//[CanEditMultipleObjects]
//[CustomEditor(typeof(DirectionRelativeMovement))]
//public class DirectionRelativeMovement_Inspector : Editor
//{
//    SerializedProperty speed;
//    SerializedProperty accel;
//    SerializedProperty plane2DNormal;
//    SerializedProperty plane2DUp;
//    SerializedProperty charControllerGravity;
//    SerializedProperty timeMode;

//    void OnEnable()
//    {
//        speed = serializedObject.FindProperty("speed");
//        accel = serializedObject.FindProperty("accel");
//        plane2DNormal = serializedObject.FindProperty("plane2DNormal");
//        plane2DUp = serializedObject.FindProperty("plane2DUp");
//        charControllerGravity = serializedObject.FindProperty("charControllerGravity");
//        timeMode = serializedObject.FindProperty("timeMode");
//    }

//    public override void OnInspectorGUI()
//    {
//        EditorGUILayout.PropertyField(speed);
//        EditorGUILayout.PropertyField(accel);
//        EditorGUILayout.PropertyField(plane2DNormal);
//        EditorGUILayout.PropertyField(plane2DUp);
//        EditorGUILayout.PropertyField(charControllerGravity);
//        EditorGUILayout.PropertyField(timeMode);

//        serializedObject.ApplyModifiedProperties();
//    }
//}
#endif

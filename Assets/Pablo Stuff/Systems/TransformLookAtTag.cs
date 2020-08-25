using UnityEngine;

[ExecuteInEditMode]
public class TransformLookAtTag : MonoBehaviour
{
    [SerializeField]
    string targetTag = "MainCamera";
    [SerializeField]
    float X = 20f;
    [SerializeField]
    float Y = 180f;
    [SerializeField]
    float Z = 0f;

    Transform target;

    void OnEnable()
    {
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
    }

    void Update()
    {
        transform.LookAt(target);
        Vector3 lookAtEuler = new Vector3(
            Mathf.Repeat(transform.localEulerAngles.x + 180f, 360f) - 180f,
            Mathf.Repeat(transform.localEulerAngles.y + 180f, 360f) - 180f,
            Mathf.Repeat(transform.localEulerAngles.z + 180f, 360f) - 180f
            );
        transform.localEulerAngles = new Vector3(
            (Mathf.Abs(lookAtEuler.x) > X) ? (Mathf.Sign(lookAtEuler.x) * X) : lookAtEuler.x,
            (Mathf.Abs(lookAtEuler.y) > Y) ? (Mathf.Sign(lookAtEuler.y) * Y) : lookAtEuler.y,
            (Mathf.Abs(lookAtEuler.z) > Z) ? (Mathf.Sign(lookAtEuler.z) * Z) : lookAtEuler.z
            );
    }
}

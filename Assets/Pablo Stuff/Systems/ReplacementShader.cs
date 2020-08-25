using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ReplacementShader : MonoBehaviour
{
    Camera cam;

    [SerializeField]
    Shader shader = null;
    [SerializeField]
    string shaderTag = "";

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        cam.SetReplacementShader(shader, shaderTag);
    }

    private void OnDisable()
    {
        cam.ResetReplacementShader();
    }
}

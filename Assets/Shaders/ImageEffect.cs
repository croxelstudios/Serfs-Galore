using UnityEngine;

[ExecuteInEditMode]
public class ImageEffect : MonoBehaviour
{
    [SerializeField]
    Material material = null;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null) Graphics.Blit(source, destination, material);
        else Graphics.Blit(source, destination);
    }
}

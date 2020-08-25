using UnityEngine;

public class Animator_SetFloatParameter : MonoBehaviour
{
    [SerializeField] //TO DO: search on parent by default
    Animator animator = null;
    [SerializeField]
    string parameter = "Speed";

    public void SetFloat(float input)
    {
        animator.SetFloat(parameter, input);
    }
}

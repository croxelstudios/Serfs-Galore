using UnityEngine;

public class Animator_SetBoolParameter : MonoBehaviour
{
    [SerializeField] //TO DO: search on parent by default
    Animator animator = null;
    [SerializeField]
    string parameter = "Bool";

    public void SetBool(bool input)
    {
        animator.SetBool(parameter, input);
    }
}

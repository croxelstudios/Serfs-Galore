using UnityEngine;
using UnityEngine.Events;

public class StartEvent : MonoBehaviour
{
    [SerializeField]
    UnityEvent actions = null;
    [SerializeField]
    bool launchOnEnable = false;

    void Start()
    {
        if (isActiveAndEnabled && !launchOnEnable) actions?.Invoke();
    }

    void OnEnable()
    {
        if (launchOnEnable) actions?.Invoke();
    }
}

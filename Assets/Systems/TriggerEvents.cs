using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : TriggerManager
{
    [SerializeField]
    UnityEvent entered = null;
    [SerializeField]
    UnityEvent exited = null;

    protected override void OnTrigEnter()
    {
        entered?.Invoke();
    }

    protected override void OnTrigExit()
    {
        exited?.Invoke();
    }
}

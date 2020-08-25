using UnityEngine.Events;
using UnityEngine;

public class EventsLauncher : MonoBehaviour
{
    public UnityEvent[] events;

    void OnDisable()
    {
    }

    public void LaunchEvent(int eventID)
    {
        if (isActiveAndEnabled) events[eventID]?.Invoke();
    }
}

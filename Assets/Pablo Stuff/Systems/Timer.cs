using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Timer : MonoBehaviour
{
    [SerializeField]
    protected float seconds = 1f;
    [SerializeField]
    UnityEvent actions = null;
    [SerializeField]
    bool unscaledTime = false;
    [SerializeField]
    bool startOnEnable = false;

    void OnEnable()
    {
        if (startOnEnable) StartTimer();
    }

    void OnDisable()
    {
        StopTimer();
    }

    public void StopTimer()
    {
        StopAllCoroutines();
    }

    public virtual void StartTimer()
    {
        StartCoroutine(Alarm());
    }

    IEnumerator Alarm()
    {
        if (unscaledTime) yield return new WaitForSecondsRealtime(seconds);
        else yield return new WaitForSeconds(seconds);
        actions?.Invoke();
    }
}

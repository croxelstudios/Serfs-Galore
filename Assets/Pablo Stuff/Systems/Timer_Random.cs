using UnityEngine;

public class Timer_Random : Timer
{
    [SerializeField]
    float secondsMax = 2f;
    float secondsMin;

    void Awake()
    {
        secondsMin = seconds;
    }

    public override void StartTimer()
    {
        seconds = Random.Range(secondsMin, secondsMax);
        base.StartTimer();
    }
}

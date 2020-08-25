using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField]
    float seconds = 3f;

    void Update()
    {
        if (seconds <= 0f) Destroy(gameObject);
        seconds -= Time.deltaTime;
    }
}

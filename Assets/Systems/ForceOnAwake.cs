using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceOnAwake : MonoBehaviour
{

    [SerializeField]
    Vector3 forceDirection = Vector3.zero;
    [SerializeField]
    bool random = false;
    [SerializeField]
    float magnitude = 1f;
    [SerializeField]
    float upForceMultiplier = 1f;

    private void Awake()
    {
        if (random) forceDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, upForceMultiplier), Random.Range(-1f, 1f));
        GetComponent<Rigidbody>().AddForce(forceDirection * magnitude);
    }
}

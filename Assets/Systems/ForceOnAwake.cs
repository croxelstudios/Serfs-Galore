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
    float magnitude = 1;
    [SerializeField]
    float upForceMultiplier = 1;

    private void Awake()
    {
        if (random)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), Random.Range(0, 1*upForceMultiplier), Random.Range(-1, 1))*magnitude);
        }
        else
        {
            GetComponent<Rigidbody>().AddForce(forceDirection * magnitude);
        }
    }
}

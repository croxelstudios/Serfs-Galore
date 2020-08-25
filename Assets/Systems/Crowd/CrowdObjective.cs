using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrowdObjective : MonoBehaviour
{

    [SerializeField]
    UnityEvent onCompleted = null;
    // Start is called before the first frame update

    public void Completed()
    {
        onCompleted?.Invoke();
        //Debug.Log("COMPLETED");
    }
}

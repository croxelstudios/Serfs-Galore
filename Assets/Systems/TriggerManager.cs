using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TriggerManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Determines if it should launch events when more than one collision with these tags ocurr")]
    bool fuseColliders = true;
    [SerializeField]
    [Tooltip("Will fire on any collision if this array is empty")]
    string[] detectionTags = null;

    List<Collider> colliders;
    List<Collider2D> colliders2D;

    protected virtual void Awake()
    {
        colliders = new List<Collider>();
        colliders2D = new List<Collider2D>();
    }

    void FixedUpdate()
    {
        for (int i = colliders.Count - 1; i > -1; i--)
        {
            if ((colliders[i] == null) || (!colliders[i].enabled) || !colliders[i].gameObject.activeInHierarchy)
            {
                colliders.RemoveAt(i);
                if ((colliders.Count == 0) || !fuseColliders) OnTrigExit();
            }
        }

        for (int i = colliders2D.Count - 1; i > -1; i--)
        {
            if ((colliders2D[i] == null) || (!colliders2D[i].enabled))
            {
                colliders2D.RemoveAt(i);
                if ((colliders.Count == 0) || !fuseColliders) OnTrigExit();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((detectionTags.Contains(other.tag) || (detectionTags == null)))
        {
            if ((colliders.Count == 0) || !fuseColliders) OnTrigEnter();
            colliders.Add(other);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (detectionTags.Contains(other.tag) || (detectionTags == null))
        {
            if ((colliders.Count == 0) || !fuseColliders) OnTrigEnter();
            colliders2D.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("exit" + other.gameObject.name);
        if (colliders.Contains(other))
        {
            colliders.Remove(other);
            if ((colliders.Count == 0) || !fuseColliders) OnTrigExit();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (colliders2D.Contains(other))
        {
            colliders2D.Remove(other);
            if ((colliders.Count == 0) || !fuseColliders) OnTrigExit();
        }
    }

    protected virtual void OnTrigEnter()
    {

    }

    protected virtual void OnTrigExit()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class CrowdItem : MonoBehaviour
{

    [SerializeField]
    string playerTag = "Player";
    [SerializeField]
    Transform parent = null;
    [SerializeField]
    public int amountToGrab = 5;
    [SerializeField]
    float moveToCenterSpeed = 1;
    [SerializeField]
    float carryHeight = 1.3f;
    [SerializeField]
    string allyTag = "AllyInteraction";

    [SerializeField]
    public Transform objective = null;

    bool grabbed = false;
    bool grabbing;

    List<Transform> interactingElements = new List<Transform>();

    Crowd c;

    [SerializeField]
    UnityEvent onGrabbed = null;
    [SerializeField]
    UnityEvent onUsed = null;

    private void Awake()
    {
        c = GameObject.FindGameObjectWithTag(playerTag).GetComponent<Crowd>();
    }

    public int GetIteractingCount()
    {
        return interactingElements.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform raycastTrigger = null;
        for(int i = 0; i < transform.parent.childCount; i++)
        {
            if(transform.parent.GetChild(i).tag == "RaycastItem")
            {
                raycastTrigger = transform.parent.GetChild(i);
            }
        }
        if (other.tag == allyTag && !grabbed && other.GetComponentInParent<CrowdElement>().currentTarget == raycastTrigger)
        {
            interactingElements.Add(other.transform.parent);
        }

        if(other.transform == objective)
        {
            onUsed?.Invoke();
            //Debug.Log("ITEM USED");
            CrowdObjective co;
            if (!other.transform.parent.GetComponentInChildren<CrowdObjective>())
            {
                co = other.GetComponentInParent<CrowdObjective>();
            }
            else
            {
                co = other.transform.parent.GetComponentInChildren<CrowdObjective>();
            }

            co.Completed();

            foreach(Transform t in interactingElements)
            {
                CrowdElement ce = t.GetComponent<CrowdElement>();
                ce.AssignNewTarget(ce.player.followPoint);
            }
            Destroy(parent.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Transform raycastTrigger = null;
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i).tag == "RaycastItem")
            {
                raycastTrigger = transform.parent.GetChild(i);
            }
        }
        if (other.tag == allyTag && !grabbed && other.GetComponentInParent<CrowdElement>().currentTarget == raycastTrigger)
        {
            if (!interactingElements.Contains(other.transform.parent))
            {
                interactingElements.Add(other.transform.parent);
            }
        }
        else if (other.tag == allyTag && !grabbed && other.GetComponentInParent<CrowdElement>().currentTarget != raycastTrigger)
        {
            interactingElements.Remove(other.transform.parent);
        }
    }

    public void Drop()
    {
        interactingElements.Clear();
        grabbed = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == allyTag && !grabbed)
        {
            interactingElements.Remove(other.transform.parent);
        }
    }

    private void FixedUpdate()
    {
        if (interactingElements.Count >= amountToGrab)
        {
            if (!grabbed) onGrabbed?.Invoke();
            grabbed = true;

            foreach(Transform t in interactingElements)
            {
                t.GetComponent<CrowdElement>().carryingItem = true;
            }
            c.currentItem = this;
            MoveToCenterOfInteracting();
        }
    }

    public Vector3 GetCenterOfInteracting()
    {
        Vector3 center = Vector3.zero;
        foreach (Transform element in interactingElements)
        {
            center += element.position;
        }

        return center / interactingElements.Count;
    }

    public void MoveToCenterOfInteracting()
    {
        parent.transform.position = Vector3.MoveTowards(parent.position, GetCenterOfInteracting(), moveToCenterSpeed * Time.deltaTime);
        parent.transform.position = new Vector3(parent.transform.position.x, carryHeight, parent.transform.position.z);
    }
}

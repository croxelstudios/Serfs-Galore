using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using TMPro;

public class Crowd : MonoBehaviour
{

    public List<CrowdElement> crowd = new List<CrowdElement>();
    public List<CrowdElement> followingCrowd = new List<CrowdElement>();

    [SerializeField]
    string ctgTag = "CTG";

    [SerializeField]
    float minDistanceMultiplier = 1;
    [SerializeField]
    float minDistanceOffset = 1;

    float finalTarget;
    Vector3 finalNormalized;

    public Transform followPoint;
    [SerializeField]
    float radiusMultiplier = 1;
    [SerializeField]
    float radiusOffset = 1;

    Vector3 prevPos;

    public CrowdItem currentItem;

    [HideInInspector]
    public float followRadiusFinal;
    [SerializeField]
    string canvasMinionsTag = "HudMinions";

    TextMeshProUGUI tmpMinions;

    CinemachineTargetGroup ctg;

    private void Awake()
    {
        ctg = GameObject.FindGameObjectWithTag(ctgTag).GetComponent<CinemachineTargetGroup>();
        
        //Pablo cambió esto
        GameObject followPointGO = new GameObject();
        followPointGO.name = "FollowPoint";
        followPoint = followPointGO.transform;
        followPoint.parent = transform;
        followPoint.localPosition = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (prevPos != transform.position)
        {
            StartCoroutine(CalculateMinDistance(transform.position));
            followRadiusFinal = radiusOffset + (followingCrowd.Count * radiusMultiplier);
        }
        if(!tmpMinions)
            tmpMinions = GameObject.FindGameObjectWithTag(canvasMinionsTag)?.GetComponent<TextMeshProUGUI>();
        if (tmpMinions)
        {
            tmpMinions.text = crowd.Count().ToString();
        }
        prevPos = transform.position;
    }

    public int CurrentCrowdQuantity()
    {
        return crowd.Count();
    }

    public void AddElementToCrowd(CrowdElement ce)
    {
        //ce.AssignNewTarget(followPoint);
        ce.currentlyInCrowd = true;
        if (!crowd.Contains(ce))
        {
            crowd.Add(ce);
            ce.onNewElementInCrowd?.Invoke();
        }
        if (!followingCrowd.Contains(ce))   
            followingCrowd.Add(ce);

        bool inGroup = false;
        foreach(CinemachineTargetGroup.Target t in ctg.m_Targets)
        {
            if (t.target.transform == ce.transform)
            {
                inGroup = true;
                break;
            }

        }
        if (!inGroup)
            ctg.AddMember(ce.transform, 1, 0);

    }

    public void AssignTargetToRandomElement(Transform target)
    {
        CrowdElement ce = target.GetComponentInParent<CrowdElement>();
        if (ce)
        {
            if (!ce.sl.ded && ce.elementEnabled)
            {
                ce.AssignNewTarget(followPoint);
                if (!followingCrowd.Contains(ce))
                    followingCrowd.Add(ce);
            }
        }
        else
        {
            CrowdObjective co = target.parent.GetComponentInChildren<CrowdObjective>();
            if (co)
            {
                if (currentItem && (currentItem.objective.parent == target.parent))
                {
                    List<CrowdElement> ceToRemove = new List<CrowdElement>();
                    for (int i = 0; i < followingCrowd.Count; i++)
                    {
                        if (followingCrowd[i].carryingItem)
                        {
                            followingCrowd[i].AssignNewTarget(target);
                            ceToRemove.Add(followingCrowd[i]);
                            //followingCrowd.RemoveAt(i);
                        }
                    }
                    foreach (CrowdElement ceInList in ceToRemove)
                    {
                        followingCrowd.Remove(ceInList);
                    }
                }
            }
            else
            {
                if (crowd.Count > 0 && crowd.Count > CountCarryingElements())//Pablo: Ponía followingCrowd antes ambas veces en vez de crowd
                {
                    // Pablo ha tocao esto
                    bool noFollowerAvailable = true;
                    //
                    foreach (CrowdElement cee in followingCrowd)
                    {
                        if (!cee.carryingItem)
                        {
                            cee.AssignNewTarget(target);
                            followingCrowd.Remove(cee);
                            //
                            noFollowerAvailable = false;
                            //
                            break;
                        }
                    }
                    //
                    if (noFollowerAvailable)
                        foreach (CrowdElement cee in crowd)
                        {
                            if (!cee.carryingItem)
                            {
                                cee.AssignNewTarget(target);
                                break;
                            }
                        }
                    //
                }
            }
        }
    }

    public void CheckCarryingItem(CrowdElement ce)
    {
        if (ce.carryingItem)
        {
            foreach(CrowdElement cee in followingCrowd)
            {
                if (cee.carryingItem)
                {
                    cee.carryingItem = false;
                    cee.AssignNewTarget(followPoint);
                }
            }
            if (currentItem)
            {
                currentItem.Drop();
            }
        }
    }

    int CountCarryingElements()
    {
        int total = 0;
        foreach(CrowdElement ce in followingCrowd)
        {
            if (ce.carryingItem)
            {
                total++;
            }
        }

        return total;
    }

    public void RemoveElementFromCrowd(CrowdElement ce)
    {
        crowd.Remove(ce);
        followingCrowd.Remove(ce);
        ce.currentlyInCrowd = false;
        ctg.RemoveMember(ce.transform);
    }

    IEnumerator CalculateMinDistance(Vector3 pos)
    {
        Vector3 currentFinalPos = pos;
        yield return new WaitForEndOfFrame();

        finalNormalized = (currentFinalPos - transform.position).normalized * ((minDistanceMultiplier * followingCrowd.Count) + minDistanceOffset);

        if (finalNormalized.magnitude > Mathf.Epsilon)
            followPoint.localPosition = finalNormalized;
    }
}

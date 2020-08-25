using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using System;

[RequireComponent(typeof(DirectionRelativeMovement))]
public class CrowdElement : MonoBehaviour
{

    [SerializeField]
    string playerTag = "Player";

    [SerializeField]
    public Transform currentTarget;
    [SerializeField]
    public bool elementEnabled = false;

    [SerializeField]
    float maxSpeed = 1;
    [SerializeField]
    float acceleration = 1;
    [SerializeField]
    float decceleration = 5;
    [SerializeField]
    float distanceToStartDecceleration = 1;
    [SerializeField]
    float distanceToStartDeccelerationWithObject = 0.1f;
    [SerializeField]
    float maxDistanceInCrowd = 20;

    float currentSpeed = 0;

    [HideInInspector]
    public SimpleLife sl;

    [SerializeField]
    public int price = 10;
    DirectionRelativeMovement drm;

    [SerializeField]
    float getOutOfCollisionOffset = 0.01f;
    Vector3 prevPos;
    [HideInInspector]
    public bool currentlyInCrowd = false;

    public bool carryingItem = false;

    [SerializeField]
    public UnityEvent onNewElementInCrowd = null;
    [SerializeField]
    public UnityEvent onNewOrder = null;
    [SerializeField]
    float minMagnitude = 0.01f;
    [SerializeField]
    FloatEvent magnitude = null;
    [SerializeField]
    UnityEvent onLostFromCrowd = null;




    public Crowd player;

    CharacterController cc = null;

    // Start is called before the first frame update
    void Start()
    {
        drm = GetComponent<DirectionRelativeMovement>();
        sl = GetComponentInChildren<SimpleLife>();
        player = GameObject.FindGameObjectWithTag(playerTag).GetComponent<Crowd>();
        cc = GetComponent<CharacterController>();
    }

    public void GetOutOfCollision()
    {
        Vector3 startPoint = new Vector3(transform.position.x, transform.position.y - cc.height, transform.position.z);
        Vector3 endPoint = new Vector3(transform.position.x, transform.position.y + cc.height, transform.position.z);
        Vector3 vx = new Vector3((cc.radius * 2) + getOutOfCollisionOffset, 0, 0);
        Vector3 vz = new Vector3(0, 0, (cc.radius * 2) + getOutOfCollisionOffset);
        Collider[] c1;
        LayerMask lm = LayerMask.GetMask("Solid", "Minion");
        c1 = Physics.OverlapCapsule(startPoint + vx, endPoint + vx, cc.radius, lm);
        if (c1.Length == 0)
        {
            transform.position = transform.position + vx;
        }
        c1 = Physics.OverlapCapsule(startPoint - vx, endPoint - vx, cc.radius, lm);
        if (c1.Length == 0)
        {
            transform.position = transform.position - vx;
        }
        c1 = Physics.OverlapCapsule(startPoint + vz, endPoint + vz, cc.radius, lm);
        if (c1.Length == 0)
        {
            transform.position = transform.position + vz;
        }
        c1 = Physics.OverlapCapsule(startPoint - vz, endPoint - vz, cc.radius, lm);
        if (c1.Length == 0)
        {
            transform.position = transform.position - vz;
        }

        
    }

    void CheckOverlap()
    {
        Vector3 startPoint = new Vector3(transform.position.x, transform.position.y - cc.height, transform.position.z);
        Vector3 endPoint = new Vector3(transform.position.x, transform.position.y + cc.height, transform.position.z);
        Collider[] c1;
        LayerMask lm = LayerMask.GetMask("Solid", "Minion");
        c1 = Physics.OverlapCapsule(startPoint, endPoint, cc.radius/3, lm);
        if (c1.Length > 1 && UnityEngine.Random.Range(0,15)==5)
        {
            GetOutOfCollision();
        }
    }

    void FixedUpdate()
    {

        if (carryingItem && currentTarget == player.followPoint && Vector3.Distance(transform.position, currentTarget.position) > 10)
        {
            //VAYA COSA FEA LOKO | THIS IS UGLY AF
            Vector3 oldPos = transform.position;
            transform.position = (transform.position + currentTarget.position) / 2;
            transform.position = (transform.position + oldPos) / 2;
        }
        else
        {
            if (elementEnabled && !sl.ded)
            {
                if (GetComponentInChildren<Canvas>())
                {
                    GetComponentInChildren<Canvas>().gameObject.SetActive(false);
                }
                if (carryingItem && currentTarget != player.followPoint && !currentTarget.parent.GetComponentInChildren<CrowdObjective>())
                {
                    AssignNewTarget(player.followPoint);
                }
                if (currentTarget != null)
                {
                    //Debug.Log(currentTarget.GetComponentInParent<CrowdEnemy>());
                    if (currentTarget.GetComponentInParent<CrowdEnemy>() && !currentTarget.GetComponentInParent<CrowdEnemy>().enabled)
                    {
                        currentlyInCrowd = true;
                        AssignNewTarget(player.followPoint);
                    }
                    FollowTarget();
                }
                else
                {
                    currentlyInCrowd = true;
                    AssignNewTarget(player.followPoint);
                }

                if (Vector3.Distance(transform.position, player.followPoint.position) > maxDistanceInCrowd)
                {
                    RemoveThisFromCrowd();
                    onLostFromCrowd?.Invoke();
                }
            }
        }
        CheckOverlap();
        prevPos = transform.position;
    }

    public void AssignNewTarget(Transform target)
    {
        if (!sl.ded /*&& elementEnabled */&& currentlyInCrowd)
        {
            currentTarget = target;
            sl.CheckTarget(target);
            currentSpeed = 0;
            onNewOrder?.Invoke();
            if (target == player.followPoint)
            {
                carryingItem = false;
                player.AddElementToCrowd(this);
            }
        }
    }

    void FollowTarget()
    {
        Crowd player = GameObject.FindGameObjectWithTag(playerTag).GetComponent<Crowd>();
        if (currentTarget)
        {
            if (currentTarget == player.followPoint)
            {
                if (Vector3.Distance(currentTarget.position, transform.position) > distanceToStartDecceleration && Vector3.Distance(transform.position, player.transform.position) > player.followRadiusFinal)
                    currentSpeed += acceleration * Time.fixedDeltaTime;
                else if (currentSpeed > 0)
                    currentSpeed -= decceleration * Time.fixedDeltaTime;
                if (currentSpeed > maxSpeed)
                    currentSpeed = maxSpeed;
                if (currentSpeed < 0)
                    currentSpeed = 0;
            }
            else
            {
                if (currentTarget.GetComponent<CrowdItem>())
                {
                    if (Vector3.Distance(currentTarget.position, transform.position) > distanceToStartDeccelerationWithObject)
                        currentSpeed += acceleration * Time.fixedDeltaTime;
                    else if (currentSpeed > 0)
                        currentSpeed -= decceleration * Time.fixedDeltaTime;
                    if (currentSpeed > maxSpeed)
                        currentSpeed = maxSpeed;
                    if (currentSpeed < 0)
                        currentSpeed = 0;
                }
                else
                {
                    if (Vector3.Distance(currentTarget.position, transform.position) > distanceToStartDecceleration)
                        currentSpeed += acceleration * Time.fixedDeltaTime;
                    else if (currentSpeed > 0)
                        currentSpeed -= decceleration * Time.fixedDeltaTime;
                    if (currentSpeed > maxSpeed)
                        currentSpeed = maxSpeed;
                    if (currentSpeed < 0)
                        currentSpeed = 0;
                }
            }

            Vector2 newDirection = new Vector2(currentTarget.position.x - transform.position.x, currentTarget.position.z - transform.position.z).normalized * currentSpeed;
            drm.MoveRelative(newDirection);
            float mag = Vector3.Distance(prevPos, transform.position) / Time.fixedDeltaTime;
            if (mag > minMagnitude) magnitude?.Invoke(mag);
            else magnitude?.Invoke(0f);
        }
        else
        {
            //if (!player.crowd.Contains(this))
            //{
            //    player.AddElementToCrowd(this);
            //}
            AssignNewTarget(player.followPoint);
        }
    }

    public void RemoveThisFromCrowd()
    {
        player.GetComponent<Crowd>().RemoveElementFromCrowd(this);
        elementEnabled = false;
        //if(currentTarget == player.followPoint)
        //{
        currentTarget = null;
        //}
    }

    public int Buy(int coins)
    {
        if (coins >= price)
        {
            int originalPrice = price;
            price = 0;
            elementEnabled = true;
            sl.Revive();
            return -originalPrice;
        }
        return 0;
    }

    [Serializable]
    class FloatEvent : UnityEvent<float> { }
    [Serializable]
    class VectorEvent : UnityEvent<Vector2> { }
}
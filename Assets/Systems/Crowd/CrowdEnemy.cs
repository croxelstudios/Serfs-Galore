using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CrowdEnemy : MonoBehaviour
{
    [SerializeField]
    string playerTag = "Player";

    [SerializeField]
    Transform currentTarget;
    [SerializeField]
    Behaviour behaviour = Behaviour.idle;

    [SerializeField]
    SimpleLife sl;
    DirectionRelativeMovement drm;

    public UnityEvent onNewTarget;

    // Start is called before the first frame update
    void Start()
    {
        drm = GetComponent<DirectionRelativeMovement>();
    }

    void FixedUpdate()
    {
        switch (behaviour)
        {
            case Behaviour.idle:

                break;
            case Behaviour.focusPlayer:
                if (currentTarget != null)
                {
                    FollowTarget();
                }
                else
                {
                    Transform player = GameObject.FindGameObjectWithTag(playerTag).transform;
                    AssignNewTarget(player.transform);
                }
                break;
        }
    }

    public void AssignNewTarget(Transform target)
    {
        currentTarget = target;
        onNewTarget?.Invoke();
    }

    void FollowTarget()
    {
        if (currentTarget)
        {
            drm.MoveRelative(new Vector2(currentTarget.position.x - transform.position.x, currentTarget.position.z - transform.position.z).normalized);
        }
        else
        {
            Transform player = GameObject.FindGameObjectWithTag(playerTag).transform;
            AssignNewTarget(player.transform);
        }
    }


}

public enum Behaviour
{
    idle,
    focusPlayer
}
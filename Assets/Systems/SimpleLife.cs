using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleLife : MonoBehaviour
{

    [SerializeField]
    string foeTag = "";
    [HideInInspector]
    public float life = 10;
    [SerializeField]
    public float maxLife = 10;
    [SerializeField]
    float dps = 1;

    [SerializeField]
    bool attacking = false;

    [SerializeField]
    SimpleLife target = null;

    public UnityEvent onGetDamage;
    public UnityEvent onMakeDamage;
    public UnityEvent onDie;
    public UnityEvent onRevive;

    float timer = 0;

    [HideInInspector]
    public bool ded = false;

    private void Awake()
    {
        life = maxLife;

        /*if (GetComponentInParent<CrowdEnemy>())
        {
            attacking = true;
        }*/
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (!ded && attacking)
        {
            if (timer >= 1)
            {
                if (target)
                {
                    CheckTarget(target.transform);
                    //Debug.Log(attacking);
                    MakeDamage();
                }
                timer = 0;
            }
        }
    }

    public void GetDamage(float amount)
    {
        if (!ded)
        {
            onGetDamage?.Invoke();
            //Debug.Log(name + ": " + life);
            life -= amount;
            if (life <= 0)
                Die();
        }
    }
    public void CheckTarget(Transform target)
    {
        if (GetComponentInParent<CrowdEnemy>() && target.tag == foeTag)
        {
            if (this.target && this.target.ded)
            {
                attacking = false;
                //Debug.Log("false1");
            }
        }
        else
        {
            if (target.tag == foeTag || target.tag == "RaycastEnemy")
            {
                attacking = true;
            }
            else
            {
                attacking = false;
                //Debug.Log("false1");
            }
        }
        
    }

    public void MakeDamage()
    {
        if (target && !target.ded)
        {
            if (GetComponentInParent<CrowdElement>() && target.tag == "Player")
            {
                
            }
            else
            {
                onMakeDamage?.Invoke();
                target.GetDamage(dps);
            }
            //Debug.Log("attack");
        }
    }

    public void Die()
    {
        if (!ded)
        {
            ded = true;
            onDie?.Invoke();
            if (transform.tag == "Player")
            {
                //Debug.Log("GAMEOVER");
            }
            else
            {
                CrowdElement ce = GetComponentInParent<CrowdElement>();
                if (ce)
                {
                    ce.player.CheckCarryingItem(ce);
                    ce.RemoveThisFromCrowd();
                }
            }
        }
        
        //Debug.Log("ded");
    }

    public void SetTarget(SimpleLife target)
    {
        if (!target.ded)
            this.target = target;
    }

    public void Revive()
    {
        if (ded)
            onRevive?.Invoke();
        ded = false;
        life = maxLife;
        timer = 0;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        SimpleLife sl = other.GetComponent<SimpleLife>();
        //Debug.Log(transform.parent.name + ": " + other.transform.parent.name);
        if (GetComponentInParent<CrowdEnemy>())
        {
            if (other.tag == "Player" || (other.tag == foeTag && other.GetComponentInParent<CrowdElement>().elementEnabled))
            {
                target = sl;
                attacking = true;
                //Debug.Log("BBBBBBBB");
            }
        }
        else
        {
            if ((other.tag == foeTag) && sl && !sl.ded && target && target.tag != "Player")
            {
                target = sl;
                //Debug.Log(transform.parent.name + ": " + target.transform.parent.name);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        SimpleLife sl = other.GetComponent<SimpleLife>();
        if ((target == null || target.ded) && (other.tag == foeTag || other.tag == "Player") && sl && !sl.ded)
        {
            target = sl;
            if (GetComponentInParent<CrowdEnemy>() && other.GetComponentInParent<CrowdElement>() && other.GetComponentInParent<CrowdElement>().elementEnabled)
            {
                attacking = true;
                //Debug.Log("AAAAAA");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == foeTag || other.tag == "Player")
        {
            target = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Units : GamePieces
{

    public UnitData unit;

    int health;

    //Objects to interact with.
    GameObject targetedObject;
    Vector3 locationToMoveTo;

    //Booleans for determining actions.
    public bool isSelected;
    bool isAttacking;
    bool isMoving;

    //The nav mesh agent.
    NavMeshAgent m_Agent;

    //overhead text
    public TextMeshProUGUI selectedText; //Unit text needs to look at camera otherwse its weird.
    // Start is called before the first frame update
    void Start()
    {
        health = unit.maxHealth;
        m_Agent= GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //I think these are gonna wanna be coroutines, for now I'm just getting the code down.
        if (isAttacking)
        {
            if (targetedObject != null)
            {
                AttackTarget(targetedObject);
            }
            else
            {
                isAttacking= false;
            }
        }

        else if (isMoving)
        {
            if(!m_Agent.isStopped)
            {
                MoveToLocation(locationToMoveTo);
            }
            else
            {
                isMoving = false;
            }
        }

        else
        {
            //CheckForEnemies();
        }
        selectedText.text = "Selected: " + isSelected;
    }

    void MoveToLocation(Vector3 location)
    {
        if (!isSelected)
        {
            return;
        }
        //Sets nav agent destination to the targeted location.
        m_Agent.destination = location;
    }

    void AttackTarget(GameObject target)
    {
        //While the unit is too far from the target, move towards the target. Otherwise, attack the target.
        float distance = Vector3.Distance(gameObject.transform.position, target.transform.position);

        if (distance < unit.attackRange)
        {
            StartCoroutine(Attack(target));
        }
        else
        {
            MoveToLocation(target.transform.position);
        }
    }

    IEnumerator Attack(GameObject target)
    {
        if (target.GetComponentInParent<Units>() != null)
        {
            target.GetComponent<Units>().TakeDamage(unit.damage);
        }
        else if (target.GetComponentInParent<Structures>() != null)
        { 
            target.GetComponent<Structures>().TakeDamage(unit.damage);

            //I don't think this needs to be here anymore.
            if (target.GetComponent<Structures>() == null)
            {
                isAttacking = false;
                target = null;
            }
        }
        else if (target.GetComponentInParent<ResourceEntities>() != null)
        {
            target.GetComponent<ResourceEntities>().TakeDamage(unit.damage);
        }
        yield return new WaitForSeconds(unit.timeBetweenAttacks);
    }

    void TakeDamage(int damage)
    {
        //Take damage from some external source.
        health -= damage;

        if (health <= 0)
        {
            SlayUnit();
        }
    }

    void SlayUnit()
    {
        //Unit dies, probably destroy it and do some stuff on death.
        Debug.Log("dead");
    }
}

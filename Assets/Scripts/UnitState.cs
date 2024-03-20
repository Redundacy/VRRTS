using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitState : MonoBehaviour
{
    //Unit type, responsible for stats and whatnot.
    public string unitType;
    
    //Various stats.
    float health;
    public float maxHealth;
    public float damage;
    public float maxAttackRange;
    public float vision = 5;

    //Objects to interact with.
    GameObject targetedObject;
    Vector3 locationToMoveTo;

    //Booleans for determining actions.
    public bool isSelected;
    bool isAttacking;
    bool isMoving;

    //The nav mesh agent.
    NavMeshAgent m_Agent;

    // Start is called before the first frame update
    void Start()
    {
        switch(unitType)
        {
            case "Basic":

                maxHealth = 100;
                damage = 30;
                maxAttackRange = 1;
                break;

            default:
                break;
        }

        health = maxHealth;
        m_Agent = GetComponent<NavMeshAgent>();
        PlayerActions.MoveSelectedUnits += MoveToLocation;
    }

    // Update is called once per frame
    void Update()
    {
        //I think these are gonna wanna be coroutines, for now I'm just getting the code down.
        if (isAttacking)
        {
            AttackTarget(targetedObject);
        }

        else if (isMoving)
        {
            MoveToLocation(locationToMoveTo);
        }

        else
        {
            CheckForEnemies();
        }
    }

    void CheckForEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, vision); //Add a layermask here to only interact with units

        if (hitColliders.Length != 0)
        {
            targetedObject = hitColliders[0].gameObject;
            isAttacking = true;
        }
    }

    void MoveToLocation(Vector3 location)
    {
        if(!isSelected)
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

        if (distance < maxAttackRange)
        {
            Attack(target);
        } else
        {
            MoveToLocation(target.transform.position);
        }
    }

    void Attack(GameObject target)
    {
        target.GetComponent<UnitState>().TakeDamage(damage);
    }

    void InteractWithThing()
    {
        //Will likely be used for collectin resources, for now implementation is unclear.
    }

    void TakeDamage(float damage)
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
    }
}

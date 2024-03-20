using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyUnit : MonoBehaviour
{

    //So like, there's a lot of code here that is just copied from UnitState, which makes me think that maybe the two scripts could be combined? Not worrying about that now but it's a thought.

    //Unit type, responsible for stats and whatnot.
    public string unitType;

    //Various stats.
    float health;
    float maxHealth;
    float damage;
    public float maxAttackRange;
    public float vision; //Used to see units to attack.

    //Objects to interact with.
    GameObject targetedObject;
    Vector3 locationToMoveTo;

    //Booleans for determining actions.
    bool isAttacking;
    bool isMoving;

    //The nav mesh agent.
    NavMeshAgent m_Agent;

    // Start is called before the first frame update
    void Start()
    {
        switch (unitType)
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttacking)
        {
            CheckForEnemies();
        }
        else
        {
            AttackTarget(targetedObject);
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

    void AttackTarget(GameObject target)
    {
        //While the unit is too far from the target, move towards the target. Otherwise, attack the target.
        float distance = Vector3.Distance(gameObject.transform.position, target.transform.position);

        if (distance < maxAttackRange)
        {
            Attack(target);
        }
        else
        {
            MoveToLocation(target.transform.position);
        }
    }

    void Attack(GameObject target)
    {
        target.GetComponent<UnitState>
    }

    void MoveToLocation(Vector3 target)
    {
        m_Agent.destination = target;
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

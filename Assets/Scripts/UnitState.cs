using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class UnitState : MonoBehaviour
{
    //Unit type, responsible for stats and whatnot.
    public string unitType;

    public enum Team
    {
        AlliedTeam,
        EnemyTeam,
        Hostile,
        Neutral
    }
    
    //Various stats.
    float health;
    public float maxHealth;
    public float damage;
    public float maxAttackRange;
    public float vision = 5;

    //Objects to interact with.
    GameObject targetedObject;
    Vector3 locationToMoveTo;
    public LayerMask hostileTargets;

    //Booleans for determining actions.
    public bool isSelected;
    Outline outline;
    bool isAttacking;
    bool isMoving;

    //The nav mesh agent.
    NavMeshAgent m_Agent;

    //overhead text
    public TextMeshProUGUI selectedText; //Unit text needs to look at camera otherwse its weird.

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
        //set color based on team

        // always stuff
        m_Agent = GetComponent<NavMeshAgent>();
        outline = GetComponent<Outline>();
        PlayerActions.MoveSelectedUnits += MoveToLocation;
        PlayerActions.TryAttackObject += SetAttacks;
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
            //CheckForEnemies();
        }
        selectedText.text = "Selected: " + isSelected;
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        outline.enabled = !outline.enabled;
    }

    void CheckForEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, vision, hostileTargets); //Add a layermask here to only interact with units

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

    void SetAttacks(GameObject target)
    {
        if(target.tag == "Object")
        {
            isAttacking = true;
            targetedObject = target;
        }
        
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
        if(target.GetComponentInParent<UnitState>() != null)
        {
            target.GetComponent<UnitState>().TakeDamage(damage);
        } else
        {
            target.GetComponent<Structures>().TakeDamage(damage);
            if(target.GetComponent<Structures>() == null)
            {
                isAttacking = false;
                target = null;
            }
        }
        
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
        Debug.Log("dead");
    }
}

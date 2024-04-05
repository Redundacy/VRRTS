using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Matchmaker.Models;
using UnityEditor;
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
    Outline outline;
    bool isAttacking;
    bool isMoving;
    bool movingToAttack;

    //The nav mesh agent.
    NavMeshAgent m_Agent;

    public LayerMask onlyTargetGamePieces;
    
    public TextMeshProUGUI healthBarText;
    public Canvas healthBar;

    //overhead text
    //public TextMeshProUGUI selectedText; //Unit text needs to look at camera otherwse its weird.
    // Start is called before the first frame update
    void Start()
    {
        //health = unit.maxHealth;
        m_Agent= GetComponent<NavMeshAgent>();
        outline = GetComponent<Outline>();
        PlayerActions.MoveSelectedUnits += MoveToLocation;
        PlayerActions.TryAttackObject += AttackTarget;

        healthBarText.text = "Health: " + health + "/" + unit.maxHealth;
    }

    public void InitializeData()
    {
        health = unit.maxHealth;
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
            CheckForEnemies();
        }
        //selectedText.text = "Selected: " + isSelected;

        Vector3 lookDirection = healthBar.transform.position - GameObject.Find("VRRTS Player").transform.position;
        healthBar.transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        outline.enabled = !outline.enabled;
        Debug.Log(isSelected + "\n" + outline.enabled);
    }

    void CheckForEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, unit.visionRange, onlyTargetGamePieces);

        if (hitColliders.Length != 0)
        {
            foreach (Collider collider in hitColliders)
            {
                if ((collider.gameObject.GetComponentInParent<Units>() && collider.gameObject.GetComponentInParent<Units>().unit.GetTeamString() != "EnemyTeam") 
                    || (collider.gameObject.GetComponent<Structures>() && collider.gameObject.GetComponent<Structures>().structure.GetTeamString() != "EnemyTeam"))
                {
                    //Do nothing
                } else
                {
                    targetedObject = hitColliders[0].gameObject;
                    isAttacking = true;
                }
            }
        }
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
        targetedObject = target;
        isAttacking = true;
        Debug.Log("omw to " + target.name);
        //While the unit is too far from the target, move towards the target. Otherwise, attack the target.
        float distance = Vector3.Distance(gameObject.transform.position, target.transform.position);
        //Debug.Log(distance + " and " + unit.attackRange);

        if (distance < unit.attackRange)
        {
            isAttacking = false;
            StartCoroutine(Attack(target));
        }
        else
        {
            MoveToLocation(target.transform.position);
        }
    }

    IEnumerator Attack(GameObject target)
    {
        Debug.Log("attacking " + target.name);
        while (target != null)
        {
            if (target.GetComponentInParent<Units>() != null)
            {
                target.GetComponentInParent<Units>().TakeDamage(unit.damage);
            }
            else if (target.GetComponent<Structures>() != null)
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
    }

    void TakeDamage(int damage)
    {
        //Take damage from some external source.
        health -= damage;
        healthBarText.text = "Health: " + health + "/" + unit.maxHealth;
        if (health <= 0)
        {
            SlayUnit();
        }
    }

    void SlayUnit()
    {
        //Unit dies, probably destroy it and do some stuff on death.
        Debug.Log("dead");
        Destroy(gameObject);
    }
}

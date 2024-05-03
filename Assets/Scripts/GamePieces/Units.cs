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

    public enum Team
    {
        AlliedTeam,
        EnemyTeam,
        Hostile,
        Neutral
    }

    [SerializeField]private Team team;

    //The nav mesh agent.
    NavMeshAgent m_Agent;

    public LayerMask onlyTargetGamePieces;
    
    public TextMeshProUGUI healthBarText;
    public Canvas healthBar;

    public Material EnemyColor;

    //overhead text
    //public TextMeshProUGUI selectedText; //Unit text needs to look at camera otherwse its weird.
    // Start is called before the first frame update
    void Start()
    {
        if(unit != null)
        {
            health = unit.maxHealth;
            healthBarText.text = "Health: " + health + "/" + unit.maxHealth;
        }
        
        m_Agent= GetComponent<NavMeshAgent>();
        outline = GetComponent<Outline>();
        PlayerActions.MoveSelectedUnits += MoveToLocation;
        PlayerActions.TryAttackObject += AttackTarget;

    }

    private void OnDestroy()
    {
        PlayerActions.MoveSelectedUnits -= MoveToLocation;
        PlayerActions.TryAttackObject -= AttackTarget;
    }

    public void InitializeData()
    {
        health = unit.maxHealth;
        healthBarText.text = "Health: " + health + "/" + unit.maxHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        //I think these are gonna wanna be coroutines, for now I'm just getting the code down.
        if (movingToAttack)
        {
            if (targetedObject != null)
            {
                AttackTarget(targetedObject, false);
            }
            else
            {
                movingToAttack= false;
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

        else if(targetedObject == null)
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
                if ((collider.gameObject.GetComponentInParent<Units>() && collider.gameObject.GetComponentInParent<Units>().unit.GetTeamString() == unit.GetTeamString())
                    || (collider.gameObject.GetComponent<Structures>() && collider.gameObject.GetComponent<Structures>().structure.GetTeamString() == unit.GetTeamString()))
                {
                    //Do nothing
                }
                else
                {
                    //Debug.Log(collider.gameObject.GetComponentInParent<Units>().unit.GetTeamString() + " And " + unit.GetTeamString());
                    Debug.Log("Attacking " + collider.gameObject);
                    targetedObject = collider.gameObject;
                    movingToAttack = true;
                }
            }
        }
    }

    void MoveToLocation(Vector3 location)
    {
        if (!isSelected && !movingToAttack)
        {
            Debug.Log("Unit cannot move! " + isSelected + " " + movingToAttack);
            return;
        }
        //Sets nav agent destination to the targeted location.
        m_Agent.destination = location;
        //StopAllCoroutines(); //maybe
    }

    void AttackTarget(GameObject target, bool commanded)
    {
        if(commanded)
        {
            if (!isSelected)
            {
                return;
            }
            movingToAttack = true;
        }
        targetedObject = target;
        //Debug.Log("omw to " + target.name);
        //While the unit is too far from the target, move towards the target. Otherwise, attack the target.
        float distance = Vector3.Distance(gameObject.transform.position, target.transform.position);
        //Debug.Log(distance + " and " + unit.attackRange);

        if (distance < unit.attackRange && movingToAttack)
        {
            m_Agent.isStopped = true;
            StartCoroutine(Attack(target));
            Debug.Log("attack started");
            movingToAttack=false;
            isAttacking = true;
        }
        else
        {
            MoveToLocation(target.transform.position);
        }
    }

    IEnumerator Attack(GameObject target)
    {
        Debug.Log("attacking " + target.name);
        int targetHealth;
        while (target != null)
        {
            if (target.GetComponentInParent<Units>() != null)
            {
                targetHealth = target.GetComponentInParent<Units>().TakeDamage(unit.damage, gameObject);
                if(targetHealth <=0)
                {
                    isAttacking = false;
                    targetedObject = null;
                    m_Agent.isStopped = false;
                    Debug.Log("attack finished");
                    break;
                }
            }
            else if (target.GetComponent<Structures>() != null)
            {
                target.GetComponent<Structures>().TakeDamage(unit.damage);

                //I don't think this needs to be here anymore.
                if (target.GetComponent<Structures>() == null)
                {
                    isAttacking = false;
                    targetedObject = null;
                }
            }
            else if (target.GetComponentInParent<ResourceEntities>() != null)
            {
                target.GetComponent<ResourceEntities>().TakeDamage(unit.damage);
            }
            yield return new WaitForSeconds(unit.timeBetweenAttacks);
        }
        isAttacking = false;
        targetedObject = null;
        m_Agent.isStopped = false;
        Debug.Log("attack finished");
    }

    public void SetTeam(string teamToSet)
    {
        switch (teamToSet)
        {
            case "AlliedTeam":
                team = Team.AlliedTeam;
                break;
            case "EnemyTeam":
                team = Team.EnemyTeam;
                gameObject.GetComponentInChildren<MeshRenderer>().material = EnemyColor;
                break;
            case "Hostile":
                team = Team.Hostile;
                break;
            case "Neutral":
                team = Team.Neutral;
                break;
        }
    }

    public string GetTeamString()
    {
        return team.ToString();
    }

    int TakeDamage(int damage, GameObject source)
    {
        //Take damage from some external source.
        health -= damage;

        healthBarText.text = "Health: " + health + "/" + unit.maxHealth;

        //Debug.Log($"{unit.name} @{health}/{unit.maxHealth}");

        if (health <= 0)
        {
            SlayUnit();
        }
        if(!isAttacking)
        {
            movingToAttack=true;
            targetedObject = source;
        }

        return health;
    }

    void SlayUnit()
    {
        //Unit dies, probably destroy it and do some stuff on death.
        Debug.Log("dead");
        Destroy(gameObject);
    }
}

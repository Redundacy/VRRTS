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
    public Animator modelAnimator;

    //This SUCKS but it hopefully works
    [SerializeField] GameObject marchingPoint0;
    [SerializeField] GameObject marchingPoint1;
    [SerializeField] GameObject marchingPoint2;
    [SerializeField] GameObject marchingPoint3;
    [SerializeField] GameObject marchingPoint4;
    [SerializeField] GameObject marchingPoint5;
    [SerializeField] GameObject marchingPoint6;
    [SerializeField] GameObject playerCommandTower;

    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject deathEffect;

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

        marchingPoint0 = GameObject.Find("MarchingPoint");
        marchingPoint1 = GameObject.Find("MarchingPoint (1)");
        marchingPoint2 = GameObject.Find("MarchingPoint (2)");
        marchingPoint3 = GameObject.Find("MarchingPoint (3)");
        marchingPoint4 = GameObject.Find("MarchingPoint (4)");
        marchingPoint5 = GameObject.Find("MarchingPoint (5)");
        marchingPoint6 = GameObject.Find("MarchingPoint (6)");
        playerCommandTower = GameObject.Find("AllyCommandTower");

        float xOffset = Random.Range(-3f, 3f);
        float zOffset = Random.Range(-3f, 3f);

        Vector3 newLocation = gameObject.transform.position;
        newLocation.x = newLocation.x + xOffset;
        newLocation.z = newLocation.z + zOffset;

        MarchToLocation(newLocation);
    }

    private void OnDestroy()
    {
        PlayerActions.MoveSelectedUnits -= MoveToLocation;
        PlayerActions.TryAttackObject -= AttackTarget;

        if (GetTeamString() == "EnemyTeam")
        {
            OpponentBrain.MarchOnPlayer -= MarchOnThePlayer;
        }
    }

    public void InitializeData()
    {
        health = unit.maxHealth;
        healthBarText.text = "Health: " + health + "/" + unit.maxHealth;
        Instantiate(unit.hat, transform.Find("Unit Model").Find("upperBody").Find("head").Find("hatParent"));
        Instantiate(unit.model, transform.Find("Unit Model").Find("upperBody").Find("shoulderRight").Find("armRight"));
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
                modelAnimator.SetBool("characterWalk", false);
                isMoving = false;
            }
        }

        else if(targetedObject == null)
        {
            CheckForEnemies();
            modelAnimator.SetBool("CharacterAttack", false);
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
    public void SelectOn()
    {
        isSelected = true;
        outline.enabled = true;
        Debug.Log(isSelected + "\n" + outline.enabled);
    }
    public void SelectOff()
    {
        isSelected = false;
        outline.enabled = false;
        Debug.Log(isSelected + "\n" + outline.enabled);
    }

    void CheckForEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, unit.visionRange, onlyTargetGamePieces);

        if (hitColliders.Length != 0)
        {
            foreach (Collider collider in hitColliders)
            {
                if ((collider.gameObject.GetComponentInParent<Units>() && collider.gameObject.GetComponentInParent<Units>().GetTeamString() == GetTeamString())
                    || (collider.gameObject.GetComponentInParent<Structures>() && collider.gameObject.GetComponentInParent<Structures>().GetTeamString() == GetTeamString()))
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
        modelAnimator.SetBool("characterWalk", true);
        //StopAllCoroutines(); //maybe
    }

    void MarchToLocation(Vector3 location)
    {
        //Sets nav agent destination to the targeted location.
        m_Agent.destination = location;
        modelAnimator.SetBool("characterWalk", true);
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

        if (distance < unit.attackRange && movingToAttack && !isAttacking)
        {
            m_Agent.isStopped = true;
            StartCoroutine(Attack(target));
            Debug.Log("attack started");
            movingToAttack=false;
            modelAnimator.SetBool("characterWalk", false);
            modelAnimator.SetBool("CharacterAttack", true);
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
            else if (target.GetComponentInParent<Structures>() != null)
            {
                target.GetComponentInParent<Structures>().TakeDamage(unit.damage);

                //I don't think this needs to be here anymore.
                if (target.GetComponentInParent<Structures>() == null)
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
        modelAnimator.SetBool("CharacterAttack", false);
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
                foreach (MeshRenderer bodyPart in transform.Find("Unit Model").GetComponentsInChildren<MeshRenderer>())
                {
                    bodyPart.material = EnemyColor;
                }
                OpponentBrain.MarchOnPlayer += MarchOnThePlayer;
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

    void MarchOnThePlayer()
    {
        int randomOdds = Random.Range(0, 1);
        if (randomOdds == 0)
        {
            StartCoroutine(March());
        }
    }

    IEnumerator March()
    {
        MarchToLocation(marchingPoint0.transform.position);
        yield return new WaitForSeconds(5f);
        MarchToLocation(marchingPoint1.transform.position);
        yield return new WaitForSeconds(5f);
        MarchToLocation(marchingPoint2.transform.position);
        yield return new WaitForSeconds(5f);
        MarchToLocation(marchingPoint3.transform.position);
        yield return new WaitForSeconds(5f);
        MarchToLocation(marchingPoint4.transform.position);
        yield return new WaitForSeconds(5f);
        MarchToLocation(marchingPoint5.transform.position);
        yield return new WaitForSeconds(5f);
        MarchToLocation(marchingPoint6.transform.position);
        yield return new WaitForSeconds(5f);
        movingToAttack = true;
        AttackTarget(playerCommandTower, false);
    }

    int TakeDamage(int damage, GameObject source)
    {
        //Take damage from some external source.
        health -= damage;

        healthBarText.text = "Health: " + health + "/" + unit.maxHealth;

        //Debug.Log($"{unit.name} @{health}/{unit.maxHealth}");

        Vector3 hitParticlePosition = gameObject.transform.position;
        hitParticlePosition.y = hitParticlePosition.y + 0.25f;
        Instantiate(hitEffect, hitParticlePosition, Quaternion.identity);

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
        StartCoroutine(DeathAnimation());
    }

    public IEnumerator DeathAnimation()
    {
        modelAnimator.SetBool("characterDIE", true);
        Vector3 deathParticlePosition = gameObject.transform.position;
        deathParticlePosition.y = deathParticlePosition.y + 0.5f;
        Instantiate(deathEffect, deathParticlePosition, Quaternion.identity);
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}

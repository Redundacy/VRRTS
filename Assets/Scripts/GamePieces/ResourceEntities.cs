using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceEntities : GamePieces
{
    public ResourceEntityData resourceEntity;

    //General structure stats
    float health;

    public TextMeshProUGUI healthBarText;
    public Canvas healthBar;
    [SerializeField] float healthBarRenderDistance;

    [SerializeField] AudioClip takeDamageSound;
    [SerializeField] float volume;
    [SerializeField] GameObject hitEffect;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        health = resourceEntity.maxHealth;
        healthBarText.text = "Health: " + health + "/" + resourceEntity.maxHealth;

        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(gameObject.transform.position, GameObject.Find("VRRTS Player").transform.position);
        if (distanceToPlayer < healthBarRenderDistance)
        {
            healthBar.enabled = true;
            Vector3 lookDirection = healthBar.transform.position - GameObject.Find("VRRTS Player").transform.position;
            healthBar.transform.rotation = Quaternion.LookRotation(lookDirection);
        } else
        {
            healthBar.enabled = false;
        }
    }

    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        Debug.Log("Damage taken! Health: " + health);
        healthBarText.text = "Health: " + health + "/" + resourceEntity.maxHealth; //Kinda not a fan of just having this line here, but making a method feels silly. Idk.

        Vector3 hitParticlePosition = gameObject.transform.position;
        hitParticlePosition.y = hitParticlePosition.y + 0.25f;
        Instantiate(hitEffect, hitParticlePosition, Quaternion.identity);

        audioSource.PlayOneShot(takeDamageSound, volume);
        
        if (health <= 0)
        {
            DestroyEntity();
        }
    }

    public void DestroyEntity()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().GivePlayerResources(resourceEntity.numResourcesGranted);

        //Destroy self when all is done.
        Debug.Log("Structure destroyed! Hopefully I don't exist anymore.");
        Destroy(gameObject);
    }
}

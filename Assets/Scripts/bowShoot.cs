using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bowShoot : MonoBehaviour
{
    public Transform projSpawnPoint;
    public GameObject projPrefab;
    public float projSpeed = 10;

    Animator animator;
    int bowFireHash;

    Coroutine bowFiringCo;

    void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Log(animator);
        bowFireHash = Animator.StringToHash("bowFire");

        bowFiringCo = StartCoroutine(bowProjFire());
    }

    IEnumerator bowProjFire()
    {
        yield return new WaitForSeconds(1.5f);
        var projectile = Instantiate(projPrefab, projSpawnPoint.position, projSpawnPoint.rotation);
        projectile.GetComponent<Rigidbody>().velocity = projSpawnPoint.forward * projSpeed;
        
    }

    void Update()
    {
        bool bowFire = animator.GetBool(bowFireHash);
        int i = 0;
        if (Input.GetKey("q") && i < 1)
        {
            animator.SetBool(bowFireHash, true);
            i++;
            StartCoroutine(bowProjFire());
            
        }

    


    }
    
}

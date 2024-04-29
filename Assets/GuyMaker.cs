using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuyMaker : MonoBehaviour
{
    private ShowcaseUnits showcaseUnit;
    private ShowcaseStructures showcaseStructure;

    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<ShowcaseUnits>() != null)
            showcaseUnit = GetComponent<ShowcaseUnits>();
        if(GetComponent<ShowcaseStructures>() != null)
            showcaseStructure = GetComponent<ShowcaseStructures>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("hit " + collision.collider.name);
        if(collision.collider.gameObject.layer == 3)
        {
            if(showcaseUnit != null)
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().RequestMakeGuy("AlliedTeam", showcaseUnit.unit, collision.contacts[0].point);
            } else if (showcaseStructure != null)
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().RequestMakeStructure("AlliedTeam", showcaseStructure.structure, collision.contacts[0].point);
            }
            Destroy(gameObject);
        }
    }
}

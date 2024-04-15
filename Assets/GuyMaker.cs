using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuyMaker : MonoBehaviour
{
    private ShowcaseUnits showcaseUnit;
    // Start is called before the first frame update
    void Start()
    {
        showcaseUnit = GetComponent<ShowcaseUnits>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit " + collision.collider.name);
        if(collision.collider.gameObject.layer == 3)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().RequestMakeGuy("AlliedUnit", showcaseUnit.unit, collision.contacts[0].point);
            Destroy(gameObject);
        }
    }
}

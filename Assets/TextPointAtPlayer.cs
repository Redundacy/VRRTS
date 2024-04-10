using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPointAtPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDirection = gameObject.transform.position - GameObject.Find("VRRTS Player").transform.position;
        gameObject.transform.rotation = Quaternion.LookRotation(lookDirection);
    }
}

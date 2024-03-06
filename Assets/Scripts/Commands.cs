using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commands : MonoBehaviour, CommandsInterface
{

    public Material selectedMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectUnit()
    {
        gameObject.GetComponent<MeshRenderer>().material = selectedMaterial;
    }
}

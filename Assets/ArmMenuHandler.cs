using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmMenuHandler : MonoBehaviour
{
    public GameObject currentHandPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localEulerAngles.z >=50 && transform.localEulerAngles.z <= 130)
        {
            currentHandPanel.SetActive(true);
            //Debug.Log("shown panel");
        } else
        {
            currentHandPanel.SetActive(false);
        }
        //Debug.Log(transform.localEulerAngles.z);
    }
}

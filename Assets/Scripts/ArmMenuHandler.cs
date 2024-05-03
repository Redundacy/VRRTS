using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ArmMenuHandler : MonoBehaviour
{
    public GameObject currentHandPanel;
    public SteamVR_Action_Boolean isHUD = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ShowHUD");
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isHUD.state)
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

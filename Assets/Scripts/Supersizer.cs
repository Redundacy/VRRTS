using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Valve.VR;
using System;
using TMPro;

public class Supersizer : MonoBehaviour
{
    public List<SteamVR_Input_Sources> sources;
    public SteamVR_Action_Boolean supersizeMe = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("SupersizeMode");
    public int scaleSize;
    private bool supersized;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (SteamVR_Input_Sources source in sources)
        {
            if(supersizeMe.GetStateUp(source) && !supersized)
            {
                SteamVR_Fade.View(Color.black, 0.5f);
                supersized = true;
                Invoke("Resize", 0.5f);
            } else if(supersizeMe.GetStateUp(source) && supersized)
            {
                SteamVR_Fade.View(Color.black, 0.5f);
                supersized = false;
                Invoke("Resize", 0.5f);
            }
        }
    }

    void Resize()
    {
        SteamVR_Fade.View(Color.clear, 0.5f);
        if(supersized)
        {
            transform.localScale = Vector3.one * scaleSize;
        } else
        {
            transform.localScale = Vector3.one;
        }
        
    }
}

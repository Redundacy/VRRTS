using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ArmWheelHandler : MonoBehaviour
{
    [SerializeField]private List<UnitData> buyableUnits = new List<UnitData>();
    [SerializeField]private List<StructureData> buildableStructures = new List<StructureData>();

    [SerializeField]private List<GameObject> shopPodiums = new List<GameObject>();

    [SerializeField]private List<GameObject> iconPrefabs = new List<GameObject>();


    public SteamVR_Action_Boolean isRotate = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("RotateWheel");
    public SteamVR_Action_Boolean isChangeWheel = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ChangeWheelType");
    public List<SteamVR_Input_Sources> sources;

    private List<UnitData> currentPodiumUnits = new List<UnitData>();
    private bool isUnits = true;

    // Start is called before the first frame update
    void Start()
    {
        currentPodiumUnits = buyableUnits.GetRange(0, 3);
        foreach(GameObject showcaseUnit in shopPodiums)
        {
            VisualizePodium(shopPodiums.IndexOf(showcaseUnit));
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isRotate.GetStateDown(sources[0]))
        {
            Debug.Log("rotating left");
            RotateLeft();
        } else if (isRotate.GetStateDown(sources[1]))
        {
            Debug.Log("rotating Right");
            RotateRight();
        }

        if(isChangeWheel.stateDown)
        {
            isUnits = !isUnits;
            foreach (GameObject showcaseUnit in shopPodiums)
            {
                VisualizePodium(shopPodiums.IndexOf(showcaseUnit));
            }
        }
    }

    void RotateLeft()
    {
        if(isUnits)
        {
            int catalogIndex = buyableUnits.IndexOf(currentPodiumUnits[0]);
            for(int i = 2; i >0; i--)
            {
                currentPodiumUnits[i] = currentPodiumUnits[i - 1];
            }
            if (catalogIndex == 0)
            {
                currentPodiumUnits[0] = buyableUnits[buyableUnits.Count - 1];
            } else
            {
                currentPodiumUnits[0] = buyableUnits[catalogIndex - 1];
            }
            foreach (GameObject showcaseUnit in shopPodiums)
            {
                VisualizePodium(shopPodiums.IndexOf(showcaseUnit));
                showcaseUnit.GetComponentInChildren<ShowcaseUnits>().unit = currentPodiumUnits[shopPodiums.IndexOf(showcaseUnit)];
            }
        }
    }

    void RotateRight()
    {
        int catalogIndex = buyableUnits.IndexOf(currentPodiumUnits[2]);
        for (int i = 0; i < 2; i++)
        {
            currentPodiumUnits[i] = currentPodiumUnits[i + 1];
        }
        if (catalogIndex == currentPodiumUnits.Count-1)
        {
            currentPodiumUnits[2] = buyableUnits[0];
        }
        else
        {
            currentPodiumUnits[2] = buyableUnits[catalogIndex + 1];
        }
        foreach (GameObject showcaseUnit in shopPodiums)
        {
            VisualizePodium(shopPodiums.IndexOf(showcaseUnit));
            showcaseUnit.GetComponentInChildren<ShowcaseUnits>().unit = currentPodiumUnits[shopPodiums.IndexOf(showcaseUnit)];
        }
    }


    public void VisualizePodium(int podiumIndex)
    {
        Transform cylinderRef = shopPodiums[podiumIndex].transform;
        if(cylinderRef.Find("Showcase Unit(Clone)") != null)
        {
            Destroy(cylinderRef.Find("Showcase Unit(Clone)").gameObject);
        } else if (cylinderRef.Find("Showcase Structure(Clone)") != null)
        {
            Destroy(cylinderRef.Find("Showcase Structure(Clone)").gameObject);
        }

        if(isUnits)
        {
            GameObject icon = Instantiate(iconPrefabs[0],cylinderRef);
            icon.transform.SetLocalPositionAndRotation(new Vector3(0, 7, 0), new Quaternion(0, 0.707106829f, 0, 0.707106829f));
            icon.transform.localScale = new Vector3(0.5f,5,0.5f);

            icon.GetComponent<ShowcaseUnits>().unit = currentPodiumUnits[podiumIndex];
        } else
        {
            GameObject icon = Instantiate(iconPrefabs[1], cylinderRef);
            icon.transform.SetLocalPositionAndRotation(new Vector3(0, 7, 0), new Quaternion(0, 0.707106829f, 0, 0.707106829f));
            icon.transform.localScale = new Vector3(3f, 20f, 3f);

        }
        
    }
}

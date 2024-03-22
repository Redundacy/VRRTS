using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmWheelHandler : MonoBehaviour
{
    [SerializeField]private List<UnitData> buyableUnits = new List<UnitData>();
    [SerializeField]private List<GameObject> shopPodiums = new List<GameObject>();

    private List<UnitData> currentPodiumUnits = new List<UnitData>();

    // Start is called before the first frame update
    void Start()
    {
        currentPodiumUnits = buyableUnits.GetRange(0, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LeftArrowPressed()
    {

    }

    public void RightArrowPressed()
    {

    }

    public void VisualizePodium(int podiumIndex)
    {
        UnitState showcaseUnit = shopPodiums[podiumIndex].GetComponentInChildren<UnitState>();
        //set unit
        //update that unit
    }
}

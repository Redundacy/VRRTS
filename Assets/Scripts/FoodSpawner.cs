using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class FoodSpawner : MonoBehaviour
{
    //Much of the code here is derived from Garret's work. Thank you, Garret.
    public GameObject food;
    private Interactable interactable;
	private ShowcaseUnits showcaseUnits;

    private void Awake()
    {
        interactable = this.GetComponent<Interactable>();
		showcaseUnits = this.GetComponent<ShowcaseUnits>();
    }

	private void OnHandHoverBegin(Hand hand)
    {

    }

	private void OnHandHoverEnd(Hand hand)
    {

    }

	private void HandHoverUpdate(Hand hand)
	{
		GrabTypes startingGrabType = hand.GetGrabStarting();
		bool isGrabEnding = hand.IsGrabEnding(this.gameObject);
		if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
		{
			// Instantiate the new object and position it at the hand's attachment point
			GameObject spawnedFood = Instantiate(food, hand.transform.position, hand.transform.rotation) as GameObject;
			Debug.Log("Food spawned!");
			if(spawnedFood.GetComponent<ShowcaseUnits>() != null)
            {
				spawnedFood.GetComponent<ShowcaseUnits>().unit = showcaseUnits.unit;
			} else if(spawnedFood.GetComponent<ShowcaseStructures>() != null)
            {
				spawnedFood.GetComponent<ShowcaseStructures>().structure = GetComponent<ShowcaseStructures>().structure;
            }
			
			// Attach the spawned object to the hand
			hand.AttachObject(spawnedFood, startingGrabType);
		}
		else if (isGrabEnding)
		{

		}
	}
}

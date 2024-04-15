using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class GameStartButton : MonoBehaviour
{
    //Much of the code here is derived from Garret's work. Thank you, Garret.
    private Interactable interactable;
    GameObject player;

    private void Awake()
    {
        interactable = this.GetComponent<Interactable>();
    }

    private void Start()
    {
        player = GameObject.Find("VRRTS Player");
    }

    private void OnHandHoverBegin(Hand hand)
    {

    }

    private void OnHandHoverEnd(Hand hand)
    {

    }

    //When the button is pressed, it moves the player to their start position and calls the start game function in game manager.
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabEnding(this.gameObject);
        if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            //TODO: Fade the screen before doing this
            player.transform.position = GameObject.Find("PlayerSpawnPoint").transform.position;
            //TODO: Add a start game function to the game manager.
            //GameObject.Find("GameManager").GetComponent<GameManager>().StartTheGame();
            //TODO: Unfade once all is done
        }
        else if (isGrabEnding)
        {

        }
    }
}

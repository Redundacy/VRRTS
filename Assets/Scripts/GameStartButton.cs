using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
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
            //TEST: Fade the screen before doing this
            ScreenFadeOutAndIn();
            //TEST: Add a start game function to the game manager.
            Debug.Log("Starting the game");
            GameObject.Find("GameManager").GetComponent<GameManager>().StartTheGame();
            //TEST: Unfade once all is done
        }
        else if (isGrabEnding)
        {

        }
    }

    void ScreenFadeOutAndIn()
    {
        SteamVR_Fade.View(Color.black, 0.5f);
        Invoke("ScreenFadeIn", 0.5f);
    }

    void ScreenFadeIn()
    {
        player.transform.position = GameObject.Find("PlayerSpawnLocation").transform.position;
        SteamVR_Fade.View(Color.clear, 0.5f);
    }
}

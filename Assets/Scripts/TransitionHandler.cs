using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionHandler : MonoBehaviour
{

    private Animator transition;
    private Transform ourTransition;
    [HideInInspector] public float transitionTime = 1f;
    public enum SelectedTransition { Crossfade, Bars};
    public SelectedTransition selectedTransition;
    public enum SelectedScene {HomeScreen, Lobby, PlayerHub};
    public SelectedScene selectedScene;
    private GameObject level;
    private ConnectToServer connectToServer;
    private RoomHandler roomHandler;


    void Awake()
    {
        ourTransition = transform.Find(selectedTransition.ToString());
        ourTransition.gameObject.SetActive(true);
    }

    public void Transition(){
        string scene = selectedScene.ToString();
        switch (scene){
            case "HomeScreen":
                level = GameObject.Find("ServerConnector");
                connectToServer = level.GetComponent<ConnectToServer>();
                if (connectToServer.connectedToServer) StartCoroutine(LoadLevel(scene));
                break;
            default:
                break;
        }
    }


    public void ExternalTransition(){
        /* 
        * Used for when other scripts interact with this script rather than vice versa.
        * This is useful in particular with the Lobby as the RoomHandler script handles
        * errors and such. 
        */
        transition = ourTransition.GetComponent<Animator>();
        transition.SetTrigger("Start");
    }

    IEnumerator LoadLevel(string scene){
        transition = ourTransition.GetComponent<Animator>();
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        switch(scene){
            case "HomeScreen":
                connectToServer.Play();
                break;
            default:
                break;
        }
    }

}

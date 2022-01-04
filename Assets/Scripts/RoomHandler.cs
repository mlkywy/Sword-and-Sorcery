using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RoomHandler : MonoBehaviourPunCallbacks, ILobbyCallbacks, IConnectionCallbacks
{
    #region Primitive Variables
    private string playerName;
    public int playerChoice = 0;
    public bool hasText = false;
    #endregion

    #region GameObjects
    public GameObject createRoomInput;
    public GameObject joinRoomInput;
    public GameObject playerNameInput;
    public GameObject errorScreen;
    public TMP_Text errorMessage;
    public TMP_Text createRoomName;
    #endregion

    #region Scripts
    public TransitionHandler transitionHandler;
    private RoomOptions options;
    #endregion

    void Awake()
    {
        // Create our own room options to apply when users create their own rooms.
        options = new RoomOptions();
        options.MaxPlayers = 2;
    }

    void Update(){
        /* 
        * "createText" refers to the input field a user fills out to create room with a room name.
        * Update loops every frame, so it is important to constantly check that there is text in
        * that input field, otherwise we do not allow a user to create a room. 
        */
        string createText = createRoomName.text.ToLower();
        if (createText.Length > 1) hasText = true;
        else hasText = false;

        /* 
        * We use this same logic to get the player's name. 
        */
        if (playerNameInput.GetComponent<TMP_InputField>().text.Length == 0) playerName = null;
        else playerName = playerNameInput.GetComponent<TMP_InputField>().text.ToLower();
    }

    public void CreateRoom()
    {
        // First we create a scope-locked variable named createText, using the createRoomInput field.
        string createText = createRoomInput.GetComponent<TMP_InputField>().text.ToLower();
        Debug.Log(createText);
        // Now, prior to creating the room we do some basic error handling; passing a string for the error to display.
        if (playerName == null) DisplayOrHideError("Please set a name before creating/joining a room!");
        
        // Finally, one more check – we need to make sure that the room being created has a name.
        else if (hasText) {
            // If it does, we initialise a transition.
            StartCoroutine(Transition("Create", createText));  
        }
        // Otherwise, we display an error.
        else DisplayOrHideError("Please set a room name before creating a room");
    }

    public void JoinRoom()
    {
        // More error handling, follows the same as above.
        if (playerName == null) DisplayOrHideError("Please set a name before creating/joining a room!");
        else {
            /* 
            * In this case, we do not need to be as stringent with the name-checking when a user joins a lobby
            * This is because we are using PunCallbacks, and we can use the OnJoinRoomFailed() function.
            */
            string joinText = joinRoomInput.GetComponent<TMP_InputField>().text.ToLower();
            Debug.Log(joinText);
            if (joinText.Length > 0) StartCoroutine(Transition("Join", joinText));
            else DisplayOrHideError("Room does not exist!");
        }
    }

    public override void OnJoinedRoom()
    {
        // Set the Nickname, then load the level.
        PhotonNetwork.NickName = playerName;
        PhotonNetwork.LoadLevel("PlayerHub");
    }

    public override void OnJoinRoomFailed(short returnCode, string message){
        // Hide the transition
        transitionHandler.gameObject.SetActive(false);
        // This will let us display an error message to the user if the requested room cannot be joined.
        DisplayOrHideError(message);
    }

    public void DisplayOrHideError(string message){
        /* 
        * This is our own method to display GUI error messages. It is very basic.
        * Essentially, it just activates an ErrorScreen GameObject and has functionality
        * for a button to hide the error message, so that the user may try again. 
        */ 
        errorMessage.text = message;
        if (errorScreen.activeInHierarchy) errorScreen.SetActive(false);
        else errorScreen.SetActive(true);
    }

    IEnumerator Transition(string function, string args){
        // A basic Coroutine for nice GUI transitions. Takes in parameters so that it can be utilised by all methods.
        transitionHandler.gameObject.SetActive(true);
        transitionHandler.ExternalTransition();
        yield return new WaitForSeconds(transitionHandler.transitionTime);
        if (function == "Create") PhotonNetwork.CreateRoom(args, options);
        else if (function == "Join") PhotonNetwork.JoinRoom(args);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CharacterSelector : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    #region Character Selection Variables
    public GameObject[] playerOneOptions;
    public GameObject[] playerTwoOptions;
    public int playerOneChoice = 0;
    public int playerTwoChoice = 0;
    #endregion
    #region TMP Elements
    public TMP_Text playerOneCharName;
    public TMP_Text playerTwoCharName;
    public TMP_Text playerOneCharType;
    public TMP_Text playerTwoCharType;
    public TMP_Text playerOneText;
    public TMP_Text playerTwoText;
    public TMP_Text timerText;
    #endregion
    #region UI Elements
    public Button playerOneReady;
    public Button playerTwoReady;
    public GameObject buttons;
    #endregion
    #region Misc
    private PhotonView view;
    private bool playerOneSet = false;
    private bool playerTwoSet = false;
    private bool P1Ready = false;
    private bool P2Ready = false;
    private bool timerSet = false;
    private int countdown = 5;
    #endregion

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        view = GetComponent<PhotonView>();
    }

    void Update()
    {   
        // Set the player names; synchronised across the server.
        if (!playerOneSet || !playerTwoSet){
            view.RPC("UpdateText", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber - 1);
        }

        // Start a countdown timer to begin the game.
        if (P1Ready && P2Ready && !timerSet){
            timerSet = true;
            StartCoroutine("CountdownTimer", countdown);
        }

    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        if (player.ActorNumber == 2) view.RPC("ShowButtons", RpcTarget.All);
        Debug.Log(player.ActorNumber);
    }

    private IEnumerator CountdownTimer(int time){
        // A very basic Coroutine to just make a 5 second countdown appear.
        while (time > 0){
            view.RPC("CountdownText", RpcTarget.All, time.ToString());
            time--;
            yield return new WaitForSeconds(1);
        }
        // Starts the game after the loop/countdown is finished.
        StartGame();
    }

    void StartGame(){
        // Synchronous level loading.
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("LevelOne");
    }

 
    #region Button Logic
    public void Ready(int playerID){
        // A method for the ready button onClick – makes sure that only the correct corresponding player can click their own button.
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1){
            if (playerID == 1) {
                view.RPC("SetReady", RpcTarget.All, playerID);
            }
        }
        if (PhotonNetwork.LocalPlayer.ActorNumber == 2){
            if (playerID == 2) {
                view.RPC("SetReady", RpcTarget.All, playerID);
            }
        }
    }
    #endregion
    
    #region PunRPCs
    [PunRPC]
    void UpdateText(int playerID){
        // Handles player names in the GUI.
        switch(playerID){
            case 0:
                playerOneSet = true;
                playerOneText.text = $"P1: {PhotonNetwork.PlayerList[playerID].NickName} ";
                break;
            case 1:
                playerTwoSet = true;
                playerTwoText.text = $"P2: {PhotonNetwork.PlayerList[playerID].NickName}";
                break;
        }
    }
    [PunRPC]
    void ShowButtons()
    {
        buttons.SetActive(true);
    }
    [PunRPC]
    void SetReady(int playerNum){
        // Changes the ready buttons to be disabled after a player has clicked one.
        switch(playerNum){
            case 1:
                P1Ready = true;
                playerOneReady.interactable = !playerOneReady.interactable;
                break;
             case 2:
                P2Ready = true;
                playerTwoReady.interactable = !playerTwoReady.interactable;
                break;
        }
    }
    [PunRPC]
    void CountdownText(string time){
        // We use a PunRPC for this so that the countdown before game start is synchronised across the server.
        timerText.text = time;
    }


    [PunRPC]
    void UpdatePlayerName(string playerName, int playerID){
        // Self-explanatory, sets the player names. 
        if (playerID == 1){
            playerOneCharName.text = playerName;
        } else if (playerID == 2){
            playerTwoCharName.text = playerName;
        }
    }
    #endregion
}

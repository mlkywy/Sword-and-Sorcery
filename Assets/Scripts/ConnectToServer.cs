using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks
{

    public TMP_Text connectionStatus;
    [HideInInspector] public bool connectedToServer = false;


    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        connectionStatus.text = "Connected!";
        connectedToServer = true;
    }

    public void Play(){
        if (connectedToServer) SceneManager.LoadScene("Lobby");
    }

}

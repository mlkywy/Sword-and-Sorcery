using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class SpawnPlayers : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject[] characters;   
    [SerializeField] private GameObject debugCamera;
    #endregion

    void Awake()
    {
        debugCamera.SetActive(false);
    }

    void Start()
    {
        // Instantiate the players
        PhotonNetwork.Instantiate(characters[PhotonNetwork.LocalPlayer.ActorNumber - 1].name, new Vector2(Random.Range(-2, 0), 0), Quaternion.identity);
        
        
    }
}

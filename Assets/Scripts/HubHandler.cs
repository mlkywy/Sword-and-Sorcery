using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HubHandler : MonoBehaviour
{
    public GameObject selectorUI;
    
    void Start()
    {
        PhotonNetwork.InstantiateRoomObject(selectorUI.name, new Vector2(0, 0), Quaternion.identity);
    }
}

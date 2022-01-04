using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraSpawner : MonoBehaviour
{
    public GameObject cameraRig;

    void Start()
    {
         PhotonNetwork.InstantiateRoomObject(cameraRig.name, new Vector2(0, 0), Quaternion.identity);
    }

}

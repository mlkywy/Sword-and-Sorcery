using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class LevelHandler : MonoBehaviour, IPunObservable
{
    private PhotonView view;
    private int currentKeys = 0;
    private bool doorUnlocked = false;
    [SerializeField] private GameObject[] keys;
    [SerializeField] private GameObject[] doors;
    private int runTime = 1;
    private string sceneName;
    private DoorHandler doorHandler;


    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        view = gameObject.GetComponent<PhotonView>();
        AddObservable();
    }

    void Update()
    {
        keys = GameObject.FindGameObjectsWithTag("Key");
        doors = GameObject.FindGameObjectsWithTag("Door");
    }

    public void KeyAcquired()
    {
        currentKeys++;
        if (currentKeys >= keys.Length) {
            doorUnlocked = true;
        }

        if (doorUnlocked && runTime > 0) {
            view.RPC("UnlockDoors", RpcTarget.All, runTime);
            runTime--;
        }
    }

    [PunRPC]
    void UnlockDoors(int runTimes) 
    {
        Debug.Log("UnlockDoors() invoked");
        if (runTimes > 0) 
        {
            Debug.Log("if statement verified");
            foreach (GameObject door in doors) {
                doorHandler = door.GetComponent<DoorHandler>();
                doorHandler.doorIsUnlocked = true;
                doorHandler.Unlock();
            }
        }
    }

    public void Load(int buildIndex) {
        sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(buildIndex));
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel(sceneName);
    }

    void AddObservable()
    {
        if (!view.ObservedComponents.Contains(this))
        {
            view.ObservedComponents.Add(this);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) 
        {
            stream.SendNext(doorUnlocked);
            stream.SendNext(currentKeys);
            stream.SendNext(runTime);
        }
        else 
        {
            doorUnlocked = (bool) stream.ReceiveNext();
            currentKeys = (int) stream.ReceiveNext();
            runTime = (int) stream.ReceiveNext();
        }
    }
}

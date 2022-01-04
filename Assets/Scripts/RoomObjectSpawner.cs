using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RoomObjectSpawner : MonoBehaviour, IPunObservable
{
    #region Variables
    public enum SceneHandler {LevelOne, LevelTwo, LevelThree};
    public SceneHandler sceneHandler;
    [SerializeField] private GameObject crate;
    [SerializeField] private GameObject key;
    [SerializeField] private GameObject door;
    private PhotonView view;
    private bool levelOneSpawned = false;
    private bool levelTwoSpawned = false;
    #endregion

    void Awake() 
    {

    }
    
    // Start is called before the first frame update
   void Start() {
       Scene scene = SceneManager.GetActiveScene(); 
       view = gameObject.GetComponent<PhotonView>();
       if (!PhotonNetwork.IsMasterClient) return;
       else {
           switch(scene.name) {
            case "LevelOne":
                // Level one objects...
                if (!levelOneSpawned) LevelOneSetup();
                break;
            case "LevelTwo":
                // Level two objects...
                if (!levelTwoSpawned) LevelTwoSetup();
                break;
            case "LevelThree":
                // Level three objects...
                break;
        }

       }
}


    void LevelOneSetup() {
        if (PhotonNetwork.IsMasterClient && view.IsMine) {
            PhotonNetwork.InstantiateRoomObject(crate.name, new Vector2(5.0f, -1.28f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject(key.name, new Vector2(13.14f, 14.43f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject(door.name, new Vector2(29f, -1.11f), Quaternion.identity);
            levelOneSpawned = true;
        }
    }

    void LevelTwoSetup() {

        if (PhotonNetwork.IsMasterClient && view.IsMine) {
            PhotonNetwork.InstantiateRoomObject(crate.name, new Vector2(23.0f, -1.4f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject(crate.name, new Vector2(-6.16f, 7.71f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject(crate.name, new Vector2(7.28f, 14.56f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject(crate.name, new Vector2(10.7f, 14.56f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject(crate.name, new Vector2(13.9f, 14.56f), Quaternion.identity);

            PhotonNetwork.InstantiateRoomObject(key.name, new Vector2(30.27f, -1.0f), Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject(key.name, new Vector2(-16.5f, 13.56f), Quaternion.identity);

            PhotonNetwork.InstantiateRoomObject(door.name, new Vector2(30f, 6.89f), Quaternion.identity);
            levelTwoSpawned = true;
            view.RPC("MovePlayers", RpcTarget.All);
        }

    }


    [PunRPC]
    void MovePlayers() {
        GameObject knight = GameObject.FindGameObjectWithTag("Knight");
        GameObject mage = GameObject.FindGameObjectWithTag("Mage");

        // Set their spawn
        knight.transform.position = new Vector3(-11.46f, -1.51f, 0);
        mage.transform.position = new Vector3(-13.86f, -1.51f, 0);
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
            if (PhotonNetwork.IsMasterClient) { 
                stream.SendNext(levelOneSpawned);
                stream.SendNext(levelTwoSpawned);
            }
        }
        else 
        {
            levelOneSpawned = (bool) stream.ReceiveNext();
            levelTwoSpawned = (bool) stream.ReceiveNext();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class DoorHandler : MonoBehaviour, IPunObservable
{

    private PhotonView view;
    private LevelHandler levelHandler;
    public bool doorIsUnlocked = false;
    [SerializeField] private GameObject doorClosed;
    [SerializeField] private GameObject doorOpen;
    private bool doorClosedActive;
    private bool doorOpenActive;
    private BoxCollider2D boxCollider2D;
    private bool knight = false;
    private bool mage = false;
    private bool fnCalled = false;


    // Start is called before the first frame update
    void Start()
    {
        view = gameObject.GetComponent<PhotonView>();
        doorClosed = gameObject.transform.GetChild(0).gameObject;
        doorOpen = gameObject.transform.GetChild(1).gameObject;
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        levelHandler = GameObject.FindGameObjectWithTag("Level Handler").GetComponent<LevelHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (doorIsUnlocked) doorClosed.SetActive(false);
    }

    public void Unlock()
    {
        // OpenDoor();
        view.RPC("OpenDoor", RpcTarget.All);
    }

    [PunRPC]
    void OpenDoor()
    {
        doorClosed.SetActive(false);
        boxCollider2D.enabled = true;

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
            stream.SendNext(doorOpenActive);
            stream.SendNext(doorClosedActive);
            stream.SendNext(doorIsUnlocked);
        }
        else 
        {
            doorOpenActive = (bool) stream.ReceiveNext();
            doorClosedActive = (bool) stream.ReceiveNext();
            doorIsUnlocked = (bool) stream.ReceiveNext();
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Knight") knight = true;
        if (col.tag == "Mage") mage = true;
    }

    void OnTriggerStay2D(Collider2D col) {
        if (mage && knight) {
            if (!fnCalled && PhotonNetwork.IsMasterClient) {
                fnCalled = true;
                StartCoroutine(LevelLoad());
            } 
        }
    }

    IEnumerator LevelLoad() {
        levelHandler.Load(SceneManager.GetActiveScene().buildIndex + 1);
        yield return new WaitForSeconds(20);
        fnCalled = false;
    }
}

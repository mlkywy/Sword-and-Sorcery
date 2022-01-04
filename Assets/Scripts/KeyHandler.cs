using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

#if UNITY_EDITOR 
using UnityEditor;
#endif

public class KeyHandler : MonoBehaviour
{
    [SerializeField] private LevelHandler levelHandler;
    private PhotonView view;
    public AudioClip sound;
    private AudioSource source;
    private UnityAction methodDelegate;
    public UnityEvent GetKeyEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }


    void Start()
    {
        source = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
        view = gameObject.GetComponent<PhotonView>();
        levelHandler = GameObject.FindGameObjectWithTag("Level Handler").GetComponent<LevelHandler>();

        if (GetKeyEvent == null)
			GetKeyEvent = new UnityEvent();

        methodDelegate = System.Delegate.CreateDelegate(typeof(UnityAction), source, "Play") as UnityAction;

        #if UNITY_EDITOR
        UnityEditor.Events.UnityEventTools.AddPersistentListener(GetKeyEvent, methodDelegate);
        #endif
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        if (col.tag == "Mage" || col.tag == "Knight") {
            levelHandler.KeyAcquired();
            source.clip = sound;
            GetKeyEvent.Invoke();
            Destroy(gameObject);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

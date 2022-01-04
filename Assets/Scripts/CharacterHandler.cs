using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using TMPro;

public class CharacterHandler : MonoBehaviour, IPunObservable
{
    
    private PhotonView view;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private CharacterController2D characterController2D;
    [SerializeField] private string characterType;
    [SerializeField] private float runSpeed = 10f;
    public GameObject particleHandler;
    public ParticleSystem particles;
    private float horizontalMove = 0;
    private bool jump = false;
    private bool isGrounded;
    public AudioSource walkSFX;
    [SerializeField] private TMP_Text playerName;

    
    

    void Awake()
    {
        particleHandler = this.transform.Find("Particles").gameObject;
        particles = particleHandler.GetComponentInChildren<ParticleSystem>();
        view = this.gameObject.GetComponent<PhotonView>();
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        anim = this.gameObject.GetComponent<Animator>();
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        characterType = this.gameObject.tag;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject); 
        if (characterType == "Mage") characterController2D.m_JumpForce *= 1.5f;
        view.RPC("UpdateName", RpcTarget.All);
        AddObservable();
    }

    
    void Update()
    {
        isGrounded = characterController2D.m_Grounded;
        if (!view.IsMine) return;
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetButtonDown("Jump")) jump = true;
        if (horizontalMove != 0 && isGrounded)
        {
            if (!walkSFX.isPlaying) walkSFX.Play();
            anim.SetBool("isRunning", true);
        }
        else 
        {
            walkSFX.Stop();
            anim.SetBool("isRunning", false);
        }
    }

    void FixedUpdate()
    {
        characterController2D.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }

    void AddObservable()
    {
        if (!view.ObservedComponents.Contains(this))
        {
            view.ObservedComponents.Add(this);
        }
    }

    public void Particles(){
        view.RPC("PlayParticles", RpcTarget.All);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) stream.SendNext(spriteRenderer.flipX);
        else spriteRenderer.flipX = (bool) stream.ReceiveNext();
    }

    [PunRPC]
    void UpdateName()
    {
        if (characterType == "Knight") playerName.text = $"{PhotonNetwork.PlayerList[0].NickName}";
        else if (characterType == "Mage") playerName.text = $"{PhotonNetwork.PlayerList[1].NickName}";
    }

    [PunRPC]
    void PlayParticles()
    {
        particles.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CrateScript : MonoBehaviour, IPunObservable
{

    private Rigidbody2D rb;
    private PhotonView view;
    public bool grounded = false;
    public bool insideBounds = false;
    private bool isFrozenX = false;
    [SerializeField] private LayerMask groundCheckLayer;
    [SerializeField] private BoxCollider2D groundCheckBox;
    [SerializeField] private GameObject hint;
    [SerializeField] private Transform groundCheck;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.freezeRotation = true;
        view = gameObject.GetComponent<PhotonView>();
        AddObservable();
    }

    void Update()
    {
        if (!isGrounded()) 
        {
            
            Debug.Log("I'm falling now." + gameObject.ToString());
            rb.gravityScale = 15f;
            // if (!isFrozenX) StartCoroutine(freezeX());
            rb.freezeRotation = true;
        }

        if (isGrounded()) 
        {
            Debug.Log("I'm touching the ground." + gameObject.ToString());


            if (insideBounds && Input.GetKey(KeyCode.LeftShift)) 
            {
                rb.constraints = RigidbodyConstraints2D.None;
                rb.freezeRotation = true;
                isFrozenX = false;
            }
            
            if (!insideBounds || Input.GetKeyUp(KeyCode.LeftShift)) {
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
                rb.freezeRotation = true;
            }
        }
        
    }

    bool isGrounded() {
        Color rayColor;
        float height = .15f;
        RaycastHit2D hit = Physics2D.Raycast(groundCheckBox.bounds.center, Vector2.down, groundCheckBox.bounds.extents.y + height, groundCheckLayer);
        if (hit.collider != null) {
            rayColor = Color.green;
        } else {
            rayColor = Color.red;
        }
        Debug.DrawRay(groundCheckBox.bounds.center, Vector2.down * (groundCheckBox.bounds.extents.y + height), rayColor);
        grounded = hit.collider != null;
        return hit.collider != null;
    }

    // IEnumerator freezeX() {
    //     isFrozenX = true;
    //     yield return new WaitForSeconds(0.2f);
    //     rb.constraints = RigidbodyConstraints2D.FreezePositionX;
    // }

    void AddObservable()
    {
        if (!view.ObservedComponents.Contains(this))
        {
            view.ObservedComponents.Add(this);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) {
             stream.SendNext(grounded);
             stream.SendNext(rb.bodyType);
             stream.SendNext(rb.constraints);
        }
        else 
        {
            grounded = (bool) stream.ReceiveNext();
            rb.bodyType = (RigidbodyType2D) stream.ReceiveNext();
            rb.constraints = (RigidbodyConstraints2D) stream.ReceiveNext();
        }
    }


    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Knight") {
            hint.SetActive(true);
            insideBounds = true;
        }

        if (col.gameObject.tag == "Crate") {
            insideBounds = true;
        }
    }

    void OnTriggerExit2D(Collider2D col){
        if (col.gameObject.tag == "Knight") 
        {
            hint.SetActive(false);
            insideBounds = false;
        }
    }
}

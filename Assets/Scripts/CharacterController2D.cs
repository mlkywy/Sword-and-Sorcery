using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] public float m_JumpForce = 10f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching
	[SerializeField] private CharacterHandler characterHandler;
	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	public bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private SpriteRenderer m_SpriteRenderer;
	private bool isFalling = false;
	private float fallGravityMultiplier = .05f;
	private bool spaceDown = false;
	private bool isJumping = false;
	private bool canJump = true;
	private PhotonView view;
	private Animator anim;
	[SerializeField] private float jumpTimeCounter;
	private ContactFilter2D contactFilter;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	public UnityEvent OnJumpEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	public BoolEvent OnFlipEvent;
	private bool m_wasCrouching = false;

	

	private void Awake()
	{
		anim = this.gameObject.GetComponent<Animator>();
		view = this.gameObject.GetComponent<PhotonView>();
		m_SpriteRenderer = GetComponent<SpriteRenderer>();
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnJumpEvent == null)
			OnJumpEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();

		if (OnFlipEvent == null)
			OnFlipEvent = new BoolEvent();
	}


	private void Update()
	{
		if (!view.IsMine) return;
		if (isJumping) anim.SetBool("isJumping", true);
		else anim.SetBool("isJumping", false);

		if (isFalling) anim.SetBool("isFalling", true);

		if (m_Grounded) anim.SetBool("isFalling", false);

		if (Input.GetButton("Jump")) spaceDown = true;

		if (Input.GetButtonUp("Jump")) 
		{
			isJumping = false;
			spaceDown = false;
		}
		
		if (m_Grounded && spaceDown && canJump) 
		{
			isJumping = true;
			jumpTimeCounter = 0.35f;
		}
		if (spaceDown && isJumping)
		{
			if (jumpTimeCounter > 0){
				m_Rigidbody2D.velocity = Vector2.up * m_JumpForce;
				jumpTimeCounter -= Time.deltaTime;
			}
			else 
			{
				isJumping = false;
				
			}
		}

		if (Input.GetButtonDown("Jump")) OnJumpEvent.Invoke();

	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
		isFalling = (!isJumping) ? true : false;
		

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}

		// The player can only jump if his head is not blocked by something – we will use a raycast for this
		Ray ray = new Ray (m_CeilingCheck.position, Vector3.up);
		float rayDistance = .1f;
		RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction, rayDistance);

		if (hit2D.collider != null) canJump = false;
		else canJump = true;

		Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);
		
		if (isFalling && !m_Grounded) m_Rigidbody2D.gravityScale += fallGravityMultiplier;
		else m_Rigidbody2D.gravityScale = 1;

	}


	public void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;
		if(m_FacingRight) m_SpriteRenderer.flipX = false;
		else m_SpriteRenderer.flipX = true;
		OnFlipEvent.Invoke(true);
		
	}
}

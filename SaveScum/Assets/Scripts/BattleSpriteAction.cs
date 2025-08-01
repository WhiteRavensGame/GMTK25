using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;

public class BattleSpriteAction : MonoBehaviour
{
    public static event EventHandler OnPlayerDeath;

    static int hashSpeed = Animator.StringToHash ("Speed");
	static int hashFallSpeed = Animator.StringToHash ("FallSpeed");
	static int hashGroundDistance = Animator.StringToHash ("GroundDistance");
	static int hashIsCrouch = Animator.StringToHash ("IsCrouch");
	static int hashAttack1 = Animator.StringToHash ("Attack1");
	static int hashAttack2 = Animator.StringToHash ("Attack2");
	static int hashAttack3 = Animator.StringToHash ("Attack3");


	static int hashDamage = Animator.StringToHash ("Damage");
	static int hashIsDead = Animator.StringToHash ("IsDead");

	[SerializeField] private float characterHeightOffset = 0.2f;
	[SerializeField] LayerMask groundMask;

	[SerializeField, HideInInspector] Animator animator;
	[SerializeField, HideInInspector]SpriteRenderer spriteRenderer;
	[SerializeField, HideInInspector]Rigidbody2D rig2d;

	[SerializeField] public PlayerCharacter characterType;

	public int hp = 4;

	private bool canJump = true;
	private bool isAlive = true;


	void Awake ()
	{
		animator = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		rig2d = GetComponent<Rigidbody2D> ();
	}

    void Start()
    {
		GameManager.Instance.OnTimeUp += TimeUp;
    }

    void Update ()
	{
		if (!isAlive)
			return;
		

		float axis = Input.GetAxisRaw ("Horizontal");
		bool isDown = Input.GetAxisRaw ("Vertical") < 0;

        Vector2 velocity = rig2d.linearVelocity;
        if (Input.GetButtonDown("Jump") && canJump)
        {
            velocity.y = 5;
            canJump = false;
        }
        if (axis != 0)
        {
            spriteRenderer.flipX = axis < 0;
			float moveXVelMultiplier = 2;
            velocity.x = axis * moveXVelMultiplier;
        }
        rig2d.linearVelocity = velocity;

        var distanceFromGround = Physics2D.Raycast (transform.position, Vector3.down, 1, groundMask);

		// update animator parameters
		animator.SetBool (hashIsCrouch, isDown);
		animator.SetFloat (hashGroundDistance, distanceFromGround.distance == 0 ? 99 : distanceFromGround.distance - characterHeightOffset);
		animator.SetFloat (hashFallSpeed, rig2d.linearVelocity.y);
		animator.SetFloat (hashSpeed, Mathf.Abs (axis));
		if( Input.GetKeyDown(KeyCode.Z) ){  animator.SetTrigger(hashAttack1); }
		if( Input.GetKeyDown(KeyCode.X) ){  animator.SetTrigger(hashAttack2); }
		if( Input.GetKeyDown(KeyCode.C) ){  animator.SetTrigger(hashAttack3); }

		//// flip sprite
		//if (axis != 0)
		//	spriteRenderer.flipX = axis < 0;

        //QQQQ hardcoded jump
        if (animator.GetFloat("GroundDistance") < 0.2f && animator.GetFloat("FallSpeed") < 0)
        {
            canJump = true;
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnTimeUp -= TimeUp;
		Debug.Log("Destructor called");
    }

    public PlayerCharacter GetCharacterType()
	{
		return characterType;
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DamageObject"))
		{
			HurtPlayer();
			Console.WriteLine("Damage player");
		}
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("DamageObject"))
        {
            HurtPlayer();
            Console.WriteLine("Damage player (trigger)");
        }
    }

    private void HurtPlayer(int damage = 999)
	{
		hp -= damage;
		if(hp <= 0)
		{
			KillPlayer();
		}

        animator.SetTrigger(hashDamage);
    }

	private void KillPlayer()
	{
		if (!isAlive)
			return;

		hp = 0;
        animator.SetBool(hashIsDead, true);
        isAlive = false;
		OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        animator.SetTrigger(hashDamage);
    }

	private void RevivePlayer()
	{

	}

	private void TimeUp(object sender, System.EventArgs e)
	{
		KillPlayer();
	}

}

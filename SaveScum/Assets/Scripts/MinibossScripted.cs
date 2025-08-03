using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;

public class MinibossScripted : MonoBehaviour
{
    public static event EventHandler OnPlayerDeath;

    static int hashSpeed = Animator.StringToHash("Speed");
    static int hashFallSpeed = Animator.StringToHash("FallSpeed");
    static int hashGroundDistance = Animator.StringToHash("GroundDistance");
    static int hashIsCrouch = Animator.StringToHash("IsCrouch");
    static int hashAttack1 = Animator.StringToHash("Attack1");
    static int hashAttack2 = Animator.StringToHash("Attack2");
    static int hashAttack3 = Animator.StringToHash("Attack3");


    static int hashDamage = Animator.StringToHash("Damage");
    static int hashIsDead = Animator.StringToHash("IsDead");

    [SerializeField] private float characterHeightOffset = 0.2f;
    [SerializeField] LayerMask groundMask;

    [SerializeField, HideInInspector] Animator animator;
    [SerializeField, HideInInspector] SpriteRenderer spriteRenderer;
    [SerializeField, HideInInspector] Rigidbody2D rig2d;

    [SerializeField] public PlayerCharacter characterType;

    public int hp = 4;

    private bool canJump = true;
    private bool isAlive = true;

    private Rigidbody2D rbTarget;
    private BattleSpriteAction playerTarget;

    [SerializeField] public GameObject slamParticle;

    public MinibossState bossState = MinibossState.IDLE;
    private Vector3 startPos;

    [SerializeField] public AudioClip[] hurtAudio;
    [SerializeField] public AudioClip[] deathAudio;
    [SerializeField] public AudioClip[] winAudio;
    [SerializeField] public AudioClip[] attackAudio;

    public enum MinibossState
    {
        IDLE,
        CHALLENGED, //player enter trigger box
        SEE_SAVESTATUE,
        APPROACH_SAVESTATUE,
        HIT_SAVESTATUE,
        POSTHIT_SAVESTATUE,
        RETURN_TO_POST,
        DEAD //fried by laser
    }



    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rig2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        startPos = transform.position;

        Invoke("DelayStatueSearch", 0.5f);
    }

    private void DelayStatueSearch()
    {
        GameObject statue = GameObject.FindGameObjectWithTag("Statue");
        if (statue != null)
        {
            if (statue.TryGetComponent<Rigidbody2D>(out rbTarget))
            {
                //Walk towards the target, and do some actions in order. 
                float xDistance = Mathf.Abs(rbTarget.transform.position.x - transform.position.x);
                float yDistance = Mathf.Abs(rbTarget.transform.position.y - transform.position.y);

                //if the player is higher above the miniboss (y), don't do anything and just do idle (ignore statue)
                Debug.Log(xDistance + "," + yDistance);
                if (yDistance < 0.3f && xDistance < 3.5f)
                {
                    StartCoroutine(ProcessStatueDecoy());
                }


            }
        }
    }

    IEnumerator ProcessStatueDecoy()
    {
        yield return new WaitForSeconds(1f);
        //walk towards 
        bossState = MinibossState.APPROACH_SAVESTATUE;
        yield return new WaitForSeconds(5f);
        animator.SetTrigger(hashAttack2);
        yield return new WaitForSeconds(0.67f);
        bossState = MinibossState.HIT_SAVESTATUE;
        yield return new WaitForSeconds(3f);
        bossState = MinibossState.RETURN_TO_POST;
        yield return new WaitForSeconds(5f);
        yield return null;
    }

    private void SimulateForwardMovement(Vector3 targetPos)
    {
        
        float axis = -1; //move left
        if (targetPos.x > transform.position.x)
            axis = 1; //move right

        bool isDown = false;

        Vector2 velocity = rig2d.linearVelocity;

        if (axis != 0)
        {
            spriteRenderer.flipX = axis < 0;
            float moveXVelMultiplier = 0.75f;
            velocity.x = axis * moveXVelMultiplier;
        }
        else //removes the horizontal movement when left/right released. 
        {
            //velocity.x = 0;
            float frictionFactor = 3;
            velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * frictionFactor);
        }
        rig2d.linearVelocity = velocity;

        float xDistance = Mathf.Abs(targetPos.x - transform.position.x);
        float xDistanceSpacing = 0.5f;
        animator.SetFloat(hashSpeed, Mathf.Abs(axis));

        if (targetPos == startPos) xDistanceSpacing = 0.05f; //go right back to starting point without buffers.
        if (xDistance <= xDistanceSpacing)
        {
            rig2d.linearVelocity = Vector2.zero;

            spriteRenderer.flipX = true; //hardcode left turn.
            axis = 0;
            animator.SetFloat(hashSpeed, Mathf.Abs(axis));
        }

       
    }

    void Update()
    {
       

        if (!isAlive)
            return;

        if(bossState == MinibossState.APPROACH_SAVESTATUE)
        {
            SimulateForwardMovement(rbTarget.transform.position);
        }
        else if(bossState == MinibossState.HIT_SAVESTATUE)
        {
            SendTargetFlyingUp();
            bossState = MinibossState.POSTHIT_SAVESTATUE; //prevents spam hit.
        }
        else if(bossState == MinibossState.RETURN_TO_POST)
        {
            SimulateForwardMovement(startPos);
        }


        float axis = 0;
        bool isDown = false;

        //Vector2 velocity = rig2d.linearVelocity;
        //if (Input.GetButtonDown("Jump") && canJump)
        //{
        //    velocity.y = 5;
        //    canJump = false;
        //}
        //if (axis != 0)
        //{
        //    spriteRenderer.flipX = axis < 0;
        //    float moveXVelMultiplier = 2;
        //    velocity.x = axis * moveXVelMultiplier;
        //}
        //else //removes the horizontal movement when left/right released. 
        //{
        //    //velocity.x = 0;
        //    float frictionFactor = 3;
        //    velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * frictionFactor);
        //}
        //rig2d.linearVelocity = velocity;

        var distanceFromGround = Physics2D.Raycast(transform.position, Vector2.down, 1, groundMask);

        // update animator parameters
        animator.SetBool(hashIsCrouch, isDown);

        //in case the code after this doesn't work, use this instead. 
        float groundDistance = distanceFromGround.collider != null ? distanceFromGround.distance - characterHeightOffset : 99f;
        animator.SetFloat(hashGroundDistance, Mathf.Max(groundDistance, 0f));

        //animator.SetFloat (hashGroundDistance, distanceFromGround.distance == 0 ? 99 : distanceFromGround.distance - characterHeightOffset);

        animator.SetFloat(hashFallSpeed, rig2d.linearVelocity.y);
        //animator.SetFloat(hashSpeed, Mathf.Abs(axis));
        //if (Input.GetKeyDown(KeyCode.Z)) { animator.SetTrigger(hashAttack1); }
        if (Input.GetKeyDown(KeyCode.X)) { animator.SetTrigger(hashAttack2); }
        //if (Input.GetKeyDown(KeyCode.C)) { animator.SetTrigger(hashAttack3); }

        //// flip sprite
        //if (axis != 0)
        //	spriteRenderer.flipX = axis < 0;

    }

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //kill player upon touch
            if (collision.gameObject.TryGetComponent<BattleSpriteAction>(out BattleSpriteAction player))
            {
                player.PlayerTrippedExplosion();

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if(bossState == MinibossState.IDLE)
            {
                Console.WriteLine("Player hit by Miniboss. Go flying.");
                animator.SetTrigger(hashAttack2);
                //Fun physics
                if (collider.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
                {
                    rbTarget = rb;
                    Invoke("SendTargetFlyingUp", 0.5f);

                }
                if (collider.gameObject.TryGetComponent<BattleSpriteAction>(out BattleSpriteAction player))
                {
                    playerTarget = player;

                }
            }

        }
    }
    public void SendTargetFlyingUp()
    {

        AudioManager.Instance.PlaySFX(attackAudio);

        float explosionForce = 10;
        float explosionRadius = 3f;
        float upwardsModifier = 3f;
        //Vector2 explosionDirection = Vector2.up.normalized;
        Vector2 explosionDirection = (rbTarget.transform.position - transform.position).normalized;
        float distance = Vector2.Distance(rbTarget.transform.position, transform.position);
        float forceMultiplier = Mathf.Clamp01(1 - (distance / explosionRadius));

        Vector2 force = explosionDirection * explosionForce * forceMultiplier;
        force.y += upwardsModifier * explosionForce * forceMultiplier;

        rbTarget.AddForce(force, ForceMode2D.Impulse);

        if(playerTarget != null)
            playerTarget.PlayerTrippedExplosion();

        Instantiate(slamParticle, new Vector3(rbTarget.transform.position.x, rbTarget.transform.position.y), Quaternion.identity);


    }

    private void HurtPlayer(int damage = 999)
    {
        hp -= damage;
        if (hp <= 0)
        {
            KillPlayer();
        }
        else
        {
            AudioManager.Instance.PlaySFX(hurtAudio);
        }

        animator.SetTrigger(hashDamage);
    }

    private void KillPlayer()
    {
        if (!isAlive)
            return;

        AudioManager.Instance.PlaySFX(deathAudio);

        hp = 0;
        animator.SetBool(hashIsDead, true);
        isAlive = false;
        OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        animator.SetTrigger(hashDamage);
    }

    public void HitBySecretMelee()
    {
        HurtPlayer();
    }

}

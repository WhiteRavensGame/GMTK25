using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEngine.UI;

public class FinalBossPattern : MonoBehaviour
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
    [SerializeField] private GameObject goal;

    public int hp = 6;
    public int maxHP = 6;

    [SerializeField] private Image healthBarBG;
    [SerializeField] private Image healthBar;

    private float iFrameTimer = 0;
    private float iFrameDuration = 1.5f;

    private bool canJump = true;
    private bool isAlive = true;

    private Rigidbody2D rbTarget;
    private BattleSpriteAction playerTarget;

    [SerializeField] public GameObject slamParticle;

    public BossState bossState = BossState.IDLE;
    private Vector3 startPos;
    float overallSpeed = 1;

    [SerializeField] public AudioClip[] jumpAudio;
    [SerializeField] public AudioClip[] hurtAudio;
    [SerializeField] public AudioClip[] deathAudio;
    [SerializeField] public AudioClip[] winAudio;
    [SerializeField] public AudioClip[] attackAudio;

    [SerializeField] public AudioClip gunfireAudio;

    [SerializeField] private List<GameObject> spawnableProjectiles;
    private List<GameObject> projectiles;


    public enum BossState
    {
        IDLE,
        CHARGING, //telepathing attacks
        FIRING, // fire attacks simultaneously
        RELOADING, 
        PLAYER_DEFEATED,
        DEAD 
    }



    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rig2d = GetComponent<Rigidbody2D>();
        projectiles = new List<GameObject>();

        
    }

    void Start()
    {
        startPos = transform.position;
        
        Invoke("DelayPlayerSearch", 0.75f);
    }

    private void DelayPlayerSearch()
    {
        GameObject g = GameObject.FindGameObjectWithTag("Player");
        playerTarget = g.GetComponent<BattleSpriteAction>();
        Debug.Log(g.name);
        StartCoroutine(ProcessAttackPattern());
        healthBarBG.gameObject.SetActive(true);
        healthBar.gameObject.SetActive(true);
    }

    Bounds GetCameraBounds()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        Vector3 center = cam.transform.position;
        Vector3 size = new Vector3(width, height, 0);

        return new Bounds(center, size);

    }

    private Vector2 GetRandomPositionAroundPlayer(Vector2 playerPosition, float minDistance, float maxDistance)
    {
        Bounds cameraBounds = GetCameraBounds();
        Vector2 spawnPos;
        Vector3 spawnPos3D;
        Vector2 ownerPos = new Vector2(transform.position.x, transform.position.y);
        float distanceToBoss = 999;

        int attempts = 0;
        do
        {
            float angle = UnityEngine.Random.Range(0, 360f);
            float distance = UnityEngine.Random.Range(minDistance, maxDistance);

            float radians = angle * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * distance;
            spawnPos = playerPosition + offset;
            spawnPos3D = new Vector3(spawnPos.x, spawnPos.y, cameraBounds.center.z);
            attempts++;

            distanceToBoss = Vector2.Distance(spawnPos, ownerPos);

            if (attempts >= 50) break;

        } while (!cameraBounds.Contains(spawnPos3D) || distanceToBoss < 1.25f);

        //Debug.Log(attempts);
        

        return spawnPos;
    }

    private void PointProjectileTowardsPlayer(Transform projectileTransform, Vector2 playerPosition)
    {
        Vector2 direction = (playerPosition - (Vector2)projectileTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectileTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    //method called by the animation Attack1
    public void SpawnProjectiles()
    {
        int projectilesToLaunch = 4;
        Vector2 playerPos = new Vector2(playerTarget.transform.position.x, playerTarget.transform.position.y);

        for (int i = 0; i < projectilesToLaunch; i++)
        {
            //Randomize a point around the player, and have projectile point towards player.
            int randProjectile = UnityEngine.Random.Range(0, spawnableProjectiles.Count);

            Vector2 spawnLocation = GetRandomPositionAroundPlayer(playerPos, 2f, 5f);

            GameObject g = Instantiate(spawnableProjectiles[randProjectile], spawnLocation, Quaternion.identity);
            PointProjectileTowardsPlayer(g.transform, playerPos);
        }
    }


    void OnDrawGizmos()
    {
        Bounds bounds = GetCameraBounds();
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    public void FireProjectiles()
    {

    }

    public void DespawnProjectiles()
    {

    }


    IEnumerator ProcessAttackPattern()
    {
        while (playerTarget.IsAlive())
        {
            yield return new WaitForSeconds(3f / overallSpeed);
            bossState = BossState.CHARGING; //show all directions of where the attack can come from.
            animator.speed = overallSpeed;
            animator.SetTrigger(hashAttack1);
            AudioManager.Instance.PlaySFX(attackAudio);
        }

        //Player Died. Do taunt.
        Debug.Log("BOSS TAUNT");
        AudioManager.Instance.PlaySFX(winAudio);
        
       // yield return new WaitForSeconds(1f/overallSpeed);
       //// bossState = BossState.HIT_SAVESTATUE;
       // yield return new WaitForSeconds(3f);
       //// bossState = BossState.RETURN_TO_POST;
       // yield return new WaitForSeconds(5f);
       // yield return null;
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

        //make boss go faster when lower in health
        overallSpeed = 1 + ( 1 - (float)hp / (float)maxHP );
        
        BossProjectiles.speedModifier = overallSpeed;

        //update iFrames if any
        if (iFrameTimer > 0)
            iFrameTimer -= Time.deltaTime;

        //if (bossState == BossState.APPROACH_SAVESTATUE)
        //{
        //    SimulateForwardMovement(rbTarget.transform.position);
        //}
        //else if (bossState == BossState.HIT_SAVESTATUE)
        //{
        //    SendTargetFlyingUp();
        //    bossState = BossState.POSTHIT_SAVESTATUE; //prevents spam hit.
        //}
        //else if (bossState == BossState.RETURN_TO_POST)
        //{
        //    SimulateForwardMovement(startPos);
        //}


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
        //if (Input.GetKeyDown(KeyCode.X)) { animator.SetTrigger(hashAttack1); }
        //if (Input.GetKeyDown(KeyCode.C)) { animator.SetTrigger(hashAttack3); }

        //// flip sprite
        //if (axis != 0)
        //	spriteRenderer.flipX = axis < 0;

    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isAlive)
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
        if (collider.gameObject.CompareTag("Player") && isAlive)
        {
            if (bossState == BossState.IDLE)
            {
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
        else if (collider.gameObject.CompareTag("Projectiles"))
        {
            HurtPlayer(1);
            animator.speed = 1;
            Destroy(collider.gameObject);
        }
        else if(collider.gameObject.CompareTag("MeleeHitBox"))
        {
            Debug.Log("Hit by melee");
            HurtPlayer(2);
            animator.speed = 1;
        }
    }

    public void PlayGunshotAudio()
    {
        AudioManager.Instance.PlaySFX(gunfireAudio);
    }

    public void SendTargetFlyingUp()
    {
        

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

        if (playerTarget != null)
            playerTarget.PlayerTrippedExplosion();

        Instantiate(slamParticle, new Vector3(rbTarget.transform.position.x, rbTarget.transform.position.y), Quaternion.identity);


    }

    private void HurtPlayer(int damage = 999)
    {
        if (iFrameTimer > 0)
            return;

        hp -= damage;
        healthBar.fillAmount = (float)hp / (float)maxHP;

        if (hp <= 0)
        {
            KillPlayer();
        }
        else
        {
            AudioManager.Instance.PlaySFX(hurtAudio);
        }

        animator.SetTrigger(hashDamage);
        iFrameTimer = iFrameDuration;
        
    }

    private void KillPlayer()
    {
        if (!isAlive)
            return;

        AudioManager.Instance.PlaySFX(deathAudio);

        hp = 0;
        healthBarBG.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(false);
        animator.SetBool(hashIsDead, true);
        isAlive = false;
        OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        animator.SetTrigger(hashDamage);
        StopAllCoroutines();
        goal.SetActive(true);
    }

    public void HitBySecretMelee()
    {
        HurtPlayer(2);
    }

}

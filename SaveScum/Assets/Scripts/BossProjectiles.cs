using UnityEngine;

public class BossProjectiles : MonoBehaviour
{
    public static float speedModifier = 1;

    [SerializeField] private float speed = 5f;
    [SerializeField] public bool isScriptedProjectile = false;

    private Rigidbody2D rb;
    private float delay = 0;
    private float lifeSpan = 0;
    private float lifeTime = 5;

    float delayTimer;
    bool isMoving;

    [SerializeField] private GameObject hitParticle;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        delay = UnityEngine.Random.Range(1.8f, 2.2f) / speedModifier;
        delayTimer = delay;
        isMoving = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        delayTimer -= Time.deltaTime;
        if(delayTimer <= 0 && !isMoving)
        {
            rb.linearVelocity = transform.right * speed * speedModifier;
            isMoving = true;
        }

        if(isMoving)
        {
            lifeSpan += Time.deltaTime;
            if (lifeSpan >= lifeTime)
                Destroy(this.gameObject);
        }
        

    }

    public GameObject GetHitParticle()
    {
        return hitParticle;
    }

}

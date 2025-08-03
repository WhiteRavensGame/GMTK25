using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Miniboss"))
        {
            Debug.Log("hit miniboss");
            if(collision.gameObject.TryGetComponent<FinalBossPattern>(out FinalBossPattern finalBoss))
            {
                finalBoss.HitBySecretMelee();
            }
            else if (collision.gameObject.TryGetComponent<MinibossScripted>(out MinibossScripted miniboss))
            {
                miniboss.HitBySecretMelee();
            }
        }
    }
}

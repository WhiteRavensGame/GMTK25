using Unity.VisualScripting;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float explosionForce = 10;
    private float explosionRadius = 3f;
    private float upwardsModifier = 1.5f;

    public GameObject explosionObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<BattleSpriteAction>(out BattleSpriteAction player))
            {
                //if the player is dead and flying from physics, don't kill them further. 
                if (!player.IsAlive())
                    return;

                player.PlayerTrippedExplosion();
                Vector2 hitPoint = (transform.position + player.transform.position) / 2f;
                Instantiate(explosionObject, hitPoint, Quaternion.identity);
            }

            //Fun physics
            if (collision.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                Vector2 explosionDirection = (rb.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(rb.transform.position, transform.position);
                float forceMultiplier = Mathf.Clamp01(1 - (distance / explosionRadius));

                Vector2 force = explosionDirection * explosionForce * forceMultiplier;
                force.y += upwardsModifier * explosionForce * forceMultiplier;

                rb.AddForce(force, ForceMode2D.Impulse);
            }


        }
    }

}

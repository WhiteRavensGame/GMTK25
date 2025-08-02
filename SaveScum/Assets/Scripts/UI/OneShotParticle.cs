using UnityEngine;

public class OneShotParticle : MonoBehaviour
{

    private float timeTillDestroy = 0.55f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("DestroyParticle", 0.55f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DestroyParticle()
    {
        Destroy(this.gameObject);
    }
}

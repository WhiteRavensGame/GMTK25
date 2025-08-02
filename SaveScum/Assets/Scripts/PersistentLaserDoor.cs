using UnityEngine;

public class PersistentLaserDoor : MonoBehaviour
{
    private bool isActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isActive = GameManager.Instance.GetLasersActive();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

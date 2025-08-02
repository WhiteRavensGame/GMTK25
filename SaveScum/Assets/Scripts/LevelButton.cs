using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelButton : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [SerializeField] protected float pressOffset = -0.15f;
    [SerializeField] protected float pressSpeed = 2f;
    [SerializeField] protected Color pressColor = Color.green;

    protected List<GameObject> pressers;

    protected Color originalColor;

    protected Vector3 unpressedPosition;
    protected Vector3 pressedPosition;
    protected bool isSteppedOn = false;

    protected float triggerCooldown = 0.5f;
    protected float lastTriggerTime = -1f;
    
    public bool IsButtonPressed()
    {
        return isSteppedOn;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalColor = spriteRenderer.color;
        unpressedPosition = transform.position;
        pressedPosition = unpressedPosition + new Vector3(0, pressOffset, 0);
        pressers = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = isSteppedOn ? pressedPosition : unpressedPosition;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * pressSpeed);

        if (isSteppedOn)
        {
            spriteRenderer.color = pressColor;
        }
        else
        {
            spriteRenderer.color = originalColor;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //prevents double quick presses of the button. 
        if (Time.time - lastTriggerTime < triggerCooldown) return;

        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Statue"))
        {
            isSteppedOn = true;
            Debug.Log("LASERS ON!");
            GameManager.Instance.SetLasersActive(true);
            if(!pressers.Contains(collision.gameObject))
                pressers.Add(collision.gameObject);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Statue"))
        {
            if (pressers.Contains(collision.gameObject))
                pressers.Remove(collision.gameObject);

            if (pressers.Count <= 0)
            {
                Debug.Log("LASERS OFF!");
                GameManager.Instance.SetLasersActive(false);
                isSteppedOn = false;
            }
                
            
        }
    }

}

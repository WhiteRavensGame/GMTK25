using UnityEngine;
using UnityEngine.UIElements;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private float pressOffset = -0.15f;
    [SerializeField] private float pressSpeed = 2f;
    [SerializeField] private Color pressColor = Color.green;

    private Color originalColor;

    private Vector3 unpressedPosition;
    private Vector3 pressedPosition;
    private bool isSteppedOn = false;
    
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
        if(collision.gameObject.CompareTag("Player"))
        {
            isSteppedOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isSteppedOn = false;
        }
    }
}

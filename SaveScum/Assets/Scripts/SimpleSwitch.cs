using UnityEngine;

public class SimpleSwitch : LevelButton
{
    [SerializeField] private GameObject blackOutFill;

    private void Update()
    {
        blackOutFill.SetActive(!GameManager.Instance.GetElectricityActive());

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

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Statue"))
        {
            isSteppedOn = true;
            Debug.Log("LIGHTS ON!");
            GameManager.Instance.SetElectricityActive(true);
            if (!pressers.Contains(collision.gameObject))
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
                Debug.Log("LIGHTS OFF!");
                GameManager.Instance.SetElectricityActive(false);
                isSteppedOn = false;
            }


        }
    }
}

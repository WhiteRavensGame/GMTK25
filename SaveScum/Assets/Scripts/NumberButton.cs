using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class NumberButton : LevelButton
{
    [SerializeField] private int number;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Color incorrectPressColor;

    [SerializeField] private GameObject explosiveObject;

    private bool incorrectPressed = false;


    //public bool IsButtonPressed()
    //{
    //    return isSteppedOn;
    //}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalColor = spriteRenderer.color;
        unpressedPosition = transform.position;
        pressedPosition = unpressedPosition + new Vector3(0, pressOffset, 0);
        pressers = new List<GameObject>();

        numberText.text = number.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = isSteppedOn ? pressedPosition : unpressedPosition;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * pressSpeed);

        if (incorrectPressed)
        {
            spriteRenderer.color = incorrectPressColor;
        }
        else if (isSteppedOn)
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

        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.TryGetComponent<BattleSpriteAction>(out BattleSpriteAction player))
            {
                if (!player.IsAlive())
                    return;
            }

            isSteppedOn = true;
            if (!pressers.Contains(collision.gameObject))
            {
                pressers.Add(collision.gameObject);
                
                if(!NumberPuzzle.Instance.numberGameSolved)
                    NumberPuzzle.Instance.EnterNumber(number, this);
            }
                

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (pressers.Contains(collision.gameObject))
                pressers.Remove(collision.gameObject);

            if (pressers.Count <= 0)
                isSteppedOn = false;

        }
    }

    public void SpawnExplosive()
    {
        incorrectPressed = true;

        //RIP Unitychan.
        Instantiate(explosiveObject, transform.position, Quaternion.identity);
    }

}

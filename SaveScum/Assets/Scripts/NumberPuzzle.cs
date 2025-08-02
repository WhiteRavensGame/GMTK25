using System;
using TMPro;
using UnityEngine;

public class NumberPuzzle : MonoBehaviour
{
    public static NumberPuzzle Instance;

    public int numbersEntered = 0;
    private int currentDigit = 1000;
    private int randomizedNumber = -1;

    public bool numberGameSolved = false;

    [SerializeField] private TextMeshProUGUI numbersEnteredText;
    [SerializeField] private GoalPoint goalPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            randomizedNumber = UnityEngine.Random.Range(1234, 9876);
        }
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        goalPoint.gameObject.SetActive(false);
        ResetNumber();
    }

    // Update is called once per frame
    void Update()
    {
        numbersEnteredText.text = numbersEntered.ToString("0000");
    }

    public void EnterNumber(int number, NumberButton numButton)
    {
        if (numberGameSolved)
            return;

        numbersEntered *= 10;
        numbersEntered += number;

        Debug.Log($"{numbersEntered} vs { randomizedNumber / currentDigit}");
        if (numbersEntered != randomizedNumber / currentDigit)
        {
            if (numbersEntered == 6 || numbersEntered == 69 || numbersEntered == 694 || numbersEntered == 6942 || numbersEntered == 69420)
            {
                if(numbersEntered == 69420)
                {
                    Debug.Log("WIN. Loophole found");
                    WinNumberGame();
                }
                    
            }
            else
            {
                //Lose. Kill player. 
                Debug.Log("Lose");
                numButton.SpawnExplosive();
                numbersEnteredText.color = Color.red;
                //ResetNumber();
            }
        }
        else if(numbersEntered == randomizedNumber)
        {
            Debug.Log("Win");
            WinNumberGame();
        }

        //you survived. next number.
        currentDigit = Math.Max(1, currentDigit/10);


    }

    public void WinNumberGame()
    {
        numberGameSolved = true;
        goalPoint.gameObject.SetActive(true);
        numbersEnteredText.color = Color.green;
    }

    public void ResetNumber()
    {
        if(numberGameSolved) 
            return;

        numbersEntered = 0;
        numbersEnteredText.color = Color.white;
        

        Debug.Log("Randomized number: " + randomizedNumber);
    }
}

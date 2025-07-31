using System;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private float maxTimeLeft = 3f;
    private float timeLeft;

    private GameState gameState = GameState.Playing;

    public event EventHandler OnTimeUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        timeLeft = maxTimeLeft;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameState == GameState.Playing)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft <= 0)
            {
                gameState = GameState.GameOver;
                OnTimeUp.Invoke(this, EventArgs.Empty);
            }

        }
            
    }

    private void RestartLevel()
    {

    }

}

public enum GameState
{
    None,
    Playing,
    GameOver,
    Intermission
}

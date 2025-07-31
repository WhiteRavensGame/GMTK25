using System;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Data management")]
    [SerializeField] private SaveData[] saveData;
    [SerializeField] private int activeSaveData = 0;

    [Header("Game balance")]
    [SerializeField]
    private float maxTimeLeft = 3f;
    private float timeLeft;
    private int currentLevel = 1;
    private Transform playerCharacter;

    private GameState gameState = GameState.Playing;

    //other timers
    private float loseTimer = 0;
    [SerializeField] private float maxLoseTimer = 2;
    

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

        BattleSpriteAction.OnPlayerDeath += BattleSpriteAction_OnPlayerDeath;

        saveData = new SaveData[3];

        
    }

    private void BattleSpriteAction_OnPlayerDeath(object sender, EventArgs e)
    {
        gameState = GameState.Lose;
        loseTimer = maxLoseTimer;
    }

    // Update is called once per frame
    void Update()
    {
        //save spots
        //Don't save when game over.
        if (gameState == GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                saveData[0] = new SaveData(currentLevel, CharacterType.UnityChan, playerCharacter.position.x, playerCharacter.position.y);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                saveData[1] = new SaveData(currentLevel, CharacterType.UnityChan, playerCharacter.position.x, playerCharacter.position.y);

            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                saveData[2] = new SaveData(currentLevel, CharacterType.UnityChan, playerCharacter.position.x, playerCharacter.position.y);
            }

            //reload
            else if(Input.GetKeyDown(KeyCode.F1))
            {
                ReloadSave(0);
            }
        }
        

        if(gameState == GameState.Playing)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft <= 0)
            {
                gameState = GameState.Lose;
                OnTimeUp?.Invoke(this, EventArgs.Empty);
            }

        }

        //delay opening lose window.
        if(gameState == GameState.Lose)
        {
            loseTimer -= Time.deltaTime;
            if (loseTimer <= 0)
                LevelLose();
        }
            
    }

    public void ReloadSave(int index)
    {
        if (saveData[index] == null)
        {
            Debug.Log($"ERROR. NO SAVE FOUND AT INDEX {index}");
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void SetCharacter(Transform character)
    {
        this.playerCharacter = character;
    }

    public float GetTimeLeft()
    {
        return timeLeft;
    }

    public void RestartLevel()
    {
        
    }

    public void LevelWin()
    {
        UIManager.Instance.ShowLevelComplete();
        gameState = GameState.Win;
    }
    private void LevelLose()
    {
        UIManager.Instance.ShowLevelFailed();
        gameState = GameState.Lose;
    }



}

public enum GameState
{
    None,
    Playing,
    Lose,
    Win,
    Intermission
}

public enum CharacterType
{
    NONE,
    UnityChan,
    Toko,
    Holger
}

using System;
using Unity.VisualScripting;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Data management")]
    [SerializeField] private SaveData[] saveData;
    private SaveData activeSaveData;

    [Header("Characters")]
    [SerializeField] private GameObject[] characters;

    [Header("Game balance")]
    [SerializeField]
    private float maxTimeLeft = 3f;
    private float timeLeft;
    private int currentLevel = 1;
    private GameObject playerCharacter;

    private GameState gameState = GameState.Playing;

    //other timers
    private float loseTimer = 0;
    [SerializeField] private float maxLoseTimer = 2;
    

    public event EventHandler OnTimeUp;
    public event EventHandler<SaveData> OnReloadSaveFile;
    public event EventHandler OnFullLevelReload;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        Debug.Log("GM START");
        timeLeft = maxTimeLeft;

        BattleSpriteAction.OnPlayerDeath += BattleSpriteAction_OnPlayerDeath;

        //There was a save data used to load. Load that save instead. 
        if (activeSaveData != null)
        {
            Debug.Log("respawn via save point");
            RespawnCharacterWithReload();
        }
        else if (playerCharacter == null)
        {
            Debug.Log("spawn totally new character at start pos.");
            SpawnNewPlayer();
        }

        
    }

    private void SpawnNewPlayer()
    {
        //look for the spawn point. 
        Transform startPos = GameObject.FindGameObjectWithTag("StartPos").transform;
        playerCharacter = Instantiate(characters[0], new Vector3(startPos.position.x, startPos.position.y, startPos.position.z), Quaternion.identity);
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
                saveData[0] = new SaveData(currentLevel, CharacterType.UnityChan, playerCharacter.transform.position.x, playerCharacter.transform.position.y);
                activeSaveData = saveData[0];
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                saveData[1] = new SaveData(currentLevel, CharacterType.Toko, playerCharacter.transform.position.x, playerCharacter.transform.position.y);
                activeSaveData = saveData[1];
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                saveData[2] = new SaveData(currentLevel, CharacterType.Holger, playerCharacter.transform.position.x, playerCharacter.transform.position.y);
                activeSaveData = saveData[2];
            }

            //reload
            else if(Input.GetKeyDown(KeyCode.F1) )
            {
                ReloadSave(0);
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                ReloadSave(1);
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                ReloadSave(2);
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
            activeSaveData = saveData[index];
            SceneManager.LoadScene(0); //QQQQ  change to the actual level next time
            //OnReloadSaveFile?.Invoke(this, activeSaveData);

            //QQQQ
            Invoke("RespawnCharacterWithReload", 0.15f);
            //RespawnCharacterWithReload();
        }
    }

    private void RespawnCharacterWithReload()
    {
        playerCharacter = Instantiate(characters[(int)activeSaveData.characterType], 
            new Vector3(activeSaveData.savePosX, activeSaveData.savePosY, 0), Quaternion.identity);
    }

    public void SetCharacter(GameObject character)
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
    NONE = -1,
    UnityChan,
    Toko,
    Holger
}

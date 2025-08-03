using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Data management")]
    [SerializeField] private SaveData[] saveData;
    private int activeSaveIndex;
    private float loadCooldown = 1f;
    private float loadTimer = 0f;

    [Header("Databases")]
    [SerializeField] private GameObject[] characters;
    [SerializeField] private GameObject[] characterStatues;
    [SerializeField] private LevelInfo[] levelInfos;

    [Header("Game balance")]
    private int currentLevel;
    private float timeLeft;

    private GameObject playerCharacter;
    private PlayerCharacter playerCharacterType = PlayerCharacter.UnityChan;

    private GameState gameState = GameState.Playing;

    //other timers
    private float loseTimer = 0;
    [SerializeField] private float maxLoseTimer = 2;

    public int randomizedNumber;
    private bool lasersOnGlobally;
    private bool electricityOnGlobally;

    public event EventHandler OnTimeUp;
    public event EventHandler OnLevelReload;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            

            saveData = new SaveData[3];
            for (int i = 0; i < saveData.Length; i++)
            {
                saveData[i] = new SaveData();
            }

            randomizedNumber = UnityEngine.Random.Range(123, 987);
            //prevents the hundredths place to be 4 because of the loophole 420. 
            if (randomizedNumber / 100 == 4) 
                randomizedNumber -= UnityEngine.Random.Range(100, 200);
            lasersOnGlobally = true;
            electricityOnGlobally = false;
        }
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        Debug.Log("GM START");
        ResetTimer();

        BattleSpriteAction.OnPlayerDeath += BattleSpriteAction_OnPlayerDeath;

        //There was a save data used to load. Load that save instead. 
        if (saveData[activeSaveIndex] != null && saveData[activeSaveIndex].isUsed)
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

    public bool GetLasersActive()
    {
        return lasersOnGlobally;
    }
    public void SetLasersActive(bool active)
    {
        lasersOnGlobally = active;
    }

    public bool GetElectricityActive()
    {
        return electricityOnGlobally;
    }
    public void SetElectricityActive(bool active)
    {
        electricityOnGlobally = active;
    }

    public bool HasSaveFileAtIndex(int index)
    {
        
        if (index < 0 || index >= saveData.Length)
            return false;
        else if (!saveData[index].isUsed)
        {
            //Debug.Log($"{saveData[index].level}, {saveData[index].isUsed.ToString()}");

            return false;
        }
            
        else
        {   //save data exists in this index slot.
            //Debug.Log($"{saveData[index].level}, {saveData[index].characterType.ToString()}");
                return true;
        }
            
    }

    public void BeginReturnToMainMenu()
    {
        UIManager.Instance.ShowLoadTransition();
        SceneManager.LoadScene(0);
        Invoke("ReturnToMainMenu", 0.15f);
    }

    private void ReturnToMainMenu()
    {
        currentLevel = 0; //restart back to main menu
        RespawnCharacterHardReset();
        ResetTimer();
    }

    //Only call these methods from Main Menu-----
    public void StartNewGameFromMainMenu(int slot)
    {
        
        SetActiveSaveSlot(slot-1);
        LoadNextLevel();
        InitializeSaveData(slot - 1);
    }

    private void InitializeSaveData(int index)
    {
        //QQQQ. Have a more properly defined save starting position. 
        //saveData[index] = new SaveData(1, playerCharacterType, -1.5f, 0);
    }

    public void SetActiveSaveSlot(int index)
    {
        activeSaveIndex = index;
    }
    //------------------------------------------

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

        if (loadTimer > 0)
            loadTimer -= Time.deltaTime;

        //save spots
        //Don't save when game over.
        if (gameState == GameState.Playing)
        {
            if ( Input.GetKeyDown(KeyCode.E) && currentLevel < 8 && currentLevel > 0 )
            {
                //saveData[0] = new SaveData(currentLevel, playerCharacterType, playerCharacter.transform.position.x, playerCharacter.transform.position.y);
                //activeSaveData = saveData[0];
                saveData[activeSaveIndex] = new SaveData(currentLevel, playerCharacterType, playerCharacter.transform.position.x, playerCharacter.transform.position.y);
                UIManager.Instance.SaveAtSlot(activeSaveIndex, saveData[activeSaveIndex]);
            }
            //else if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    //saveData[1] = new SaveData(currentLevel, playerCharacterType, playerCharacter.transform.position.x, playerCharacter.transform.position.y);
            //    //activeSaveData = saveData[1];
            //    saveData[activeSaveIndex] = new SaveData(currentLevel, playerCharacterType, playerCharacter.transform.position.x, playerCharacter.transform.position.y);
            //    UIManager.Instance.SaveAtSlot(activeSaveIndex, saveData[activeSaveIndex]);
            //}
            //else if (Input.GetKeyDown(KeyCode.Alpha3))
            //{
            //    //saveData[2] = new SaveData(currentLevel, playerCharacterType, playerCharacter.transform.position.x, playerCharacter.transform.position.y);
            //    //activeSaveData = saveData[2];
            //    saveData[activeSaveIndex] = new SaveData(currentLevel, playerCharacterType, playerCharacter.transform.position.x, playerCharacter.transform.position.y);
            //    UIManager.Instance.SaveAtSlot(activeSaveIndex, saveData[activeSaveIndex]);
            //}

        }

        //player can load during playing or when they lost. 
        if ( (gameState == GameState.Playing || gameState == GameState.Lose) && currentLevel < 8 )
        {
            //don't spam load, or game breaks.
            if(loadTimer <= 0)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ReloadSave(0);
                    loadTimer = loadCooldown;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ReloadSave(1);
                    loadTimer = loadCooldown;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    ReloadSave(2);
                    loadTimer = loadCooldown;
                }
            }
            
        }
        


        if (gameState == GameState.Playing)
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

    public void ReloadSave(int index = -1)
    {
        UIManager.Instance.ShowLoadTransition();

        if(index == -1) //hard reset flag
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Invoke("RespawnCharacterHardReset", 0.15f);
            ResetTimer();
            
        }
        else if (saveData[index] == null)
        {
            Debug.Log($"ERROR. NO SAVE FOUND AT INDEX {index}");
        }
        else if (saveData[index].level <= 0)
        {
            //if save data haven't been made yet, don't load it (it will break).
        }
        else
        {
            currentLevel = saveData[index].level;
            SetActiveSaveSlot(index);
            
            SceneManager.LoadScene(saveData[index].level);
            //OnReloadSaveFile?.Invoke(this, activeSaveData);
            
            //QQQQ
            Invoke("RespawnCharacterWithReload", 0.15f);
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        timeLeft = levelInfos[currentLevel].timeLimit;
    }
    private void RespawnCharacterHardReset()
    {
        Transform startPos = GameObject.FindGameObjectWithTag("StartPos").transform;
        playerCharacter = Instantiate(characters[(int)playerCharacterType], startPos.position, Quaternion.identity);

        SpawnStatuesFromSaveFiles();

        gameState = GameState.Playing;

        OnLevelReload?.Invoke(this, EventArgs.Empty);
    }

    private void RespawnCharacterWithReload()
    {
        playerCharacter = Instantiate(characters[(int)saveData[activeSaveIndex].characterType], 
            new Vector3(saveData[activeSaveIndex].savePosX, saveData[activeSaveIndex].savePosY, 0), Quaternion.identity);

        SpawnStatuesFromSaveFiles();

        gameState = GameState.Playing;

        //if the player has unlocked number puzzle, just assign its combination. 
        if(NumberPuzzle.Instance != null)
        {
            NumberPuzzle.Instance.SetCurrentEnteredNumber(saveData[activeSaveIndex].numberCombinationProgress);
        }

        OnLevelReload?.Invoke(this, EventArgs.Empty);
    }

    private void SpawnStatuesFromSaveFiles()
    {

        foreach(SaveData saveFile in saveData)
        {
            //UNCOMMENT THIS PART IF DEBUGGING:
            if (saveFile == saveData[activeSaveIndex])
            {
                Debug.Log("Save file found at " + activeSaveIndex + ". Skippin");
                continue;
            }
                

            Debug.Log("Check statue save file");
            if(saveFile.isUsed)
            {
                Debug.Log("save exust");
                if(saveFile.level == currentLevel)
                {
                    Instantiate(characterStatues[(int)saveFile.characterType], new Vector3(saveFile.savePosX, saveFile.savePosY, 0), Quaternion.identity);
                }
            }
        }
    }

    public float GetTimeLeft()
    {
        return timeLeft;
    }

    public void HardResetLevel()
    {
        Debug.Log("Force hard reset (aka load autosave at start of level)");
        ReloadSave(-1);
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        
        UIManager.Instance.ShowLoadTransition();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        Debug.Log(SceneManager.GetActiveScene().buildIndex);

        //QQQQ. Just hide it when reaching final boss and end credits. 
        if (currentLevel >= 8)
            UIManager.Instance.DisplaySealedPlasterObject(true);
        else
            UIManager.Instance.DisplaySealedPlasterObject(false);

        //QQQQ check if it's the end screen 
        if (currentLevel >= 9)
            UIManager.Instance.ShowGameFinishedScreen();

            Invoke("RespawnCharacterHardReset", 0.15f);
        ResetTimer();
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

public enum PlayerCharacter
{
    NONE = -1,
    UnityChan,
    Toko,
    Holger
}

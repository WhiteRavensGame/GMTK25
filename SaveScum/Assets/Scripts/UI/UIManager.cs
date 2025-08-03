using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject levelClearPanel;
    [SerializeField] private GameObject levelFailedPanel;
    [SerializeField] private UISaveSlot[] saveSlots;

    [Header("Buttons for Events")]
    [SerializeField] private Button hardResetLevelButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button backToMainMenuButton;

    [Header("Sprites")]
    [SerializeField]
    private Sprite[] characterSprites;
    [SerializeField] private Animator transitionPanelAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        hardResetLevelButton.onClick.AddListener(() =>
        {
            GameManager.Instance.HardResetLevel();
        });
        nextLevelButton.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadNextLevel();
        });
        backToMainMenuButton.onClick.AddListener(() =>
        {
            GameManager.Instance.BeginReturnToMainMenu();
        });

        GameManager.Instance.OnLevelReload += GameManager_OnLevelReload;
        levelClearPanel.SetActive(false);
        levelFailedPanel.SetActive(false);
    }

    private void GameManager_OnLevelReload(object sender, System.EventArgs e)
    {
        levelClearPanel.SetActive(false);
        levelFailedPanel.SetActive(false);
    }

    public void ShowLevelComplete()
    {
        levelClearPanel.SetActive(true);
    }
    public void ShowLevelFailed()
    {
        levelFailedPanel.SetActive(true);
    }

    public void SaveAtSlot(int slotID, SaveData data)
    {
        Sprite sprite = characterSprites[(int)data.characterType];
        saveSlots[slotID].AssignSaveDataToSlot(data.characterType, data.level, sprite);
    }

    public void ShowLoadTransition()
    {
        transitionPanelAnimator.SetTrigger("FadeBlack");
    }
}

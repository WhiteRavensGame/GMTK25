using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotPortal : MonoBehaviour
{
    [SerializeField] private int saveSlotNumber;
    [SerializeField] private BoxCollider2D boxCollider;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshProUGUI portalText;

    private bool entered = false;


    private bool isLocked;

    void Start()
    {
        if(GameManager.Instance.HasSaveFileAtIndex(saveSlotNumber-1))
        {
            Debug.Log($"Slot {saveSlotNumber} has a save file");
            isLocked = true;
            //boxCollider.enabled = false;
            spriteRenderer.enabled = false;
        }
        else
        {
            Debug.Log($"Slot {saveSlotNumber} has NO save file");
            isLocked = false;
            portalText.text = "Slot" + saveSlotNumber;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (entered)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        entered = true;

        if (!isLocked)
            GameManager.Instance.StartNewGameFromMainMenu(saveSlotNumber);
        else
        {
            GameManager.Instance.SetActiveSaveSlot(saveSlotNumber-1);
            GameManager.Instance.ReloadSave(saveSlotNumber);
        }
            
        
       //After creating the initial save, load the player into the first level. 

    }


}

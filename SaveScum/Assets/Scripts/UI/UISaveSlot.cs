using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UISaveSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI slotNumberText;
    [SerializeField] private TextMeshProUGUI emptyText;
    [SerializeField] private Image characterSprite;
    [SerializeField] private TextMeshProUGUI levelText;
    

    private bool slotEmpty;

    private void Start()
    {
        slotEmpty = true;
        emptyText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(false);
        characterSprite.gameObject.SetActive(false);
    }

    public void AssignSaveDataToSlot(PlayerCharacter character, int level, Sprite sprite)
    {
        slotEmpty = false;
        emptyText.gameObject.SetActive(false);
        levelText.gameObject.SetActive(true);
        characterSprite.gameObject.SetActive(true);

        int index = (int)character;
        characterSprite.sprite = sprite;
        levelText.text = level.ToString();
    }

}

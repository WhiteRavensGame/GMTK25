using UnityEngine;

public class LaserDoor : MonoBehaviour
{
    [SerializeField] private LevelButton[] levelButtons;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D deathCollider;

    private bool IsOn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool unlocked = false;

        if(levelButtons.Length > 0)
        {
            unlocked = true;
            foreach (LevelButton levelButton in levelButtons)
            {
                if (!levelButton.IsButtonPressed())
                {
                    unlocked = false;
                    break;
                }
            }
        }

        IsOn = unlocked;
        spriteRenderer.enabled = !unlocked;
        deathCollider.enabled = !unlocked;

    }
}

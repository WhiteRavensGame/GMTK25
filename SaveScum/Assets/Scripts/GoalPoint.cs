using System;
using UnityEngine;

public class GoalPoint : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.LevelWin();
            if(collision.TryGetComponent<BattleSpriteAction>(out BattleSpriteAction player))
            {
                player.PlayVictoryFanfare();
            }
        }

            
    }
}

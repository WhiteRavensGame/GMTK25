using System;
using UnityEngine;

public class GoalPoint : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Instance.LevelWin();
    }
}

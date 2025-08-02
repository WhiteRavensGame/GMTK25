using System;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int level;
    public PlayerCharacter characterType;
    public float savePosX;
    public float savePosY;

    public bool isUsed;

    public SaveData()
    {
        level = 0;
        characterType = PlayerCharacter.UnityChan;
        savePosX = -1;
        savePosY = 0;

        isUsed = false;
    }

    public SaveData(int level, PlayerCharacter characterType, float savePosX, float savePosY)
    {
        this.level = level;
        this.characterType = characterType;
        this.savePosX = savePosX;
        this.savePosY = savePosY;

        isUsed = true;
    }
}

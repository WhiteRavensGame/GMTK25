using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int level { get; }
    public PlayerCharacter characterType { get; }
    public float savePosX { get; }
    public float savePosY { get; }

    public SaveData(int level, PlayerCharacter characterType, float savePosX, float savePosY)
    {
        this.level = level;
        this.characterType = characterType;
        this.savePosX = savePosX;
        this.savePosY = savePosY;
    }
}

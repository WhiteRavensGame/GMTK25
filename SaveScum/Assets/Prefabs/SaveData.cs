using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int level { get; }
    public CharacterType characterType { get; }
    public float savePosX { get; }
    public float savePosY { get; }

    public SaveData(int level, CharacterType characterType, float savePosX, float savePosY)
    {
        this.level = level;
        this.characterType = characterType;
        this.savePosX = savePosX;
        this.savePosY = savePosY;
    }
}

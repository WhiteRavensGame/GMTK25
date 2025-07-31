using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    private int level;
    private CharacterType characterType;
    private float savePosX;
    private float savePosY;

    public SaveData(int level, CharacterType characterType, float savePosX, float savePosY)
    {
        this.level = level;
        this.characterType = characterType;
        this.savePosX = savePosX;
        this.savePosY = savePosY;
    }
}

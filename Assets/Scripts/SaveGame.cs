using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SaveGame
{
    public int currentLevelID = 0;
    public int unlockedLevels = 1;
    public Colors[][] CurrentLevel = new Colors[10][];
}

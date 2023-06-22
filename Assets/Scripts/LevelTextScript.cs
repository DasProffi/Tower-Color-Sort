using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelTextScript : MonoBehaviour
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        text.text = "Level " + Math.Max(SaveGameManager.Instance.SaveGame.currentLevelID, 1);
    }
}

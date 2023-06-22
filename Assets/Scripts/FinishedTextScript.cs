using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishedTextScript : MonoBehaviour
{
    public Text text;

    // Update is called once per frame
    void Update()
    {
        text.color = GameState.Instance.IsFinished ? Color.black : Color.clear;
    }
}

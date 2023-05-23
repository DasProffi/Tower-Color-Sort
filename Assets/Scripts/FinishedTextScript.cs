using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishedTextScript : MonoBehaviour
{
    public Text Text;
    private GameManagerScript _gameManagerScript;
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        _gameManagerScript = gameManager.GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(_gameManagerScript.GameState.IsFinished);
        Text.color = _gameManagerScript.GameState.IsFinished ? Color.black : Color.clear;
    }
}

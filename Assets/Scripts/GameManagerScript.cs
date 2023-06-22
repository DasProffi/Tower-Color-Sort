using System;
using UnityEngine;
using Random = System.Random;

public class GameManagerScript : MonoBehaviour
{
    public GameObject towerRendererPrefab;
    private int _selectedTowerID = -1;
    private GameObject[] _towersRenderer;

    private bool _isSolving;
    private bool _isGenerating;
    private double _timer;
    public double solveStepTime = 0.4;
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameState.Instance.IsAutosolving)
        {
            GameState.Instance.ChangeLevel(new Random().Next());
            LayoutAndAssignComponents();
            StartSolve();
            return;
        }
        SaveGame saveGame = SaveGameManager.Instance.SaveGame;
        if (saveGame.currentLevelID == 0)
        {
            saveGame.currentLevelID = 1;
            GameState.Instance.ChangeLevel(saveGame.currentLevelID);
        }
        else
        {
            GameState.Instance.LoadGame(saveGame.CurrentLevel);
        }
        LayoutAndAssignComponents();
        Save();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isSolving)
        {
            return;
        }

        _timer += Time.deltaTime;
        if (_timer > solveStepTime)
        {
            _timer -= solveStepTime;
            SolveStep();
        }
    }

    private void SolveStep()
    {
        if (GameState.Instance.HasUndo())
        {   
            GameState.Instance.Undo();
            Rerender();
        }
        else if(GameState.Instance.Solution.Count > 0)
        {
            var (from, to) = GameState.Instance.Solution.Pop();
            if (GameState.Instance.TryMove(to, from, false))
            {
                Debug.Log("("+to+","+from+")");
            }
            else
            {   
                // These could potentially cause problems
                // Caused by only taking one from the Stack
                Debug.LogWarning("("+to+","+from+")");
            }
            Rerender();
        }
        else
        {
            GameState.Instance.ChangeSeed(new Random().Next());
            Reset();
        }
    }

    // Saves the current State
    private void Save()
    {
        SaveGameManager.Instance.SaveGame.CurrentLevel = GameState.Instance.Export();
        SaveGameManager.Instance.Save();
    }
    
    public void StartSolve()
    {   
        if(IsBlockingInput()) return;
        
        _isSolving = true;
    }

    // returns whether the input tower should be selected
    public bool SetSelectedTower(int id)
    {
        if(IsBlockingInput()) return false;
        
        if (_selectedTowerID == id)
        {
            _selectedTowerID = -1;
            return false;
        }
        if (_selectedTowerID == -1)
        {
            _selectedTowerID = id;
            return true;
        }
        if (GameState.Instance.TryMove(_selectedTowerID, id))
        {
            ClearSelections();
            Rerender();
            if (!GameState.Instance.IsAutosolving)
            {   
                if (GameState.Instance.GetIsFinished())
                {
                    SaveGameManager.Instance.SaveGame.currentLevelID += 1;
                    GameState.Instance.ChangeLevel(SaveGameManager.Instance.SaveGame.currentLevelID);
                }
                Save();
            }
        }
        return false;
    }

    public void Redo()
    {   
        if(IsBlockingInput()) return;
        
        GameState.Instance.Redo();
        Rerender();
    }
    
    public void Undo()
    {   
        if(IsBlockingInput()) return;
        
        GameState.Instance.Undo();
        Rerender();
    }

    public void Reset()
    {
        if(IsBlockingInput()) return;

        if (_towersRenderer != null)
        {
            foreach (var o in _towersRenderer)
            {
               Destroy(o);
            }
        }
        _isGenerating = true;
        GameState.Instance.Reset();
        if (!GameState.Instance.IsAutosolving)
        {
            Save();
        }
        LayoutAndAssignComponents();
        _isGenerating = false;
    }

    private void LayoutAndAssignComponents()
    {
        int numberOfTowers = GameState.Instance.NumberOfTowers;
        int boxesInTower = GameState.Instance.Towers[0].GetTowerHeight();
        _towersRenderer = new GameObject[numberOfTowers];
        float width = Screen.width;
        float usableWidth = width * 0.9f;
        float paddingPercentage = 1.5f;
        float towerWidth = usableWidth / numberOfTowers / paddingPercentage; // 0.5 as spacing between
        float minimumTowerWidth = 120;
        float rows = 1;
        int towersPerRow = numberOfTowers;
        if (towerWidth < minimumTowerWidth)
        {
            towersPerRow = (int)Math.Floor(usableWidth / minimumTowerWidth / paddingPercentage);
            towerWidth = minimumTowerWidth;
            rows = (int)Math.Ceiling((double)numberOfTowers / towersPerRow);
        }
        float boxHeight = towerWidth / 2;
        float towerHeight = boxHeight * boxesInTower;
        for (int i = 0; i < numberOfTowers; i++)
        {
            // Create a new box GameObject
            int rowIndex = i / towersPerRow;
            int towersInCurrentRow = rowIndex == numberOfTowers/towersPerRow ? numberOfTowers % towersPerRow :towersPerRow;
            _towersRenderer[i] = Instantiate(towerRendererPrefab, transform);
            TowerRendererScript towerRendererScript = _towersRenderer[i].GetComponent<TowerRendererScript>();
            towerRendererScript.id = i;
            RectTransform rectTransform = _towersRenderer[i].GetComponent<RectTransform>();
            float rowsYStart = rows * paddingPercentage * towerHeight / 2;
            float towerY = -rowIndex * paddingPercentage * towerHeight + rowsYStart;
            float columnXStart = -(towersInCurrentRow-1) * paddingPercentage * towerWidth /2;
            float towerX = (i % towersPerRow) * paddingPercentage * towerWidth + columnXStart;
            rectTransform.localPosition = new Vector3(towerX, towerY);
            rectTransform.sizeDelta = new Vector2(towerWidth, boxHeight*boxesInTower);
            BoxCollider2D boxCollider2D = _towersRenderer[i].GetComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(towerWidth, boxHeight*boxesInTower);
        }
    }

    public void ClearSelections()
    {
        _selectedTowerID = -1;
        for (int i = 0; i < GameState.Instance.NumberOfTowers; i++)
        {
            TowerRendererScript towerRendererScript = _towersRenderer[i].GetComponent<TowerRendererScript>();
            towerRendererScript.SetSelected(false);
        }
    }
    
    public void Rerender()
    {
        for (int i = 0; i < GameState.Instance.NumberOfTowers; i++)
        {
            TowerRendererScript towerRendererScript = _towersRenderer[i].GetComponent<TowerRendererScript>();
            towerRendererScript.SetTower(GameState.Instance.Towers[i]);
        }
    }

    private bool IsBlockingInput()
    {
        return _isGenerating || _isSolving;
    }
}

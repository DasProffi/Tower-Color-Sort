using System;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject towerRendererPrefab; 
    public GameState GameState;
    private int _selectedTowerID = -1;
    private GameObject[] _towersRenderer;

    private bool _isSolving = false;
    private bool _isGenerating = false;
    private double _timer = 0;
    public double solveStepTime = 0.2;
    // Start is called before the first frame update
    
    void Start()
    {
        GameState = new GameState();
        int numberOfTowers = GameState.NumberOfTowers;
        int boxesInTower = GameState.Towers[0].GetTowerHeight();
        _towersRenderer = new GameObject[numberOfTowers];
        float width = Screen.width;
        float usableWidth = width * 0.9f;
        float paddingPercentage = 1.5f;
        float towerWidth = usableWidth / numberOfTowers / paddingPercentage; // 0.5 as spacing between
        float minimumTowerWidth = 90;
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
            float towerX = usableWidth / towersInCurrentRow * (-towersInCurrentRow / 2 + i%towersPerRow + (towersInCurrentRow % 2 == 0 ? 0.5f : 0));
            rectTransform.localPosition = new Vector3(towerX, towerY);
            rectTransform.sizeDelta = new Vector2(towerWidth, boxHeight*boxesInTower);
            BoxCollider2D boxCollider2D = _towersRenderer[i].GetComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(towerWidth, boxHeight*boxesInTower);
        }
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
            if (GameState.HasUndo())
            {   
                GameState.Undo();
                Rerender();
            }
            else if(GameState.Solution.Count > 0)
            {
                var (from, to) = GameState.Solution.Pop();
                if (GameState.TryMove(to, from, false))
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
                _isSolving = false;
            }
        }
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
        if (GameState.TryMove(_selectedTowerID, id))
        {
            ClearSelections();
            Rerender();
        }
        return false;
    }

    public void Redo()
    {   
        if(IsBlockingInput()) return;
        
        GameState.Redo();
        Rerender();
    }
    
    public void Undo()
    {   
        if(IsBlockingInput()) return;
        
        GameState.Undo();
        Rerender();
    }

    public void Reset()
    {
        if(IsBlockingInput()) return;
        
        _isGenerating = true;
        ClearSelections();
        GameState = new GameState(DateTime.UtcNow.Millisecond);
        Rerender();
        _isGenerating = false;
    }

    public void ClearSelections()
    {
        _selectedTowerID = -1;
        for (int i = 0; i < GameState.NumberOfTowers; i++)
        {
            TowerRendererScript towerRendererScript = _towersRenderer[i].GetComponent<TowerRendererScript>();
            towerRendererScript.SetSelected(false);
        }
    }
    
    public void Rerender()
    {
        for (int i = 0; i < GameState.NumberOfTowers; i++)
        {
            TowerRendererScript towerRendererScript = _towersRenderer[i].GetComponent<TowerRendererScript>();
            towerRendererScript.SetTower(GameState.Towers[i]);
        }
    }

    private bool IsBlockingInput()
    {
        return _isGenerating || _isSolving;
    }
}

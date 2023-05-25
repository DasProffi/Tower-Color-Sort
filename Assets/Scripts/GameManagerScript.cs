using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    public GameObject towerRendererPrefab; 
    public GameState GameState;
    private int _selectedTowerID = -1;
    private GameObject[] _towersRenderer;
    // Start is called before the first frame update
    
    void Start()
    {
        GameState = new GameState();
        int numberOfTowers = GameState.NumberOfTowers;
        int boxesInTower = GameState.Towers[0].GetStackSize();
        _towersRenderer = new GameObject[numberOfTowers];
        float width = Screen.width;
        float usableWidth = width * 0.8f;
        float towerWidth = usableWidth / numberOfTowers / 1.5f; // 0.5 as spacing between
        float boxHeight = towerWidth / 2;
        Debug.Log(towerWidth + "," + boxHeight);
        for (int i = 0; i < numberOfTowers; i++)
        {
            // Create a new box GameObject
            _towersRenderer[i] = Instantiate(towerRendererPrefab, transform);
            TowerRendererScript towerRendererScript = _towersRenderer[i].GetComponent<TowerRendererScript>();
            towerRendererScript.id = i;
            RectTransform rectTransform = _towersRenderer[i].GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(usableWidth / numberOfTowers *(-numberOfTowers / 2 + i + (numberOfTowers % 2 == 0 ? 0.5f : 0)), 0);
            rectTransform.sizeDelta = new Vector2(towerWidth, boxHeight*boxesInTower);
            BoxCollider2D boxCollider2D = _towersRenderer[i].GetComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(towerWidth, boxHeight*boxesInTower);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // returns whether the input tower should be selected
    public bool SetSelectedTower(int id)
    {
        if (_selectedTowerID == id)
        {
            _selectedTowerID = -1;
            return false;
        }
        else if (_selectedTowerID == -1)
        {
            _selectedTowerID = id;
            return true;
        }
        else
        {
            if (GameState.TryMove(_selectedTowerID, id))
            {
                ClearSelections();
                Rerender();
            }
            return false;
        }
    }

    public void Redo()
    {
        GameState.Redo();
        Rerender();
    }
    
    public void Undo()
    {
        GameState.Undo();
        Rerender();
    }

    public void Reset()
    {
        ClearSelections();
        GameState = new GameState();
        Rerender();
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
}

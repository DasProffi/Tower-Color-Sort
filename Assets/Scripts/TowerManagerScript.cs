using UnityEngine;
using UnityEngine.Serialization;

public class TowerManagerScript : MonoBehaviour
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
        _towersRenderer = new GameObject[numberOfTowers];
        for (int i = 0; i < numberOfTowers; i++)
        {
            // Create a new box GameObject
            _towersRenderer[i] = Instantiate(towerRendererPrefab, transform);
            TowerRendererScript towerRendererScript = _towersRenderer[i].GetComponent<TowerRendererScript>();
            towerRendererScript.id = i;
            RectTransform rectTransform = _towersRenderer[i].GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(3*(-numberOfTowers / 2 + i), 0);
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
            if (GameState.Move(_selectedTowerID, id))
            {
                ClearSelections();
                Rerender();
            }
            return false;
        }
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

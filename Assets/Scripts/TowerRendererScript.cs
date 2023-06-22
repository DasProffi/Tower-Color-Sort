using UnityEngine;
using UnityEngine.Serialization;

public class TowerRendererScript : MonoBehaviour
{
    public GameObject boxPrefab;
    public int id;
    [FormerlySerializedAs("towerManagerScript")] public GameManagerScript gameManagerScript;
    private Tower _tower;
    private bool _isSelected = false;
    private GameObject[] _boxes;
    private GameObject _selectMarker;

    // Start is called on the frame when a script is enabled just before any of the Update
    private void Start()
    {
        gameManagerScript = transform.parent.GetComponent<GameManagerScript>();
        _tower = GameState.Instance.Towers[id];
        _selectMarker = transform.Find("SelectMarker").gameObject;
        _selectMarker.SetActive(false);
        _boxes = new GameObject[_tower.GetTowerHeight()];
        float boxHeight = GetComponent<RectTransform>().sizeDelta.y / _tower.GetTowerHeight();
        float boxWidth = GetComponent<RectTransform>().sizeDelta.x;
        _selectMarker.GetComponent<RectTransform>().localScale = new Vector3(2.2f,0.2f) * boxHeight;
        for (int i = 0; i < _tower.GetTowerHeight(); i++)
        {
            // Create a new box GameObject
            _boxes[i] = Instantiate(boxPrefab, transform);
            _boxes[i].name = "TowerBox" + i;
            _boxes[i].GetComponent<BoxPrefabScript>().Initialize(_tower.GetLayerColor(i));
            
            // Set the position and size of the box using RectTransform
            RectTransform rectTransform = _boxes[i].GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(0,  boxHeight * (_tower.GetTowerHeight() / 2 - i - 0.5f));
            rectTransform.sizeDelta = new Vector2(boxWidth,boxHeight);
        }
    }

    private void OnMouseOver(){
        if(Input.GetMouseButtonDown(0))
        {
            SetSelected(gameManagerScript.SetSelectedTower(id));
        }
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        _selectMarker.SetActive(isSelected);
    } 
    
    public void SetTower(Tower tower, bool isRendering = true)
    {
        _tower = tower;
        if (isRendering)
        {
            RenderTower();
        }
    }

    private void RenderTower()
    {
        for (int i = 0; i < _tower.GetTowerHeight(); i++)
        {
            BoxPrefabScript boxPrefabScript = _boxes[i].GetComponent<BoxPrefabScript>();
            boxPrefabScript.ChangeColor(_tower.GetLayerColor(i));
        }
    }
}

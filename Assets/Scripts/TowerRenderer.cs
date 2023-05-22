using System;
using UnityEngine;

public class TowerRenderer : MonoBehaviour
{
    public GameObject boxPrefab; 
    private Tower _tower= new Tower();
    private bool _isSelected = true;
    private bool _clicked = false;
    
    private const float BoxWidth = 200f;   // Width of each box
    private const float BoxHeight = 100f;  
    // Start is called before the first frame update
    void Start()
    {   
        for (int i = 0; i < _tower.GetStackSize(); i++)
        {
            // Create a new box GameObject
            GameObject box = Instantiate(boxPrefab, transform);

            // Set the position and size of the box using RectTransform
            RectTransform rectTransform = box.GetComponent<RectTransform>();
            rectTransform.position = new Vector2(0f, -i);
            rectTransform.sizeDelta = new Vector2(BoxWidth, BoxHeight);

            BoxPrefabScript boxPrefabScript = box.GetComponent<BoxPrefabScript>();
            boxPrefabScript.SetColor(Constants.ColorForColor(_tower.GetLayerColor(i)));
        }
        RectTransform towerRectTransform = GetComponent<RectTransform>();
        towerRectTransform.sizeDelta = new Vector2(BoxWidth, BoxHeight*_tower.GetStackSize());
        BoxCollider2D collider2D = GetComponent<BoxCollider2D>();
        collider2D.size = new Vector2(2, _tower.GetStackSize());
        collider2D.offset = new Vector2(0, -1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseUp()
    {
        _clicked = false;
    }

    private void OnMouseOver(){
        if(Input.GetMouseButtonDown(0) && !_clicked)
        {
            _isSelected = !_isSelected;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.position += (_isSelected ? -1 : 1) * new Vector3(0, 1, 0);
        }
    }
}

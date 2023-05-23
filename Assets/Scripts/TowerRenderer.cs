using System;
using UnityEngine;

public class TowerRenderer : MonoBehaviour
{
    public GameObject boxPrefab; 
    private Tower _tower= new Tower();
    private bool _isSelected = true;

    private GameObject[] _boxes;
    // Start is called before the first frame update
    private void Start()
    {
        _boxes = new GameObject[_tower.GetStackSize()];
        for (int i = 0; i < _tower.GetStackSize(); i++)
        {
            // Create a new box GameObject
            _boxes[i] = Instantiate(boxPrefab, transform);
            _boxes[i].name = "TowerBox" + i;

            // Set the position and size of the box using RectTransform
            RectTransform rectTransform = _boxes[i].GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(0, -i);

            BoxPrefabScript boxPrefabScript = _boxes[i].GetComponent<BoxPrefabScript>();
            boxPrefabScript.SetColor(Constants.ColorForColor(_tower.GetLayerColor(i)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver(){
        if(Input.GetMouseButtonDown(0))
        {
            SetSelected(!_isSelected);
        }
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localPosition += (_isSelected ? -1 : 1) * new Vector3(0, 1, 0);
    } 
}

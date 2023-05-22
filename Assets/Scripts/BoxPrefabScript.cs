using System;
using UnityEngine;

public class BoxPrefabScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;  // Reference to the Image component of the box
    private RectTransform _rectTransform;
    
    // Use this method to initialize the box properties
    public void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetColor(Color boxColor)
    {
        // Set the color of the box using the Image component
        spriteRenderer.color = boxColor;
    }
}
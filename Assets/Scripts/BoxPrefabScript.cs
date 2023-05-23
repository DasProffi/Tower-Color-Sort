using System;
using UnityEngine;

public class BoxPrefabScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;  // Reference to the Image component of the box

    public void SetColor(Color boxColor)
    {
        // Set the color of the box using the Image component
        spriteRenderer.color = boxColor;
    }
}
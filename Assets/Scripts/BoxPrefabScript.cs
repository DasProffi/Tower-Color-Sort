using System;
using UnityEngine;

public class BoxPrefabScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;  // Reference to the Image component of the box
    public Sprite normal;
    public Sprite empty;

    // Set the color of the box using the Color
    public void SetColor(Colors boxColor)
    {
        if (boxColor == Colors.none)
        {   
            spriteRenderer.sprite = empty;
            spriteRenderer.color = Color.black;
        }
        else
        {
            spriteRenderer.sprite = normal;
            spriteRenderer.color = Constants.ColorForColor(boxColor);
        }
    }
}
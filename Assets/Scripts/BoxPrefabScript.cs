using System;
using UnityEngine;

public class BoxPrefabScript : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;  // Reference to the Image component of the box
    public double animationTime = 0.35;
    
    private float _scale;
    
    private bool _isAnimating;
    private bool _isAppearingAnimation;
    private double _currentAnimationTime;
    
    public void Start()
    {
        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        // Set the size of animated square
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = size;
        transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = size;
        _scale = size.y;
        spriteRenderer.transform.localScale = new Vector3(_scale, _scale);
        transform.GetChild(1).GetComponent<SpriteRenderer>().transform.localScale = new Vector3(_scale, _scale);
    }

    public void Update()
    {
        if (!_isAnimating)
        {
            return;
        }
        
        _currentAnimationTime += Time.deltaTime;
        double newScale = Math.Min(_currentAnimationTime/animationTime*_scale,_scale);
        if (!_isAppearingAnimation)
        {
            newScale = _scale - newScale;
        }
        spriteRenderer.transform.localScale = new Vector3((float)newScale, (float)newScale);
        
        // Stop Animation
        if (_currentAnimationTime >= animationTime)
        {
            if (!_isAppearingAnimation)
            {
                spriteRenderer.color = Constants.ColorForColor(Colors.none);
            }
            _isAnimating = false;
        }
    }

    // Set the color of the box using the Color
    public void ChangeColor(Colors boxColor)
    {
        Color mappedColor = Constants.ColorForColor(boxColor);
        // check if should change
        if (spriteRenderer.color.Equals(mappedColor))
        {
            return;
        }
        
        _isAppearingAnimation = boxColor != Colors.none;
        if (_isAppearingAnimation)
        {
            spriteRenderer.color = mappedColor;
        }
        StartAnimating();
    }

    public void Initialize(Colors color)
    {   
        Color mappedColor = Constants.ColorForColor(color);
        _isAppearingAnimation = color != Colors.none;
        spriteRenderer.color = mappedColor;
        StartAnimating();
    }

    private void StartAnimating()
    {
        _isAnimating = true;
        _currentAnimationTime = 0;
    }
}
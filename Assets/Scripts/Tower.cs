using System;
using System.Linq;
using UnityEngine;

public class Tower
{
    private Colors[] _layerColors;
    private const int StackSize = 4;
    
    public Tower(Colors[] colorsArray = null){
        if (colorsArray == null || !IsValid(colorsArray))
        {
            colorsArray= new []{ Colors.red , Colors.blue, Colors.green, Colors.orange};
        }

        _layerColors = colorsArray;
    }
    
    public bool IsFinished()
    {
        Colors color = _layerColors[0];
        return _layerColors.All(t => t == color);
    }
    
    public bool IsEmpty()
    {
        return _layerColors.All(t => t == Colors.none);
    }

    public bool IsFull()
    {
        return _layerColors[0] != Colors.none;
    }
    
    public int FirstColorIndex()
    {
        return Array.FindIndex(_layerColors,colors => colors != Colors.none);
    }

    public Colors GetTopColor()
    {
        int firstIndex = FirstColorIndex();
        if (IsEmpty())
        {
            return Colors.none;
        }

        return _layerColors[firstIndex];
    }
    
    public int GetCountOfTopEmpty()
    {
        int count = 0;
        for (int i = 0; i < _layerColors.Length; i++)
        {
            if (_layerColors[i] != Colors.none)
            {
                return count;
            }
            count++;
        }
        return count;
    }
    
    public int GetCountOfTopColor()
    {
        int firstIndex = FirstColorIndex();
        if (IsEmpty())
        {
            return StackSize;
        }

        Colors firstColor = _layerColors[firstIndex];

        Debug.Log("not empty");
        int count = 1;
        for (int i = firstIndex+1; i < _layerColors.Length; i++)
        {
            if (_layerColors[i] != firstColor)
            {
                return count;
            }

            count++;
        }
        return count;
    }
    
    public bool IsValid(Colors[] colorsArray = null)
    {
        if (colorsArray == null)
        {
            colorsArray = _layerColors;
        }
        if (colorsArray.Length != StackSize)
        {
            return false;
        }

        int firstIndex = Array.FindIndex(colorsArray,colors => colors != Colors.none);
        if (firstIndex == -1)
        {
            return true;
        }
        for (int i = firstIndex+1; i < colorsArray.Length; i++)
        {
            if (colorsArray[i] == Colors.none)
            {
                return false;
            }
        }

        return true;
    }

    public bool SetLayerColors(Colors[] colorArray)
    {
        if (!IsValid(colorArray))
        {
            return false;
        }

        _layerColors = colorArray;
        return true;
    }

    public Colors[] GetLayerColors()
    {
        Colors[] colorsArray = new Colors[StackSize];
        Array.Copy(_layerColors, colorsArray, StackSize);
        return colorsArray;
    }

    public Colors GetLayerColor(int index)
    {
 
        return _layerColors[index];
    }

    public int GetStackSize()
    {
        return StackSize;
    }

    public override string ToString()
    {
        if (_layerColors.Length == 0)
        {
            return "[]";
        }
        string result = "[";
        for (int i=0; i < _layerColors.Length; i++)
        {
            result += _layerColors[i] + ", ";
        }

        result = result.Substring(0, result.Length - 2);
        return result += "]";
    }
}
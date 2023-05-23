using System;
using UnityEngine;

public class GameState
{   
    public int NumberOfTowers = 5;
    public Tower[] Towers;

    public GameState()
    {
        Towers = new Tower[NumberOfTowers];
        Towers[0] = new Tower(new Colors[] { Colors.blue, Colors.red, Colors.red, Colors.yellow });
        Towers[1] = new Tower(new Colors[] { Colors.red, Colors.blue, Colors.blue, Colors.yellow });
        Towers[2] = new Tower(new Colors[] { Colors.yellow, Colors.yellow, Colors.red, Colors.blue });
        Towers[3] = new Tower(new Colors[] { Colors.none, Colors.none, Colors.none, Colors.none });
        Towers[4] = new Tower(new Colors[] { Colors.none, Colors.none, Colors.none, Colors.none });
    }
    
    public bool Move(int from, int to)
    {
        Tower fromTower = Towers[from];
        Tower toTower = Towers[to];
        
        Colors fromColor = fromTower.GetTopColor();
        Colors toColor = toTower.GetTopColor();
        bool differentTopColors = toColor != fromColor && toColor != Colors.none;
        if (toTower.IsFull() || differentTopColors || fromColor == Colors.none || from == to)
        {
            return false;
        }
        int movedAmount = Math.Min(toTower.GetCountOfTopEmpty(), fromTower.GetCountOfTopColor());
        Debug.Log(movedAmount);
        
        Colors[] fromColors = fromTower.GetLayerColors();
        for (int i = fromTower.FirstColorIndex(); i < movedAmount + fromTower.FirstColorIndex(); i++)
        {
            fromColors[i] = Colors.none;
        }
        Towers[from] = new Tower(fromColors);
        
        Colors[] toColors = toTower.GetLayerColors();
        int toTowerInsertStart = toTower.FirstColorIndex() - 1;
        if (toTowerInsertStart == -2)
        {
            toTowerInsertStart = toTower.GetStackSize()-1;
        }
        for (int i = toTowerInsertStart; i >= 0  && i > toTowerInsertStart-movedAmount; i--)
        {
            toColors[i] = fromColor;
        }
        Towers[to] = new Tower(toColors);
        return true;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class GameState
{   
    public delegate void VoidDelegate();
    public int NumberOfTowers = 10;
    public Tower[] Towers;
    public bool IsFinished = false;
    private readonly Stack<Tower[]> _undoStack = new Stack<Tower[]>();
    private readonly Stack<Tower[]> _redoStack = new Stack<Tower[]>();

    public GameState()
    {
        GenerateRandom();
    }
    
    public bool TryMove(int from, int to)
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

        Move(from, to, movedAmount);
        if (GetIsFinished())
        {
            IsFinished = true;
        }
        return true;
    }

    private void Move(int fromIndex, int toIndex, int movedAmount)
    {
        Tower fromTower = Towers[fromIndex];
        Tower toTower = Towers[toIndex];
        Colors fromColor = fromTower.GetTopColor();

        CurrentToUndoStack();
        _redoStack.Clear();
        
        Colors[] fromColors = fromTower.GetLayerColors();
        for (int i = fromTower.GetFirstColorIndex(); i < movedAmount + fromTower.GetFirstColorIndex(); i++)
        {
            fromColors[i] = Colors.none;
        }
        Towers[fromIndex] = new Tower(fromColors);
        
        Colors[] toColors = toTower.GetLayerColors();
        int toTowerInsertStart = toTower.GetFirstColorIndex() - 1;
        if (toTowerInsertStart == -2)
        {
            toTowerInsertStart = toTower.GetStackSize()-1;
        }
        for (int i = toTowerInsertStart; i >= 0  && i > toTowerInsertStart-movedAmount; i--)
        {
            toColors[i] = fromColor;
        }
        Towers[toIndex] = new Tower(toColors);
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            Tower[] towers = _undoStack.Pop();
            _redoStack.Push(Towers);
            Towers = towers;
        }
    }
    
    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            Tower[] towers = _redoStack.Pop();
            _undoStack.Push(Towers);
            Towers = towers;
        }
    }

    private void CurrentToUndoStack()
    {
        Tower[] towerCopy = new Tower[NumberOfTowers];
        Array.Copy(Towers, towerCopy, NumberOfTowers);
        _undoStack.Push(towerCopy);
    }

    public bool GetIsFinished()
    {
        for (int i = 0; i < NumberOfTowers; i++)
        {
            if (!(Towers[i].IsFull() || Towers[i].IsEmpty()))
            {
                return false;
            }
        }

        return true;
    }

    public void GenerateRandom()
    {
        Towers = new Tower[NumberOfTowers];
        // Get all Colors and remove the none Color
        Colors[] usableColors = (Colors[])Enum.GetValues(typeof(Colors));
        usableColors = usableColors.Where(colors => colors != Colors.none).ToArray();
        // Get NumberOfTowers - 2 different Colors
        Random random = new Random();
        List<Colors> usedColorsList = usableColors.OrderBy(_ => random.Next()).Take(NumberOfTowers - 2).ToList();

        for (int i = 0; i < Towers.Length; i++)
        {
            Towers[i] = new Tower( Enumerable.Repeat(i < usedColorsList.Count ? usedColorsList[i] : Colors.none, Tower.StackSize).ToArray());
        }

        Shuffle();
    }

    // Shuffles backwards from sorted to unsorted
    private void Shuffle()
    {
        const int maxAmountOfShuffles = 50;
        for (int i = 0; i < maxAmountOfShuffles; i++)
        {
            int[] possibleTowers = Enumerable.Range(0, Towers.Length).ToArray();
            possibleTowers = possibleTowers.Where(i => Towers[i].GetCountOfTopColor() > 1 && !Towers[i].IsEmpty())
                .ToArray();
            Tower[] towers = Towers;

            // Take from first move to second
            List<(int, int)> possibleSwaps = new List<(int, int)>();

            foreach (var towerIndex in possibleTowers)
            {
                int[] possibleTargets = Enumerable.Range(0, Towers.Length).ToArray();
                possibleTargets = possibleTargets.Where(i => Towers[i].GetCountOfTopEmpty() > 0 && i != towerIndex)
                    .ToArray();
                foreach (var target in possibleTargets)
                {
                    possibleSwaps.Add((towerIndex, target));
                }
            }

            if (possibleSwaps.Count == 0)
            {
                break;
            }

            Random random = new Random();
            var (from, to) = possibleSwaps[random.Next(0, possibleSwaps.Count - 1)];
            int movedAmount = random.Next(1, Math.Min(Towers[from].GetCountOfTopColor(), Towers[to].GetCountOfTopEmpty()));
            Debug.Log(movedAmount);
            Move(from, to, movedAmount);
        }
    }

    public override string ToString()
    {
        if (Towers.Length == 0)
        {
            return "[]";
        }
        string result = "[";
        for (int i=0; i < Towers.Length; i++)
        {
            result += Towers[i] + ", ";
        }

        result = result.Substring(0, result.Length - 2);
        return result += "]";
    }
}
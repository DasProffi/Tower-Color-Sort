using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using util;
using Random = System.Random;

public class GameState
{   
    public delegate void VoidDelegate();
    public int NumberOfTowers = 12;
    public int SpareTowers = 2;
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
        for (int i = fromTower.GetFirstColorIndex(); i < movedAmount + fromTower.GetFirstColorIndex() && i < fromColors.Length; i++)
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
        List<Colors> usedColorsList = usableColors.OrderBy(_ => random.Next()).Take(NumberOfTowers - SpareTowers).ToList();

        for (int i = 0; i < Towers.Length; i++)
        {
            Towers[i] = new Tower( Enumerable.Repeat(i < usedColorsList.Count ? usedColorsList[i] : Colors.none, Tower.StackSize).ToArray());
        }

        Shuffle();
    }

    // Shuffles backwards from sorted to unsorted
    private void Shuffle()
    {
        const int maxAmountOfShuffles = 30;
        int towerHeight = Towers[0].GetStackSize();
        for (int i = 0; i < maxAmountOfShuffles; i++)
        {
            int[] possibleTowers = Enumerable.Range(0, Towers.Length).ToArray();
            possibleTowers = possibleTowers.Where(towerIndex => Towers[towerIndex].GetCountOfTopColor() > 1 && !Towers[towerIndex].IsEmpty())
                .ToArray();

            List<WeightedItem<int>> weighted = possibleTowers.Select(index =>
            {
                WeightedItem<int> item = new WeightedItem<int>
                {
                    Item = index,
                    Weight = (float)Towers[index].GetCountOfTopColor()/Towers[index].GetCountOfDifferentColors()
                };
                return item;
            }).ToList();

            do
            {
                int fromIndex = WeightRandomUtil.GetWeightedRandom(weighted, -1);
                int[] possibleTargets = Enumerable.Range(0, Towers.Length)
                    .Where(index => Towers[index].GetCountOfTopEmpty() > 0 && index != fromIndex)
                    .Where(index => Towers[index].GetCountOfTopColor() != towerHeight-1)
                    .ToArray();
                List<WeightedItem<int>> weightedTargets = possibleTargets.Select(index =>
                {
                    int differentColors = Towers[index].GetCountOfDifferentColors();
                    WeightedItem<int> item = new WeightedItem<int>
                    {
                        Item = index,
                        Weight = (float)Convert.ToSingle(Math.Pow(differentColors == 0 ? towerHeight:differentColors,2))/Towers[index].GetCountOfTopColor()
                    };
                    return item;
                }).ToList();
                
                if (weightedTargets.Count > 0 && fromIndex != -1)
                {
                    int target = WeightRandomUtil.GetWeightedRandom(weightedTargets, -1);
                    int maxMovedAmount = Math.Min(Towers[fromIndex].GetCountOfTopColor(), Towers[target].GetCountOfTopEmpty());
                    int movedAmount = WeightRandomUtil.GetWeightedRandom(
                        Enumerable.Range(1, maxMovedAmount).Select(amount => new WeightedItem<int>
                            {
                                Item = amount,
                                Weight = 1f/Convert.ToSingle(Math.Pow(amount,2))
                            })
                        .ToList(), -1);
                    Move(fromIndex, target, movedAmount);
                    weighted = new List<WeightedItem<int>>();
                }
                else
                {
                    weighted = weighted.Where(item => item.Item != fromIndex).ToList();
                }
            } while (weighted.Count > 0);
        }
        
        // Try to leave SpareTowers empty
        
        // Take from first move to second
        List<(int, int)> possibleSwaps = new List<(int, int)>();
        do
        {
            int[] possibleTowers = Enumerable.Range(0, Towers.Length).ToArray();
            possibleTowers = possibleTowers.Where(i => Towers[i].GetCountOfTopColor() > 1 && !Towers[i].IsEmpty() && !Towers[i].IsFull())
                .ToArray();
            
            possibleSwaps.Clear();
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
            Move(from, to, movedAmount);
        } while (possibleSwaps.Count > 0);
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
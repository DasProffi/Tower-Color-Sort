using System;
using System.Collections.Generic;
using System.Linq;
using util;
using Random = System.Random;

public class GameState
{
    public int NumberOfTowers = 13;
    public int SpareTowers = 2;
    public int NumberOfGenerationTries = 10;
    public Tower[] Towers;
    public bool IsFinished = false;
    private readonly Stack<Tower[]> _undoStack = new Stack<Tower[]>();
    private readonly Stack<Tower[]> _redoStack = new Stack<Tower[]>();
    public Stack<(int, int)> Solution = new Stack<(int, int)>();
    private readonly Random _random;

    public GameState(int seed=0)
    {
        _random = seed == 0 ? new Random() : new Random(seed);
        GenerateRandom();
    }
    
    public bool TryMove(int from, int to, bool addToUndo = true)
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

        if (addToUndo)
        {
            CurrentToUndoStack();
            _redoStack.Clear();
        }
        
        Move(from, to, movedAmount);
        if (GetIsFinished())
        {
            IsFinished = true;
        }
        return true;
    }

    private void Move(int fromIndex, int toIndex, int movedAmount = 1)
    {
        Tower fromTower = Towers[fromIndex];
        Tower toTower = Towers[toIndex];
        Colors fromColor = fromTower.GetTopColor();

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
            toTowerInsertStart = toTower.GetTowerHeight()-1;
        }
        for (int i = toTowerInsertStart; i >= 0  && i > toTowerInsertStart-movedAmount; i--)
        {
            toColors[i] = fromColor;
        }
        Towers[toIndex] = new Tower(toColors);
    }

    public bool HasUndo()
    { 
        return _undoStack.Count > 0;
    }
    
    public bool HasRedo()
    { 
        return _redoStack.Count > 0;
    }
    
    public void Undo()
    {
        if (HasUndo())
        {
            Tower[] towers = _undoStack.Pop();
            _redoStack.Push(Towers);
            Towers = towers;
        }
    }

    public void ClearUndo()
    {
        _undoStack.Clear();
    }
    
    public void Redo()
    {
        if (HasRedo())
        {
            Tower[] towers = _redoStack.Pop();
            _undoStack.Push(Towers);
            Towers = towers;
        }
    }

    private void CurrentToUndoStack()
    {
        _undoStack.Push(GetTowersCopy());
    }

    private Tower[] GetTowersCopy()
    {   
        Tower[] towerCopy = new Tower[NumberOfTowers];
        for (int i = 0; i < Towers.Length; i++)
        {
            towerCopy[i] = new Tower(Towers[i].GetLayerColors());
        }
        return towerCopy;
    }

    private int GradeTowers(Tower[] towerArray)
    {
        int towerHeight = towerArray[0].GetTowerHeight();
        int value = 0;
        foreach (var tower in towerArray)
        {
            if ((tower.GetCountOfDifferentColors() >= 2 && tower.GetCountOfColored() == towerHeight) || tower.IsEmpty())
            {
                value += 10;
            }
            else
            {
                value -= 50;
            }
            if (tower.GetCountOfDifferentColors() >= 3)
            {
                value += 2;
            }
            if (tower.GetCountOfDifferentColors() >= 3)
            {
                value += 5;
            }
        }
        return value;
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
        List<Colors> usedColorsList = usableColors.OrderBy(_ => _random.Next()).Take(NumberOfTowers - SpareTowers).ToList();
        

        Dictionary<(Tower[],Stack<(int,int)>), int> scores = new Dictionary<(Tower[],Stack<(int,int)>), int>();
        for (int generationTry = 0; generationTry < NumberOfGenerationTries; generationTry++)
        {   
            for (int i = 0; i < Towers.Length; i++)
            {
                Towers[i] = new Tower( Enumerable.Repeat(i < usedColorsList.Count ? usedColorsList[i] : Colors.none, Tower.StackSize).ToArray());
            }
            Solution = new Stack<(int, int)>();
            Shuffle();
            scores.Add((GetTowersCopy(),Solution),GradeTowers(Towers));
        }

        (Towers,Solution) = scores.FirstOrDefault(kv => kv.Value == scores.Values.Max()).Key;
        
        
        // Move Empty towers to the right
        for (int i = 0; i < SpareTowers; i++)
        {
            int spareTowerIndex = Array.FindIndex(Towers, tower => tower.IsEmpty());
            if (spareTowerIndex == -1) break;
            
            (Towers[spareTowerIndex], Towers[NumberOfTowers - i - 1]) = (Towers[NumberOfTowers - i - 1], Towers[spareTowerIndex]);
            int index = i;
            Solution = new Stack<(int, int)>(Solution.Select(move =>
            {
                int from = move.Item1;
                if (from == spareTowerIndex)
                {
                    from = NumberOfTowers - index - 1;
                }else if (from == NumberOfTowers - index - 1)
                {
                    from = spareTowerIndex;
                }
                int to = move.Item2;
                if (to == spareTowerIndex)
                {
                    to = NumberOfTowers - index - 1;
                }else if (to == NumberOfTowers - index - 1)
                {
                    to = spareTowerIndex;
                }

                return (from, to);
            }).Reverse());
        }
    }

    // Shuffles backwards from sorted to unsorted
    private void Shuffle()
    {
        const int maxAmountOfShuffles = 30;

        for (int i = 0; i < maxAmountOfShuffles; i++)
        {
            List<WeightedItem<int>> weightedFrom = GetWeightedFromTowers();
            if (weightedFrom.Count == 0) break;
            do {
                int fromIndex = WeightRandomUtil.GetWeightedRandom(weightedFrom, -1, _random);
                if (fromIndex == -1) break;
                
                int targetIndex = GetTargetTower(fromIndex);
                // Check if target is found
                if (targetIndex != -1)
                {
                    int movedAmount = GetMovedAmount(i, fromIndex, targetIndex);
                    Move(fromIndex, targetIndex, movedAmount);
                    Solution.Push((fromIndex,targetIndex));
                    break;
                }
                weightedFrom = weightedFrom.Where(item => item.Item != fromIndex).ToList();
            } while (weightedFrom.Count > 0);
        }
    }

    private List<WeightedItem<int>> GetWeightedFromTowers()
    {
        int[] fromTowers = Enumerable.Range(0, Towers.Length).ToArray();
        fromTowers = fromTowers
            .Where(towerIndex => Towers[towerIndex].CanTakeAwayTopColor())
            .ToArray();
        List<WeightedItem<int>> weightedFrom = fromTowers.Select(index =>
        {
            WeightedItem<int> item = new WeightedItem<int>
            {
                Item = index,
                Weight = (float)Towers[index].GetCountOfTopColor()/Towers[index].GetCountOfDifferentColors()
            };
            return item;
        }).ToList();
        return weightedFrom;
    }

    private int GetTargetTower(int fromIndex)
    {
        int towerHeight = Towers[0].GetTowerHeight();
        int[] possibleTargets = Enumerable.Range(0, NumberOfTowers)
            .Where(index => Towers[index].GetCountOfTopEmpty() > 0 && index != fromIndex)
            .Where(index => Towers[index].GetCountOfTopColor() != towerHeight-1)
            .Where(index => Towers[index].GetTopColor() != Towers[fromIndex].GetTopColor())
            .ToArray();
        List<WeightedItem<int>> weightedTargets = possibleTargets.Select(index =>
        {
            int differentColors = Towers[index].GetCountOfDifferentColors();
            WeightedItem<int> item = new WeightedItem<int>
            {
                Item = index,
                Weight = (float)Convert.ToSingle(Math.Pow(differentColors == 0 ? towerHeight:differentColors,2))/
                         (Towers[index].GetCountOfTopColor() * Towers[index].GetCountOfTopColor())
            };
            return item;
        }).ToList();
        return  WeightRandomUtil.GetWeightedRandom(weightedTargets, -1, _random);;
    }

    private int GetMovedAmount(int iterationStep,int fromIndex, int targetIndex)
    {
        int maxNumberOfMoves = Towers[0].GetTowerHeight() - 1;
        maxNumberOfMoves = Math.Min(maxNumberOfMoves, Towers[targetIndex].GetCountOfTopEmpty());
        int fromCountOfTopColor = Towers[fromIndex].GetCountOfTopColor();
        if (Towers[fromIndex].IsFinished()) fromCountOfTopColor -= 1;
        maxNumberOfMoves = Math.Min(maxNumberOfMoves, fromCountOfTopColor);
        
        List<WeightedItem<int>> weightedMoveAmount = Enumerable.Range(1,maxNumberOfMoves)
            .Select(amount => new WeightedItem<int> { Item = amount, Weight = iterationStep < 5 ? amount : (float)(1/Math.Pow(amount,2)) }).ToList();
        return WeightRandomUtil.GetWeightedRandom(weightedMoveAmount, 1, _random);
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
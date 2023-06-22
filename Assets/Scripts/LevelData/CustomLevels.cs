using System.Collections.Generic;

namespace LevelData
{
       public class CustomLevels
       {      
              // Level is the key, Seed is the value
              // Level : Seed
              private static readonly Dictionary<int, int> LevelMapping = new Dictionary<int, int>()
              {
                     { 0,0 },
              };
       
              public static int GetSeed(int level, int fallBack)
              {
                     return LevelMapping.TryGetValue(level, out var seed) ? seed : fallBack;
              }
       }
}

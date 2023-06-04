using System;
using System.Collections.Generic;
using System.Linq;

namespace util
{   
    public class WeightedItem<T>
    {
        public T Item { get; set; }
        public float Weight { get; set; }
    }
    
    public class WeightRandomUtil
    {
        public static T GetWeightedRandom<T>(List<WeightedItem<T>> weightedItems, T defaultValue, Random random = null)
        {
            // Calculate the total weight sum
            double totalWeight = weightedItems.Sum(item => item.Weight);

            // Generate a random value between 0 and the total weight sum
            if (random == null) random = new Random();
            double randomValue = random.NextDouble()*totalWeight;

            // Iterate over the weighted items and find the selected item
            double weightSum = 0f;
            foreach (var weightedItem in weightedItems)
            {
                weightSum += weightedItem.Weight;

                // Check if the random value falls within the current weight range
                if (randomValue <= weightSum)
                {
                    return weightedItem.Item; // Return the selected item
                }
            }
        
            return defaultValue;
        }   
    }
}

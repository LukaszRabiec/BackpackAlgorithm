using System;
using System.Collections.Generic;
using System.Linq;

namespace BackpackAlgorithm
{
    public class Backpack
    {
        public int TotalCapacity { get; set; }
        public int RemainingCapacity { get; private set; }

        public List<Item> PackedItems { get; set; }

        private List<Item> _itemsToPack;

        public Backpack(int totalCapacity)
        {
            TotalCapacity = totalCapacity;
            RemainingCapacity = totalCapacity;
            PackedItems = new List<Item>();
        }

        /// <summary>
        /// Packs specified items into a backpack in best combination so that the backpack's value was the biggest.
        /// </summary>
        /// <param name="items">Items to pack.</param>
        /// <param name="knapFactor">The number of items considered (permuted) before greedy adding.</param>
        /// <returns>Packed items with the highest possible value</returns>
        public List<Item> TryToPack(List<Item> items, int knapFactor)
        {
            ValidateItemsWeights(items);
            ValidateKnapFactor(knapFactor, items.Count);
            _itemsToPack = SortItemsByRate(items);
            var bestSet = new Set(_itemsToPack.Count);

            for (int knap = 1; knap <= knapFactor; knap++)
            {
                var knapSubset = PrepareSubsetForKnap(knap);
                var permutations = FindPermutationsOfSubset(knapSubset);
                bestSet = FindBestSet(bestSet, permutations);
            }

            AddSetToPackedItems(bestSet);
            CalculateRemainingCapacity(bestSet);

            return PackedItems;
        }

        private void ValidateItemsWeights(List<Item> items)
        {
            int itemsCount = items.Count;
            int itemsNotToPackCounter = 0;

            foreach (var item in items)
            {
                if (item.Weight > RemainingCapacity)
                {
                    itemsNotToPackCounter++;
                }
            }

            if (itemsNotToPackCounter != 0 && itemsCount / itemsNotToPackCounter == 1)
            {
                throw new ArgumentException("All specified items have weight greater than remaining backpack's capacity.");
            }
        }

        private void ValidateKnapFactor(int knapFactor, int itemsCount)
        {
            if (knapFactor > itemsCount)
            {
                throw new ArgumentException("Knap factor must be less then number of items to pack.");
            }
            if (knapFactor <= 0)
            {
                throw new ArgumentException("Knap factor must be greater then 0.");
            }
        }

        private List<Item> SortItemsByRate(List<Item> items)
        {
            return items.OrderByDescending(item => item.Rate).ToList();
        }

        private List<int> PrepareSubsetForKnap(int knap)
        {
            var knapSet = new List<int>();

            for (int itemId = 0; itemId < _itemsToPack.Count; itemId++)
            {
                knapSet.Add(itemId < knap ? 1 : 0);
            }

            return knapSet;
        }

        private List<int[]> FindPermutationsOfSubset(List<int> set)
        {
            var permutations = new List<int[]>();
            Permute(0, permutations, set);
            return permutations;
        }

        private void Permute(int startingIndex, List<int[]> permutations, List<int> set)
        {
            if (startingIndex == set.Count)
            {
                int weightsSum = SumTakenItemsWeights(set);

                if (weightsSum <= RemainingCapacity)
                {
                    permutations.Add(set.ToArray());
                }
            }
            else
            {
                var swaps = new List<int>();

                for (int index = startingIndex; index < set.Count; index++)
                {
                    if (swaps.Contains(set[index]))
                    {
                        continue;
                    }

                    swaps.Add(set[index]);

                    Swap(set, startingIndex, index);
                    Permute(startingIndex + 1, permutations, set);
                    Swap(set, startingIndex, index);
                }
            }
        }

        private int SumTakenItemsWeights(List<int> takenItemsCollection)
        {
            int sum = 0;

            for (int itemId = 0; itemId < _itemsToPack.Count; itemId++)
            {
                if (takenItemsCollection[itemId] == 1)
                {
                    sum += _itemsToPack[itemId].Weight;
                }
            }

            return sum;
        }

        private int SumTakenItemsValues(List<int> takenItemsCollection)
        {
            int sum = 0;

            for (int itemId = 0; itemId < _itemsToPack.Count; itemId++)
            {
                if (takenItemsCollection[itemId] == 1)
                {
                    sum += _itemsToPack[itemId].Value;
                }
            }

            return sum;
        }

        private void Swap(List<int> set, int firstIndex, int secondIndex)
        {
            var tmp = set[firstIndex];
            set[firstIndex] = set[secondIndex];
            set[secondIndex] = tmp;
        }

        private Set FindBestSet(Set actualBestSet, List<int[]> permutations)
        {
            var bestSet = actualBestSet;

            foreach (var permutation in permutations)
            {
                var setWithGreedilyAddedItems = MakeSetWithGreedilyAddedItems(permutation);
                bestSet = CompareAndTakeWithHighestValue(bestSet, setWithGreedilyAddedItems);
            }

            return bestSet;
        }

        private Set MakeSetWithGreedilyAddedItems(int[] permutation)
        {
            var set = CopyTakenItemsIds(permutation);
            var setWithGreedilyAddedItems = AddGreedilyItems(set);

            return setWithGreedilyAddedItems;
        }

        private Set CopyTakenItemsIds(int[] permutation)
        {
            var set = new Set(_itemsToPack.Count);

            for (int itemId = 0; itemId < set.TakenItemsIds.Length; itemId++)
            {
                if (permutation[itemId] == 1)
                {
                    set.TakenItemsIds[itemId] = 1;
                }
                else
                {
                    set.TakenItemsIds[itemId] = 0;
                }
            }

            return set;
        }

        private Set AddGreedilyItems(Set set)
        {
            var takenItemsValue = SumTakenItemsValues(set.TakenItemsIds.ToList());
            var takenItemsWeight = SumTakenItemsWeights(set.TakenItemsIds.ToList());
            var freeSpace = RemainingCapacity - takenItemsWeight;

            for (int itemId = 0; itemId < set.TakenItemsIds.Length; itemId++)
            {
                if (set.TakenItemsIds[itemId] == 0 && _itemsToPack[itemId].Weight <= freeSpace)
                {
                    set.TakenItemsIds[itemId] = 1;
                    freeSpace -= _itemsToPack[itemId].Weight;
                    takenItemsValue += _itemsToPack[itemId].Value;
                }
            }

            set.MaxValue = takenItemsValue;

            return set;
        }

        private Set CompareAndTakeWithHighestValue(Set actualBestSet, Set greedySet)
        {
            return actualBestSet.MaxValue > greedySet.MaxValue ? actualBestSet : greedySet;
        }

        private void AddSetToPackedItems(Set set)
        {
            for (int itemId = 0; itemId < set.TakenItemsIds.Length; itemId++)
            {
                if (set.TakenItemsIds[itemId] == 1)
                {
                    PackedItems.Add(_itemsToPack[itemId]);
                }
            }
        }

        private void CalculateRemainingCapacity(Set set)
        {
            for (int itemId = 0; itemId < set.TakenItemsIds.Length; itemId++)
            {
                if (set.TakenItemsIds[itemId] == 1)
                {
                    RemainingCapacity -= _itemsToPack[itemId].Weight;
                }
            }
        }
    }

    internal struct Set
    {
        public int[] TakenItemsIds;
        public int MaxValue;

        public Set(int setLength)
        {
            TakenItemsIds = new int[setLength];
            MaxValue = 0;
        }
    }
}

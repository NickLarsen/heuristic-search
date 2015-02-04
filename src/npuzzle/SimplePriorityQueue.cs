using System.Collections.Generic;
using System.Linq;

namespace npuzzle
{
    class SimplePriorityQueue<T>
    {
        private readonly SortedDictionary<uint, long> costCounts = new SortedDictionary<uint, long>(); 
        private readonly Dictionary<T, ulong> values = new Dictionary<T, ulong>();

        public int Count
        {
            get { return values.Count; }
        }

        public void Push(T item, uint g, uint h)
        {
            IncrementCostCount(g + h);
            values.Add(item, GetKey(g, h));
        }

        public T Pop()
        {
            var item = values.OrderBy(kvp => kvp.Value).First();
            values.Remove(item.Key);
            DecrementCostCount((uint)(item.Value >> 32));
            return item.Key;
        }

        public bool Contains(T item)
        {
            return values.ContainsKey(item);
        }

        public void DecreaseKey(T item, uint g, uint h)
        {
            DecrementCostCount((uint)(values[item] >> 32));
            values[item] = GetKey(g, h);
            IncrementCostCount(g + h);
        }

        public uint GetMinCost()
        {
            return costCounts.First().Key;
        }

        private static ulong GetKey(uint g, uint h)
        {
            return ((ulong)(g + h) << 32) + (uint.MaxValue - g); // makes single value sorting easier
        }

        private void IncrementCostCount(uint cost)
        {
            if (costCounts.ContainsKey(cost))
            {
                costCounts[cost] += 1;
            }
            else
            {
                costCounts.Add(cost, 1);
            }
        }

        private void DecrementCostCount(uint cost)
        {
            if (costCounts[cost] == 1)
            {
                costCounts.Remove(cost);
            }
            else
            {
                costCounts[cost] -= 1;
            }
        }
    }
}

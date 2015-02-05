using System.Collections.Generic;

namespace npuzzle
{
    class SimplePriorityQueue<T>
    {
        private const int minSize = 1048576;
        private readonly Dictionary<T, int> index = new Dictionary<T, int>();
        private KeyValuePair<ulong, T>[] values = new KeyValuePair<ulong, T>[minSize];
        private int size = 0;

        public int Count
        {
            get { return size; }
        }

        public void Push(T item, uint g, uint h)
        {
            size += 1;
            int i = size;
            if (i > values.Length) Expand();
            index[item] = i;
            values[i] = new KeyValuePair<ulong, T>(GetKey(g, h), item);
            Float(i);
        }

        public T Pop()
        {
            var item = values[1];
            SwapIndexes(1, size);
            index.Remove(item.Value);
            values[size] = new KeyValuePair<ulong, T>();
            size -= 1;
            Sink(1);
            if (size >= (minSize / 2) && size <= (values.Length / 4)) Contract();
            return item.Value;
        }

        public bool Contains(T item)
        {
            return index.ContainsKey(item);
        }

        public void DecreaseKey(T item, uint g, uint h)
        {
            var i = index[item];
            values[i] = new KeyValuePair<ulong, T>(GetKey(g, h), item);
            Float(i);
        }

        public uint GetMinCost()
        {
            return (uint)(values[1].Key >> 32);
        }

        private void Float(int i)
        {
            while (i > 1 && values[i].Key < values[i/2].Key)
            {
                SwapIndexes(i, i / 2);
                i /= 2;
            }
        }

        private void Sink(int i)
        {
            while (i * 2 <= size)
            {
                int j = 2 * i;
                if (j < size && values[j+1].Key < values[j].Key) j += 1;
                if (values[i].Key <= values[j].Key) break;
                SwapIndexes(i, j);
                i = j;
            }
        }

        private void Expand()
        {
            var tempValues = new KeyValuePair<ulong, T>[values.Length * 2];
            for (int i = 0; i < values.Length; i += 1)
            {
                tempValues[i] = values[i];
            }
            values = tempValues;
        }

        private void Contract()
        {
            var tempValues = new KeyValuePair<ulong, T>[values.Length / 2];
            for (int i = 0; i <= size; i += 1)
            {
                tempValues[i] = values[i];
            }
            values = tempValues;
        }

        private void SwapIndexes(int a, int b)
        {
            index[values[a].Value] = b;
            index[values[b].Value] = a;

            var v = values[a];
            values[a] = values[b];
            values[b] = v;
        }

        private static ulong GetKey(uint g, uint h)
        {
            return ((ulong)(g + h) << 32) + (uint.MaxValue - g); // makes single value sorting easier
        }
    }
}

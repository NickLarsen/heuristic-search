using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace npuzzle
{
    class PatternDatabase
    {
        private readonly byte[] values;
        private readonly byte[] pattern;
        private readonly int offset;

        public PatternDatabase(string filename)
            : this(File.ReadAllBytes(filename))
        {
        }

        private PatternDatabase(byte[] pdb)
        {
            values = pdb;
            offset = values[0] + 1;
            pattern = new byte[offset - 1];
            for (int i = 1; i <= pattern.Length; i += 1)
            {
                pattern[i - 1] = values[i];
            }
        }

        public void Save(string filename)
        {
            File.WriteAllBytes(filename, values);
        }

        public uint Evaluate(byte[] state)
        {
            int key = GetPatternKey(state, pattern);
            uint value = values[key + offset];
            return value;
        }

        public static PatternDatabase Create(int rows, int cols, byte[] pattern, byte[] goal, Action<CreateStats> update, Func<byte[],List<byte[]>> expand)
        {
            byte[] pdb = InitializePdb(rows, cols, pattern);
            int offset = pdb[0] + 1;
            var openList = CreateOpenList(256); // only works because incremental step cost
            openList[0].AddFirst(goal);
            var createStats = new CreateStats()
            {
                CurrentDepth = 0,
                StatesCalculated = 0,
                TotalStates = pdb.Length - offset
            };
            for (; createStats.CurrentDepth < openList.Length - 1; createStats.CurrentDepth += 1)
            {
                var nodes = openList[createStats.CurrentDepth];
                if (nodes.Count == 0) break;
                var next = openList[createStats.CurrentDepth + 1];
                byte nodeCost = (byte)createStats.CurrentDepth;
                while (nodes.Count > 0)
                {
                    update(createStats);
                    var state = nodes.First.Value;
                    nodes.RemoveFirst();
                    var key = GetPatternKey(state, pattern) + offset;
                    if (pdb[key] != byte.MaxValue) continue;
                    pdb[key] = nodeCost;
                    createStats.StatesCalculated += 1;
                    var successors = expand(state);
                    foreach (var successor in successors)
                    {
                        next.AddLast(successor);
                    }
                }
            }
            createStats.IsFinished = true;
            update(createStats);
            return new PatternDatabase(pdb);
        }

        static byte[] InitializePdb(int rows, int cols, byte[] pattern)
        {
            var size = 1;
            for (int i = 0; i < pattern.Length; i += 1)
            {
                size *= rows * cols - i;
            }
            int offset = pattern.Length + 1;
            byte[] pdb = new byte[offset + size];
            pdb[0] = (byte)pattern.Length;
            for (int i = 0; i < pattern.Length; i += 1)
            {
                pdb[i + 1] = pattern[i];
            }
            for (int i = offset; i < pdb.Length; i += 1)
            {
                pdb[i] = byte.MaxValue;
            }
            return pdb;
        }

        static LinkedList<byte[]>[] CreateOpenList(int maxValue)
        {
            var openList = new LinkedList<byte[]>[maxValue];
            for (int i = 0; i < maxValue; i += 1)
            {
                openList[i] = new LinkedList<byte[]>();
            }
            return openList;
        }

        static int GetPatternKey(byte[] state, byte[] pattern)
        {
            int hash = 0;
            int domain = 1;
            byte[] s = state.ToArray();
            for (int i = 0; i < pattern.Length; i += 1)
            {
                byte p = pattern[i];
                for (int j = i; j < s.Length; j += 1)
                {
                    if (p == s[j])
                    {
                        hash += domain * (j - i);
                        domain *= s.Length - i;
                        s.Swap(i, j);
                        break;
                    }
                }
            }
            return hash;
        }

        public class CreateStats
        {
            public int TotalStates { get; set; }
            public int CurrentDepth { get; set; }
            public int StatesCalculated { get; set; }
            public bool IsFinished { get; set; }
        }
    }
}

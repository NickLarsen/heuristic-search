using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace npuzzle
{
    class PatternDatabase
    {
        private readonly byte[] values;
        private readonly byte[] pattern;
        private readonly uint[] patternDomains;
        private readonly uint[] cValues;
        private readonly uint offset;

        public byte[] Pattern
        {
            get { return pattern; }
        }

        public PatternDatabase(string filename)
            : this(File.ReadAllBytes(filename))
        {
        }

        private PatternDatabase(byte[] pdb)
        {
            values = pdb;
            uint rows = pdb[0];
            uint cols = pdb[1];
            offset = values[2] + 3u;
            uint numElements = rows * cols;
            pattern = new byte[offset - 3];
            for (int i = 0; i < pattern.Length; i += 1)
            {
                pattern[i] = values[i + 3];
            }
            patternDomains = CreatePatternDomains(pattern, numElements);
            cValues = GenerateCValues(numElements);
        }

        public void Save(string filename)
        {
            File.WriteAllBytes(filename, values);
        }

        public uint Evaluate(byte[] state)
        {
            uint key = GetPatternKey(state, patternDomains, cValues);
            uint value = values[key + offset];
            return value;
        }

        public Dictionary<byte, int> GetValueCounts()
        {
            return values.Skip((int)offset).GroupBy(m => m).ToDictionary(m => m.Key, m => m.Count());
        }

        public static PatternDatabase Create(uint rows, uint cols, byte[] pattern, byte[] goal, Action<CreateStats> update, Func<uint, uint, byte[], List<Tuple<byte[], bool>>> expand)
        {
            byte[] createPattern = pattern.Any(m => m == 0) ? pattern : new byte[] { 0 }.Concat(pattern).ToArray();
            var visitedTrackerLow = new BitArray(new byte[int.MaxValue >> 3]); // TODO: make this less of a hack and come up with something appropriately sized and that can handle larger values
            var visitedTrackerHigh = new BitArray(new byte[int.MaxValue >> 3]); // TODO: make this less of a hack and come up with something appropriately sized and that can handle larger values
            byte[] pdb = InitializePdb(rows, cols, pattern);
            uint offset = pdb[2] + 3u;
            var openList = CreateOpenList(256); // only works because incremental step cost
            byte[] patternGoal = goal.Select(m => createPattern.Contains(m) ? m : byte.MaxValue).ToArray();
            openList[0].AddFirst(patternGoal);
            uint numElements = rows * cols;
            var createStats = new CreateStats()
            {
                CurrentDepth = 0,
                StatesCalculated = 0,
                TotalStates = GetCreateSize(createPattern, numElements),
            };
            uint[] createPatternDomain = CreatePatternDomains(createPattern, numElements);
            uint[] patternDomain = CreatePatternDomains(pattern, numElements);
            uint[] cValues = GenerateCValues(numElements);
            uint initialGoalKey = GetPatternKey(goal, createPatternDomain, cValues);
            int initialVisitedKey = initialGoalKey < visitedTrackerLow.Count ? (int)initialGoalKey : (int)(initialGoalKey - visitedTrackerLow.Count);
            visitedTrackerLow.Set(initialVisitedKey, true);
            uint goalKey = GetPatternKey(goal, patternDomain, cValues) + offset;
            pdb[goalKey] = 0;
            createStats.StatesCalculated += 1;
            for (; createStats.CurrentDepth < openList.Length - 1; createStats.CurrentDepth += 1)
            {
                var nodes = openList[createStats.CurrentDepth];
                if (nodes.Count == 0) break;
                var next = openList[createStats.CurrentDepth + 1];
                byte nodeCost = (byte)(createStats.CurrentDepth + 1);
                while (nodes.Count > 0)
                {
                    update(createStats);
                    var state = nodes.First.Value;
                    nodes.RemoveFirst();
                    var successors = expand(rows, cols, state);
                    foreach (var successor in successors)
                    {
                        uint key = GetPatternKey(successor.Item1, createPatternDomain, cValues);
                        if (key < visitedTrackerLow.Count)
                        {
                            int keylow = (int)key;
                            if (visitedTrackerLow.Get(keylow)) continue;
                            visitedTrackerLow.Set(keylow, true);
                        }
                        else
                        {
                            int keyhigh = (int)(key - visitedTrackerLow.Count);
                            if (visitedTrackerHigh.Get(keyhigh)) continue;
                            visitedTrackerHigh.Set(keyhigh, true);
                        }
                        uint finalKey = GetPatternKey(successor.Item1, patternDomain, cValues) + offset;
                        if (pdb[finalKey] == byte.MaxValue) pdb[finalKey] = nodeCost;
                        createStats.StatesCalculated += 1;
                        if (successor.Item2)
                        {
                            nodes.AddLast(successor.Item1);
                        }
                        else
                        {
                            next.AddLast(successor.Item1);
                        }
                    }
                }
            }
            createStats.IsFinished = true;
            update(createStats);
            return new PatternDatabase(pdb);
        }

        static uint[] CreatePatternDomains(byte[] pattern, uint cellCount)
        {
            uint[] patternDomains = new uint[cellCount];
            uint domain = 1;
            for (uint i = 0; i < pattern.Length; i += 1)
            {
                patternDomains[pattern[i]] = domain;
                domain *= cellCount - i;
            }
            return patternDomains;
        }

        static uint[] GenerateCValues(uint numElements)
        {
            var cValues = new uint[1 << ((int)numElements - 1)];
            for (uint i = 0; i < cValues.Length; i += 1)
            {
                cValues[i] = BitCount(i);
            }
            return cValues;
        }

        static uint BitCount(uint i)
        {
            uint ones = 0;
            while (i > 0)
            {
                ones += 1 & i;
                i = i >> 1;
            }
            return ones;
        }

        static uint GetCreateSize(byte[] pattern, uint cells)
        {
            uint size = 1;
            for (uint i = 0; i < pattern.Length; i += 1)
            {
                size *= cells - i;
            }
            return size;
        }

        static byte[] InitializePdb(uint rows, uint cols, byte[] pattern)
        {
            uint cells = rows * cols;
            uint size = 1;
            for (uint i = 0; i < pattern.Length; i += 1)
            {
                size *= cells - i;
            }
            int offset = pattern.Length + 3;
            byte[] pdb = new byte[offset + size];
            pdb[0] = (byte)rows;
            pdb[1] = (byte)cols;
            pdb[2] = (byte)pattern.Length;
            for (int i = 0; i < pattern.Length; i += 1)
            {
                pdb[i + 3] = pattern[i];
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

        static uint GetPatternKey(byte[] state, uint[] patternDomains, uint[] cValues)
        {
            uint hash = 0;
            uint seen = 0u;
            uint mask = uint.MaxValue >> (32 - state.Length);
            for (uint i = 0; i < state.Length; i += 1)
            {
                var value = state[i];
                if (value == byte.MaxValue) continue;
                var domain = patternDomains[value];
                if (domain > 0)
                {
                    uint seenLessThanValue = seen & (mask >> state.Length - value);
                    hash += domain * (i - cValues[seenLessThanValue]);
                    seen |= 1u << value;
                }
            }
            return hash;
        }

        public class CreateStats
        {
            public uint TotalStates { get; set; }
            public int CurrentDepth { get; set; }
            public uint StatesCalculated { get; set; }
            public bool IsFinished { get; set; }
        }
    }
}

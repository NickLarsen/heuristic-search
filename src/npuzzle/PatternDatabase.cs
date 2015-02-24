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
        private readonly int[] patternDomains;
        private readonly int[] cValues;
        private readonly int offset;

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
            int rows = pdb[0];
            int cols = pdb[1];
            offset = values[2] + 3;
            int numElements = rows * cols;
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
            int key = GetPatternKey(state, patternDomains, cValues);
            uint value = values[key + offset];
            return value;
        }

        public static PatternDatabase Create(int rows, int cols, byte[] pattern, byte[] goal, Action<CreateStats> update, Func<byte[],List<byte[]>> expand)
        {
            byte[] pdb = InitializePdb(rows, cols, pattern);
            int offset = pdb[2] + 3;
            var openList = CreateOpenList(256); // only works because incremental step cost
            openList[0].AddFirst(goal);
            var createStats = new CreateStats()
            {
                CurrentDepth = 0,
                StatesCalculated = 0,
                TotalStates = pdb.Length - offset
            };
            int numElements = rows * cols;
            int[] patternDomain = CreatePatternDomains(pattern, numElements);
            int[] cValues = GenerateCValues(numElements);
            var goalKey = GetPatternKey(goal, patternDomain, cValues) + offset;
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
                    var successors = expand(state);
                    foreach (var successor in successors)
                    {
                        var key = GetPatternKey(successor, patternDomain, cValues) + offset;
                        if (pdb[key] != byte.MaxValue) continue;
                        pdb[key] = nodeCost;
                        createStats.StatesCalculated += 1;
                        next.AddLast(successor);
                    }
                }
            }
            createStats.IsFinished = true;
            update(createStats);
            return new PatternDatabase(pdb);
        }

        static int[] CreatePatternDomains(byte[] pattern, int cellCount)
        {
            int[] patternDomains = new int[cellCount];
            int domain = 1;
            for (int i = 0; i < pattern.Length; i += 1)
            {
                patternDomains[pattern[i]] = domain;
                domain *= cellCount - i;
            }
            return patternDomains;
        }

        static int[] GenerateCValues(int numElements)
        {
            var cValues = new int[1 << (numElements - 1)];
            for (uint i = 0; i < cValues.Length; i += 1)
            {
                cValues[i] = BitCount(i);
            }
            return cValues;
        }

        static int BitCount(uint i)
        {
            int ones = 0;
            while (i > 0)
            {
                ones += (int)(1 & i);
                i = i >> 1;
            }
            return ones;
        }

        static byte[] InitializePdb(int rows, int cols, byte[] pattern)
        {
            var size = 1;
            for (int i = 0; i < pattern.Length; i += 1)
            {
                size *= rows * cols - i;
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

        static int GetPatternKey(byte[] state, int[] patternDomains, int[] cValues)
        {
            int hash = 0;
            uint seen = 0u;
            uint mask = uint.MaxValue >> (32 - state.Length);
            for (int i = 0; i < state.Length; i += 1)
            {
                var value = state[i];
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
            public int TotalStates { get; set; }
            public int CurrentDepth { get; set; }
            public int StatesCalculated { get; set; }
            public bool IsFinished { get; set; }
        }

        public static PatternDatabase ProcessSymmetries(PatternDatabase pdb, Symmetry[] symmetries, Action<ProcessSymmetryStats> update)
        {
            var symmetryValues = new byte[pdb.values.Length];
            for (int i = 0; i < pdb.offset; i += 1)
            {
                symmetryValues[i] = pdb.values[i];
            }
            int numElements = pdb.values[0] * pdb.values[1];
            byte[] state = new byte[numElements];
            byte[] transformed = new byte[numElements];
            valuesNotInPattern = Enumerable.Range(0, state.Length).Select(m => (byte)m).Except(pdb.pattern).ToArray();
            scratchPad = new int[pdb.pattern.Length];
            var processStats = new ProcessSymmetryStats()
            {
                IsFinished = false,
                TotalStates = symmetryValues.Length - pdb.offset,
                StatesCalculated = 0,
            };
            for (int i = pdb.offset; i < symmetryValues.Length; i += 1)
            {
                update(processStats);
                CalculateStateFromHash(state, i - pdb.offset, pdb);
                uint best = uint.MinValue;
                foreach (var symmetry in symmetries)
                {
                    symmetry.ComposeSymmetry(state, transformed);
                    var h = pdb.Evaluate(transformed);
                    var hPenalty = symmetry.Penalty > h ? 0 : h - symmetry.Penalty;
                    best = hPenalty > best ? hPenalty : best;
                }
                symmetryValues[i] = (byte)best;
                processStats.StatesCalculated += 1;
            }
            processStats.IsFinished = true;
            update(processStats);
            var symmetryPdb = new PatternDatabase(symmetryValues);
            return symmetryPdb;
        }

        private static byte[] valuesNotInPattern;
        private static int[] scratchPad;
        static void CalculateStateFromHash(byte[] state, int hash, PatternDatabase pdb)
        {
            for (int i = 0; i < state.Length; i += 1)
            {
                state[i] = byte.MaxValue;
                if (i >= scratchPad.Length) continue;
                scratchPad[i] = int.MaxValue;
            }
            for (int i = pdb.pattern.Length - 1; i >= 0; i -= 1)
            {
                int domain = pdb.patternDomains[pdb.pattern[i]];
                if (domain == 0) continue;
                scratchPad[i] = hash / domain;
                hash %= domain;
            }
            for (int i = 0; i < scratchPad.Length; i += 1)
            {
                int place = scratchPad[i];
                int unused = 0;
                for (int j = 0; j < state.Length; j += 1)
                {
                    if (state[j] != byte.MaxValue) continue;
                    if (unused == place)
                    {
                        state[j] = pdb.pattern[i];
                        break;
                    }
                    unused += 1;
                }
            }
            int notInValuesIndex = 0;
            for (int i = 0; i < state.Length; i += 1)
            {
                if (state[i] == byte.MaxValue)
                {
                    state[i] = valuesNotInPattern[notInValuesIndex];
                    notInValuesIndex += 1;
                }
            }
        }

        public class ProcessSymmetryStats
        {
            public int TotalStates { get; set; }
            public int StatesCalculated { get; set; }
            public bool IsFinished { get; set; }
        }
    }
}

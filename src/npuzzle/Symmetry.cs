using System;
using System.Collections.Generic;
using System.Linq;

namespace npuzzle
{
    class Symmetry
    {
        public string Name { get; private set; }
        public byte[] SymmetryMap { get; private set; }
        public byte[] PathMap { get; private set; }
        public uint Penalty { get; private set; }

        public Symmetry(string name, byte[] symmetryMap, byte[] pathMap, uint penalty)
        {
            Name = name;
            SymmetryMap = symmetryMap;
            PathMap = pathMap;
            Penalty = penalty;
        }

        public void ComposeSymmetry(byte[] state, byte[] symmetry)
        {
            for (int i = 0; i < symmetry.Length; i += 1)
            {
                symmetry[SymmetryMap[i]] = PathMap[state[i]];
            }
        }

        public static void ExplainSymmetries(byte[] state)
        {
            var fringePdb = new PatternDatabase("fringe.data");
            var fringeOutputs = new List<string>();
            uint best = 0;
            byte[] symmetryState = new byte[state.Length];
            foreach (var symmetry in Symmetry.N4Symmetries)
            {
                symmetry.ComposeSymmetry(state, symmetryState);
                var h = fringePdb.Evaluate(symmetryState);
                bool isOriginal = symmetry.Name == "O";
                string stateString = string.Join(" ", symmetryState.Select(m => string.Format("{0,2}", isOriginal || fringePdb.Pattern.Contains(m) ? m.ToString() : "-")));
                //string stateString = string.Join(" ", symmetryState.Select(m => string.Format("{0,2}", m)));
                var bound = symmetry.Penalty > h ? 0 : h - symmetry.Penalty;
                if (bound > best) best = bound;
                var outputLine = string.Format("{0,2}  {1}  {2,2}  -{3,1}  {4,2}", symmetry.Name, stateString, h, symmetry.Penalty, bound);
                fringeOutputs.Add(outputLine);
            }
            Console.WriteLine("---- Fringe PDB ----");
            foreach (var line in fringeOutputs.Distinct())
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("Best: " + best);
            var cornerPdb = new PatternDatabase("corner.data");
            var cornerOutputs = new List<string>();
            best = 0;
            foreach (var symmetry in Symmetry.N4Symmetries)
            {
                symmetry.ComposeSymmetry(state, symmetryState);
                var h = cornerPdb.Evaluate(symmetryState);
                bool isOriginal = symmetry.Name == "O";
                string stateString = string.Join(" ", symmetryState.Select(m => string.Format("{0,2}", isOriginal || cornerPdb.Pattern.Contains(m) ? m.ToString() : "-")));
                //string stateString = string.Join(" ", symmetryState.Select(m => string.Format("{0,2}", m)));
                var bound = symmetry.Penalty > h ? 0 : h - symmetry.Penalty;
                if (bound > best) best = bound;
                var outputLine = string.Format("{0,2}  {1}  {2,2}  -{3,1}  {4,2}", symmetry.Name, stateString, h, symmetry.Penalty, bound);
                cornerOutputs.Add(outputLine);
            }
            Console.WriteLine("---- Corner PDB ----");
            foreach (var line in cornerOutputs.Distinct())
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("Best: " + best);
        }

        public static readonly Symmetry[] N4Symmetries =
        {
            new Symmetry("O", new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, 0u),
            new Symmetry("OH", new byte[] { 3, 2, 1, 0, 7, 6, 5, 4, 11, 10, 9, 8, 15, 14, 13, 12 }, new byte[] { 0, 3, 2, 1, 7, 6, 5, 4, 11, 10, 9, 8, 15, 14, 13, 12 }, 3u),
            new Symmetry("OV", new byte[] { 12, 13, 14, 15, 8, 9, 10, 11, 4, 5, 6, 7, 0, 1, 2, 3 }, new byte[] { 0, 13, 14, 15, 12, 9, 10, 11, 8, 5, 6, 7, 4, 1, 2, 3 }, 3u),

            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 11, 7, 3, 15, 10, 6, 2, 14, 9, 5, 1, 13, 12, 8, 4 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 11, 7, 3, 15, 10, 6, 2, 14, 13, 5, 1, 12, 9, 8, 4 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 11, 7, 3, 15, 14, 6, 2, 13, 10, 5, 1, 12, 9, 8, 4 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 15, 7, 3, 14, 11, 6, 2, 13, 10, 5, 1, 12, 9, 8, 4 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 11, 7, 3, 15, 10, 6, 2, 14, 13, 9, 1, 12, 8, 5, 4 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 11, 7, 3, 15, 14, 6, 2, 13, 10, 9, 1, 12, 8, 5, 4 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 15, 7, 3, 14, 11, 6, 2, 13, 10, 9, 1, 12, 8, 5, 4 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 11, 7, 3, 15, 14, 10, 2, 13, 9, 6, 1, 12, 8, 5, 4 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 15, 7, 3, 14, 11, 10, 2, 13, 9, 6, 1, 12, 8, 5, 4 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 15, 11, 3, 14, 10, 7, 2, 13, 9, 6, 1, 12, 8, 5, 4 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 15, 11, 7, 14, 10, 6, 3, 13, 9, 5, 2, 12, 8, 4, 1 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 15, 11, 3, 14, 10, 7, 6, 13, 9, 5, 2, 12, 8, 4, 1 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 15, 7, 3, 14, 11, 10, 6, 13, 9, 5, 2, 12, 8, 4, 1 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 11, 7, 3, 15, 14, 10, 6, 13, 9, 5, 2, 12, 8, 4, 1 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 15, 11, 3, 14, 10, 7, 2, 13, 9, 6, 5, 12, 8, 4, 1 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 15, 7, 3, 14, 11, 10, 2, 13, 9, 6, 5, 12, 8, 4, 1 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 11, 7, 3, 15, 14, 10, 2, 13, 9, 6, 5, 12, 8, 4, 1 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 15, 7, 3, 14, 11, 6, 2, 13, 10, 9, 5, 12, 8, 4, 1 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 11, 7, 3, 15, 14, 6, 2, 13, 10, 9, 5, 12, 8, 4, 1 }, 6u),
            new Symmetry("OD", new byte[] { 15, 11, 7, 3, 14, 10, 6, 2, 13, 9, 5, 1, 12, 8, 4, 0 }, new byte[] { 0, 11, 7, 3, 15, 10, 6, 2, 14, 13, 9, 5, 12, 8, 4, 1 }, 6u),

            new Symmetry("M", new byte[] { 0, 4, 8, 12, 1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15 }, new byte[] { 0, 4, 8, 12, 1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15 }, 0u),
            new Symmetry("MH", new byte[] { 12, 8, 4, 0, 13, 9, 5, 1, 14, 10, 6, 2, 15, 11, 7, 3 }, new byte[] { 0, 12, 8, 4, 13, 9, 5, 1, 14, 10, 6, 2, 15, 11, 7, 3 }, 3u),
            new Symmetry("MV", new byte[] { 3, 7, 11, 15, 2, 6, 10, 14, 1, 5, 9, 13, 0, 4, 8, 12 }, new byte[] { 0, 7, 11, 15, 3, 6, 10, 14, 2, 5, 9, 13, 1, 4, 8, 12 }, 3u),

            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 15, 14, 13, 11, 10, 9, 12, 7, 6, 5, 8, 3, 2, 1, 4 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 14, 13, 12, 15, 10, 9, 8, 11, 6, 5, 4, 7, 3, 2, 1 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 15, 14, 12, 11, 10, 13, 9, 7, 6, 5, 8, 3, 2, 1, 4 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 14, 13, 12, 15, 10, 9, 8, 11, 7, 5, 4, 3, 6, 2, 1 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 15, 13, 12, 11, 14, 10, 9, 7, 6, 5, 8, 3, 2, 1, 4 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 14, 13, 12, 15, 11, 9, 8, 7, 10, 5, 4, 3, 6, 2, 1 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 14, 13, 12, 15, 11, 10, 9, 7, 6, 5, 8, 3, 2, 1, 4 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 15, 13, 12, 11, 14, 9, 8, 7, 10, 5, 4, 3, 6, 2, 1 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 15, 14, 12, 11, 10, 13, 8, 7, 6, 9, 5, 3, 2, 1, 4 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 14, 13, 12, 15, 10, 9, 8, 11, 7, 6, 4, 3, 2, 5, 1 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 15, 13, 12, 11, 14, 10, 8, 7, 6, 9, 5, 3, 2, 1, 4 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 14, 13, 12, 15, 11, 9, 8, 7, 10, 6, 4, 3, 2, 5, 1 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 14, 13, 12, 15, 11, 10, 8, 7, 6, 9, 5, 3, 2, 1, 4 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 15, 13, 12, 11, 14, 9, 8, 7, 10, 6, 4, 3, 2, 5, 1 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 15, 13, 12, 11, 14, 9, 8, 7, 10, 6, 5, 3, 2, 1, 4 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 14, 13, 12, 15, 11, 10, 8, 7, 6, 9, 4, 3, 2, 5, 1 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 14, 13, 12, 15, 11, 9, 8, 7, 10, 6, 5, 3, 2, 1, 4 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 15, 13, 12, 11, 14, 10, 8, 7, 6, 9, 4, 3, 2, 5, 1 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 14, 13, 12, 15, 10, 9, 8, 11, 7, 6, 5, 3, 2, 1, 4 }, 6u),
            new Symmetry("MD", new byte[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }, new byte[] { 0, 15, 14, 12, 11, 10, 13, 8, 7, 6, 9, 4, 3, 2, 5, 1 }, 6u),
        };
    }
}

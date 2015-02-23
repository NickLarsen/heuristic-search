using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace npuzzle
{
    class Program
    {
        private const string timerFormat = @"hh\:mm\:ss\.fff";
        private static Stopwatch timer;
        private static long lastMillis;
        private static ulong nodeCounter;
        private static string outputFormat;

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    switch (args[0])
                    {
                        case "ida*":
                            DoIDAStar(args);
                            break;
                        case "make-csv":
                            MakeCsv(args);
                            break;
                        case "compare-heuristics":
                            CompareHeuristics(args);
                            break;
                        case "build-pdb":
                            CreatePDB(args);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug();
            }
        }

        static void Debug()
        {
            //CreatePDB(new[] { "build-pdb", "fringe.data", "4", "4", "0,3,7,11,12,13,14,15" });
            //CreatePDB(new[] { "build-pdb", "corner.data", "4", "4", "0,8,9,10,12,13,14,15" });
            FastIDAStar();
            //MakeCsv(new string[] { "make-csv", @"results\korf15-ida-md-{0}.txt", @"results\korf15-ida-md-fr-{0}.txt", @"results\korf15-ida-md-co-{0}.txt", @"results\korf15-ida-md-fc-{0}.txt"});
            //Symmetry.ExplainSymmetries(KorfPuzzles.Puzzles[78].InitialState);
        }

        static void FastIDAStar()
        {
            byte[] goal = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            var puzzle = KorfPuzzles.Puzzles[87];
            Console.WriteLine("\n\n{0}: < {1} >, best: {2:n0}, korf: {3:n0}", puzzle.Number, string.Join(",", puzzle.InitialState), puzzle.Actual, puzzle.KorfNodesExpanded);
            var heuristic = GetIDAStarHeuristic();
            timer = Stopwatch.StartNew();
            var solution = IDAStarDriver(puzzle.InitialState, goal, heuristic);
            timer.Stop();
            if (solution.Length > 0)
            {
                PrintSolution(solution, Console.Out);
            }
            else
            {
                Console.WriteLine("\nNo solution possible.");
            }
        }

        static void MakeCsv(string[] args)
        {
            var data = new ulong[KorfPuzzles.Puzzles.Length][];
            foreach (var korfPuzzle in KorfPuzzles.Puzzles)
            {
                int puzzleIndex = korfPuzzle.Number - 1;
                data[puzzleIndex] = new ulong[args.Length];
                data[puzzleIndex][0] = Convert.ToUInt64(korfPuzzle.Number);
                for (int i = 1; i < args.Length; i += 1)
                {
                    string filename = string.Format(args[i], korfPuzzle.Number);
                    var importantLine = File.ReadAllLines(filename)[1];
                    var evalnodes = Convert.ToUInt64(importantLine.Substring(54));
                    data[puzzleIndex][i] = evalnodes;
                }
            }
            var outputlines = data.Select(m => string.Join(",", m));
            File.WriteAllLines("output.csv", outputlines);
        }

        static void CompareHeuristics(string[] args)
        {
            var fringePdb = new PatternDatabase("fringe.data");
            var results = new Tuple<int, uint, uint, uint>[KorfPuzzles.Puzzles.Length];
            foreach (var puzzle in KorfPuzzles.Puzzles)
            {
                uint hMD = ManhattanDistance(puzzle.InitialState);
                uint hFringe = fringePdb.Evaluate(puzzle.InitialState);
                uint better = hFringe > hMD ? hFringe - hMD : 0;
                results[puzzle.Number - 1] = Tuple.Create(puzzle.Number, hMD, hFringe, better);
            }
            foreach (var test in results.OrderBy(m => m.Item4))
            {
                if (test.Item2 > test.Item3)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                }
                else if (test.Item2 < test.Item3)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                }
                Console.Write("{0,3}:  hMD={1,3}  n={2,3}  improve:{3,3}", test.Item1, test.Item2, test.Item3, test.Item4);
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        static void CreatePDB(string[] args)
        {
            if (args.Length > 1)
            {
                outputFormat = args[1];
            }
            int rows = Convert.ToInt32(args[2]);
            int cols = Convert.ToInt32(args[3]);
            byte[] pattern = args[4].Split(',').Select(m => Convert.ToByte(m)).ToArray();
            byte[] goal = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            timer = Stopwatch.StartNew();
            var pdb = PatternDatabase.Create(rows, cols, pattern, goal, ShowCreateStats, ExpandState);
            timer.Stop();
            Console.WriteLine();
            Console.WriteLine("Completed building Pattern Database.  Saving to {0}", outputFormat);
            pdb.Save(outputFormat);
        }

        static void ShowCreateStats(PatternDatabase.CreateStats stats)
        {
            if (stats.IsFinished || (timer.ElapsedMilliseconds - lastMillis) > 1000)
            {
                Console.Write("\rtime: {0}, total-states: {1}, depth: {2}, eval: {3:n0}", timer.Elapsed.ToString(timerFormat), stats.TotalStates, stats.CurrentDepth, stats.StatesCalculated);
                lastMillis = timer.ElapsedMilliseconds;
            }
        }

        static void DoIDAStar(string[] args)
        {
            if (args.Length > 1)
            {
                outputFormat = args[1];
            }
            DoPuzzles(KorfPuzzles.Puzzles, IDAStarDriver);
        }

        static void DoPuzzles(KorfPuzzle[] puzzles, Func<byte[], byte[], Func<byte[], uint>, byte[][]> algorithm)
        {
            byte[] goal = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            var heuristic = GetIDAStarHeuristic();
            foreach (var korfPuzzle in puzzles)
            {
                if (outputFormat != null)
                {
                    string filename = string.Format(outputFormat, korfPuzzle.Number);
                    if (File.Exists(filename)) continue;
                }
                nodeCounter = 0;
                lastMillis = 0;
                Console.WriteLine("\n\n{0}: < {1} >", korfPuzzle.Number, string.Join(",", korfPuzzle.InitialState));
                Console.WriteLine("actual solution length: {0}, korf nodes evaluated: {1:n0}", korfPuzzle.Actual, korfPuzzle.KorfNodesExpanded);
                timer = Stopwatch.StartNew();
                var solution = algorithm(korfPuzzle.InitialState, goal, heuristic);
                timer.Stop();
                if (solution.Length > 0)
                {
                    PrintSolution(solution, Console.Out);
                    if (outputFormat != null)
                    {
                        string filename = string.Format(outputFormat, korfPuzzle.Number);
                        using (var writer = new StreamWriter(filename))
                        {
                            PrintSolution(solution, writer);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\nNo solution possible.");
                }
            }
        }

        static Func<byte[], uint> GetIDAStarHeuristic()
        {
            var fringePdb = new PatternDatabase("fringe.data");
            var fringeSymmetries = Symmetry.N4Symmetries.Where(m => !m.Name.StartsWith("M")).ToArray();
            var symmetryFringeHeuristic = SymmetryCheckingPdbHeuristic(fringePdb, fringeSymmetries);
            var cornerPdb = new PatternDatabase("corner.data");
            var cornerSymmetries = Symmetry.N4Symmetries.Where(m => true).ToArray();
            var symmetryCornerHeuristic = SymmetryCheckingPdbHeuristic(cornerPdb, cornerSymmetries);
            var heuristic = CompositeHeuristic(ManhattanDistance, symmetryFringeHeuristic, symmetryCornerHeuristic);
            return heuristic;
        }

        static void PrintSolution(byte[][] solution, TextWriter writeLocation)
        {
            writeLocation.WriteLine("\nSolution found! time: {0}, length: {1}, eval: {2}", timer.Elapsed.ToString(timerFormat), solution.Length - 1, nodeCounter);
            foreach (var step in solution)
            {
                writeLocation.WriteLine(string.Join(" ", step));
            }
        }

        private static readonly Stack<byte[]> noSolution = new Stack<byte[]>(); 
        private static uint nextBest;
        private static Func<byte[], uint> idash;
        private static byte[] noParent;
        static byte[][] IDAStarDriver(byte[] initialState, byte[] goal, Func<byte[], uint> h)
        {
            idash = h;
            nextBest = idash(initialState);
            nodeCounter = 1;
            noParent = new byte[initialState.Length];
            var bestPath = noSolution;
            while (bestPath == noSolution && nextBest != uint.MaxValue)
            {
                uint threshold = nextBest;
                nextBest = uint.MaxValue;
                bestPath = IDAStar(initialState, noParent, goal, 0u, threshold);
                Console.WriteLine();
            }
            return bestPath.ToArray();
        }
        static Stack<byte[]> IDAStar(byte[] current, byte[] parent, byte[] goal, uint cost, uint upperbound)
        {
            if (AreSame(current, goal)) return new Stack<byte[]>(new[] { current });
            var newCost = cost + 1;
            var successors = ExpandState(current);
            foreach (var successor in successors)
            {
                if (AreSame(successor, parent)) continue;
                nodeCounter += 1;
                var newF = newCost + idash(successor);
                if (newF > upperbound)
                {
                    if (newF < nextBest) nextBest = newF;
                }
                else
                {
                    var p = IDAStar(successor, current, goal, newCost, upperbound);
                    if (p != noSolution)
                    {
                        p.Push(current);
                        return p;
                    }
                }
            }
            if (parent == noParent || (timer.ElapsedMilliseconds - lastMillis) > 1000)
            {
                var nextBestDisplay = nextBest == uint.MaxValue ? "inf" : nextBest.ToString();
                Console.Write("\rtime: {0}, upperbound: {1}, nextBest: {2}, eval: {3:n0}     ", timer.Elapsed.ToString(timerFormat), upperbound, nextBestDisplay, nodeCounter);
                lastMillis = timer.ElapsedMilliseconds;
            }
            return noSolution;
        }

        static bool AreSame(byte[] a, byte[] b)
        {
            for (int i = 0; i < a.Length; i += 1)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        static List<byte[]> ExpandState(byte[] state)
        {
            var successors = new List<byte[]>();
            int e = 0;
            while (state[e] != 0) e += 1;
            if (e > 3) successors.Add(CreateSuccessor(state, e, e - 4)); // up
            if (e % 4 > 0) successors.Add(CreateSuccessor(state, e, e - 1)); // left
            if (e % 4 < 3) successors.Add(CreateSuccessor(state, e, e + 1)); // right
            if (e < 12) successors.Add(CreateSuccessor(state, e, e + 4)); // down
            return successors;
        }

        static byte[] CreateSuccessor(byte[] state, int a, int b)
        {
            var successor = new byte[state.Length];
            for (int i = 0; i < successor.Length; i += 1)
            {
                successor[i] = state[i];
            }
            successor.Swap(a,b);
            return successor;
        }

        static uint ManhattanDistance(byte[] state)
        {
            uint minMovesRemaning = 0;
            for (uint i = 0; i < state.Length; i += 1)
            {
                byte value = state[i];
                if (value == 0 || value == byte.MaxValue) continue;
                uint ar = value / 4u;
                uint er = i / 4u;
                minMovesRemaning += ar > er ? ar - er : er - ar;
                uint ac = value % 4u;
                uint ec = i % 4u;
                minMovesRemaning += ac > ec ? ac - ec : ec - ac;
            }
            return minMovesRemaning;
        }

        static Func<byte[], uint> SymmetryCheckingPdbHeuristic(PatternDatabase pdb, Symmetry[] symmetries)
        {
            return state =>
            {
                uint best = uint.MinValue;
                byte[] transformed = new byte[state.Length];
                foreach (var symmetry in symmetries)
                {
                    symmetry.ComposeSymmetry(state, transformed);
                    var h = pdb.Evaluate(transformed);
                    var hPenalty = symmetry.Penalty > h ? 0 : h - symmetry.Penalty;
                    best = hPenalty > best ? hPenalty : best;
                }
                return best;
            };
        }

        static Func<byte[],uint> CompositeHeuristic(params Func<byte[], uint>[] heuristics)
        {
            return state =>
            {
                uint best = uint.MinValue;
                foreach (var heuristic in heuristics)
                {
                    var h = heuristic(state);
                    if (h > best) best = h;
                }
                return best;
            };
        }
    }
}

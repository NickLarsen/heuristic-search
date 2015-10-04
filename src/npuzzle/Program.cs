using System;
using System.Collections;
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
            //CreatePDB(new[] { "build-pdb", "15dj-vl.data", "4", "4", "1,4,5,8,9,12,13" });
            //CreatePDB(new[] { "build-pdb", "15dj-vr.data", "4", "4", "2,3,6,7,10,11,14,15" });
            //CreatePDB(new[] { "build-pdb", "24dj2-tr.data", "5", "5", "2,3,4,7,8,9" });
            //CreatePDB(new[] { "build-pdb", "24dj2-br.data", "5", "5", "13,14,18,19,23,24" });
            //CreatePDB(new[] { "build-pdb", "24dj2-bl.data", "5", "5", "15,16,17,20,21,22" });
            //CreatePDB(new[] { "build-pdb", "24dj2-tl.data", "5", "5", "1,5,6,10,11,12" });
            //FastIDAStar(Convert.ToInt32(args[0]));
            FastIDAStar(40);
            //DoIDAStar(new [] { "korf-dj15-{0}.txt" });
            //BreakdownPdbs("24dj-tr.data", "24dj-br.data", "24dj-bl.data", "24dj-tl.data");
            //BreakdownPdbs("15dj-vl.data");
        }

        static void BreakdownPdbs(params string[] filenames)
        {
            foreach (var filename in filenames)
            {
                Console.WriteLine("\n" + filename);
                var pdb = new PatternDatabase(filename);
                var breakdown = pdb.GetValueCounts().OrderBy(m => m.Key).ToArray();
                foreach (var kvp in breakdown)
                {
                    Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
                }
            }
        }

        static void FastIDAStar(int puzzleNumber)
        {
            var puzzle = KorfPuzzles.Puzzles24.Single(m => m.Number == puzzleNumber);
            Console.WriteLine("\n\n{0}: < {1} >", puzzle.Number, string.Join(",", puzzle.InitialState));
            Console.WriteLine("actual solution length: {0}, korf nodes evaluated: {1:n0}", puzzle.Actual, puzzle.KorfNodesExpanded);
            var heuristic = GetIDAStarHeuristicWithMirror24();
            timer = Stopwatch.StartNew();
            var solution = IDAStarDriver(puzzle.InitialState, puzzle.Goal, heuristic);
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

        static void CreatePDB(string[] args)
        {
            if (args.Length > 1)
            {
                outputFormat = args[1];
            }
            uint rows = Convert.ToUInt32(args[2]);
            uint cols = Convert.ToUInt32(args[3]);
            byte[] pattern = args[4].Split(',').Select(m => Convert.ToByte(m)).ToArray();
            byte[] goal = Enumerable.Range(0, (int)(rows * cols)).Select(Convert.ToByte).ToArray();
            lastMillis = 0;
            timer = Stopwatch.StartNew();
            var pdb = PatternDatabase.Create(rows, cols, pattern, goal, ShowCreateStats, ExpandCreateState);
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

        static void DoIDAStar15(string[] args)
        {
            if (args.Length > 0)
            {
                outputFormat = args[0];
            }
            DoPuzzles(KorfPuzzles.Puzzles15, IDAStarDriver, GetIDAStarHeuristicWithMirror15());
        }

        static void DoPuzzles(KorfPuzzle[] puzzles, Func<byte[], byte[], Func<byte[], uint>, byte[][]> algorithm, Func<byte[], uint> heuristic)
        {
            ulong totalNodesExpanded = 0;
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
                var solution = algorithm(korfPuzzle.InitialState, korfPuzzle.Goal, heuristic);
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
                totalNodesExpanded += nodeCounter;
            }
            Console.WriteLine("\n\n\nTotal nodes expanded: " + totalNodesExpanded);
        }

        static Func<byte[], uint> GetIDAStarHeuristicWithMirror15()
        {
            var vl15 = new PatternDatabase("15dj-vl.data");
            var vr15 = new PatternDatabase("15dj-vr.data");
            var hPDB = AdditivePdbHeuristic(null, vl15, vr15);
            var hPDB2 = AdditivePdbHeuristic(MirrorState15, vl15, vr15);
            return state =>
            {
                var hOrig = hPDB(state);
                var hMirror = hPDB2(state);
                return Math.Max(hOrig, hMirror);
            };
        }

        static Func<byte[], uint> GetIDAStarHeuristicWithMirror24()
        {
            var tr24 = new PatternDatabase("24dj-tr.data");
            var br24 = new PatternDatabase("24dj-br.data");
            var bl24 = new PatternDatabase("24dj-bl.data");
            var tl24 = new PatternDatabase("24dj-tl.data");
            var hPDB = AdditivePdbHeuristic(null, tr24, br24, bl24, tl24);
            var hPDB2 = AdditivePdbHeuristic(MirrorState24Fast, tr24, br24, bl24, tl24);
            return state =>
            {
                var hOrig = hPDB(state);
                var hMirror = hPDB2(state);
                return Math.Max(hOrig, hMirror);
            };
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
        private static SuccessorList successorList;
        static byte[][] IDAStarDriver(byte[] initialState, byte[] goal, Func<byte[], uint> h)
        {
            idash = h;
            nextBest = idash(initialState);
            nodeCounter = 0;
            int boxSize = (int)Math.Sqrt(initialState.Length);
            noParent = new byte[initialState.Length];
            var bestPath = noSolution;
            successorList = new SuccessorList(initialState.Length);
            while (bestPath == noSolution && nextBest != uint.MaxValue)
            {
                uint threshold = nextBest;
                nextBest = uint.MaxValue;
                bestPath = IDAStar(initialState, boxSize, noParent, goal, 0u, threshold, successorList);
                Console.WriteLine();
            }
            return bestPath.ToArray();
        }
        static Stack<byte[]> IDAStar(byte[] current, int boxSize, byte[] parent, byte[] goal, uint cost, uint upperbound, SuccessorList successors)
        {
            if (AreSame(current, goal)) return new Stack<byte[]>(new[] { current });
            var newCost = cost + 1;
            ExpandStateOneBlank(boxSize, current, successors);
            while (successors.Size > 0)
            {
                var successor = successors.Pop();
                if (AreSame(successor, parent)) continue;
                nodeCounter += 1;
                var h = idash(successor);
                var newF = newCost + h;
                if (newF > upperbound)
                {
                    if (newF < nextBest) nextBest = newF;
                }
                else
                {
                    var p = IDAStar(successor, boxSize, current, goal, newCost, upperbound, successors.Next);
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
                Console.Write("\rtime: {0}, upperbound: {1}, nextBest: {2}, eval: {3:n0}", timer.Elapsed.ToString(timerFormat), upperbound, nextBestDisplay, nodeCounter);
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

        static void ExpandStateOneBlank(int boxSize, byte[] state, SuccessorList successors)
        {
            int e = 0;
            while (state[e] != 0) e += 1;
            if (e < (state.Length - boxSize)) CreateSuccessor(state, successors.Push(), e, e + boxSize); // down
            if (e % boxSize < (boxSize - 1)) CreateSuccessor(state, successors.Push(), e, e + 1); // right
            if (e % boxSize > 0) CreateSuccessor(state, successors.Push(), e, e - 1); // left
            if (e > (boxSize - 1)) CreateSuccessor(state, successors.Push(), e, e - boxSize); // up
        }
        static void CreateSuccessor(byte[] state, byte[] successor, int a, int b)
        {
            for (int i = 0; i < successor.Length; i += 1)
            {
                successor[i] = state[i];
            }
            successor.Swap(a, b);
        }

        static List<Tuple<byte[], bool>> ExpandCreateState(uint rows, uint cols, byte[] state)
        {
            int e = 0;
            while (state[e] != 0) e += 1;
            int boxSize = (int)rows;
            var successors = new List<Tuple<byte[], bool>>(4);
            if (e % boxSize > 0) successors.Add(CreateSuccessor(state, e, e - 1)); // left
            if (e > (boxSize - 1)) successors.Add(CreateSuccessor(state, e, e - boxSize)); // up
            if (e < (state.Length - boxSize)) successors.Add(CreateSuccessor(state, e, e + boxSize)); // down
            if (e % boxSize < (boxSize - 1)) successors.Add(CreateSuccessor(state, e, e + 1)); // right
            return successors;
        }
        static Tuple<byte[], bool> CreateSuccessor(byte[] state, int a, int b)
        {
            byte[] successor = new byte[state.Length];
            for (int i = 0; i < successor.Length; i += 1)
            {
                successor[i] = state[i];
            }
            successor.Swap(a, b);
            return Tuple.Create(successor, successor[a] == byte.MaxValue);
        }

        static Func<byte[], uint> AdditivePdbHeuristic(Action<byte[], byte[]> stateProcessor, params PatternDatabase[] pdbs)
        {
            return state =>
            {
                var s = state;
                if (stateProcessor != null)
                {
                    s = new byte[state.Length];
                    stateProcessor(state, s);
                }
                var sFast = new byte[state.Length];
                for (byte i = 0; i < state.Length; i++)
                {
                    sFast[s[i]] = i;
                }
                uint score = 0;
                for (int i = 0; i < pdbs.Length; i += 1)
                {
                    score += pdbs[i].Evaluate(sFast);
                }
                return score;
            };
        }

        private static readonly byte[] SymmetryMap15 = new byte[] { 0, 4, 8, 12, 1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15 };
        private static readonly byte[] PathMap15 = new byte[] { 0, 4, 8, 12, 1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15 };
        static void MirrorState15(byte[] state, byte[] symmetry)
        {
            for (int i = 0; i < symmetry.Length; i += 1)
            {
                symmetry[SymmetryMap15[i]] = PathMap15[state[i]];
            }
        }

        private static readonly byte[] SymmetryMap24 = new byte[] { 0, 5, 10, 15, 20, 1, 6, 11, 16, 21, 2, 7, 12, 17, 22, 3, 8, 13, 18, 23, 4, 9, 14, 19, 24 };
        private static readonly byte[] PathMap24 = new byte[] { 0, 5, 10, 15, 20, 1, 6, 11, 16, 21, 2, 7, 12, 17, 22, 3, 8, 13, 18, 23, 4, 9, 14, 19, 24 };
        static void MirrorState24(byte[] state, byte[] symmetry)
        {
            for (int i = 0; i < symmetry.Length; i += 1)
            {
                symmetry[SymmetryMap24[i]] = PathMap24[state[i]];
            }
        }

        static void MirrorState24Fast(byte[] state, byte[] symmetry)
        {
            symmetry[0] = PathMap24[state[0]];
            symmetry[5] = PathMap24[state[1]];
            symmetry[10] = PathMap24[state[2]];
            symmetry[15] = PathMap24[state[3]];
            symmetry[20] = PathMap24[state[4]];
            symmetry[1] = PathMap24[state[5]];
            symmetry[6] = PathMap24[state[6]];
            symmetry[11] = PathMap24[state[7]];
            symmetry[16] = PathMap24[state[8]];
            symmetry[21] = PathMap24[state[9]];
            symmetry[2] = PathMap24[state[10]];
            symmetry[7] = PathMap24[state[11]];
            symmetry[12] = PathMap24[state[12]];
            symmetry[17] = PathMap24[state[13]];
            symmetry[22] = PathMap24[state[14]];
            symmetry[3] = PathMap24[state[15]];
            symmetry[8] = PathMap24[state[16]];
            symmetry[13] = PathMap24[state[17]];
            symmetry[18] = PathMap24[state[18]];
            symmetry[23] = PathMap24[state[19]];
            symmetry[4] = PathMap24[state[20]];
            symmetry[9] = PathMap24[state[21]];
            symmetry[14] = PathMap24[state[22]];
            symmetry[19] = PathMap24[state[23]];
            symmetry[24] = PathMap24[state[24]];
        }
    }
}

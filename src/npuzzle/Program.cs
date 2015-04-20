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
            //FastIDAStar(9);
            DoIDAStar(new [] { "korf-dj15-{0}.txt" });
            //BreakdownPdbs("24dj-tr.data", "24dj-br.data", "24dj-bl.data", "24dj-tl.data");
            //BreakdownPdbs("15dj-vr.data");
            //QuickLook();
        }

        static void QuickLook()
        {
            var h = GetIDAStarHeuristic();
            var a = KorfPuzzles.Puzzles15
                .Average(m => (double)h(m.InitialState));
            Console.WriteLine(a);
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
            var heuristic = GetIDAStarHeuristic();
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

        static void DoIDAStar(string[] args)
        {
            if (args.Length > 0)
            {
                outputFormat = args[0];
            }
            DoPuzzles(KorfPuzzles.Puzzles15, IDAStarDriver);
        }

        static void DoPuzzles(KorfPuzzle[] puzzles, Func<byte[], byte[], Func<byte[], uint>, byte[][]> algorithm)
        {
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
            }
        }

        static Func<byte[], uint> GetIDAStarHeuristic()
        {
            //return AdditivePdbHeuristic("24dj2-tr.data", "24dj2-br.data", "24dj2-bl.data", "24dj2-tl.data");
            return GetIDAStarHeuristicWithMirror();
        }

        static Func<byte[], uint> GetIDAStarHeuristicWithMirror()
        {
            //var hPDB = AdditivePdbHeuristic("24dj-tr.data", "24dj-br.data", "24dj-bl.data", "24dj-tl.data");
            //var hPDB2 = AdditivePdbHeuristic("24dj2-tr.data", "24dj2-br.data", "24dj2-bl.data", "24dj2-tl.data");
            var hPDB = AdditivePdbHeuristic("15dj-vl.data", "15dj-vr.data");
            //var hPDB2 = AdditivePdbHeuristic("15dj-ht.data", "15dj-hb.data");
            return state => hPDB(state);
            //return state =>
            //{
            //    var hOrig = hPDB(state);
            //    var hMirror = hPDB2(state);
            //    return Math.Max(hOrig, hMirror);
            //};
        }

        //private static readonly byte[] SymmetryMap = new byte[] {0, 4, 8, 12, 1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15};
        //private static readonly byte[] PathMap = new byte[] {0, 4, 8, 12, 1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15};
        //static void MirrorState(byte[] state, byte[] symmetry)
        //{
        //    for (int i = 0; i < symmetry.Length; i += 1)
        //    {
        //        symmetry[SymmetryMap[i]] = PathMap[state[i]];
        //    }
        //}

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
            nodeCounter = 1;
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
            if (successors.Size > 0 && successors.Next == null)
            {
                successors.Next = new SuccessorList(current.Length);
            }
            while (successors.Size > 0)
            {
                var successor = successors.Pop();
                if (AreSame(successor, parent)) continue;
                nodeCounter += 1;
                var newF = newCost + idash(successor);
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
            if (e > (boxSize - 1)) CreateSuccessor(state, successors.Push(), e, e - boxSize); // up
            if (e % boxSize > 0) CreateSuccessor(state, successors.Push(), e, e - 1); // left
            if (e % boxSize < (boxSize - 1)) CreateSuccessor(state, successors.Push(), e, e + 1); // right
            if (e < (state.Length - boxSize)) CreateSuccessor(state, successors.Push(), e, e + boxSize); // down
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
            if (e > (boxSize - 1)) successors.Add(CreateSuccessor(state, e, e - boxSize)); // up
            if (e % boxSize > 0) successors.Add(CreateSuccessor(state, e, e - 1)); // left
            if (e % boxSize < (boxSize - 1)) successors.Add(CreateSuccessor(state, e, e + 1)); // right
            if (e < (state.Length - boxSize)) successors.Add(CreateSuccessor(state, e, e + boxSize)); // down
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

        static Func<byte[], uint> AdditivePdbHeuristic(params string[] pdbFilenames)
        {
            var pdbs = pdbFilenames
                .Select(f => new PatternDatabase(f))
                .ToArray();
            return state =>
            {
                uint score = 0;
                for(int i = 0; i < pdbs.Length; i += 1)
                {
                    score += pdbs[i].Evaluate(state);
                }
                return score;
            };
        }
    }
}

﻿using System;
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
            //CreatePDB(new[] { "build-pdb", "24dj-tr.data", "5", "5", "3,4,8,9,13,14" });
            CreatePDB(new[] { "build-pdb", "24dj-br.data", "5", "5", "17,18,19,22,23,24" });
            //CreatePDB(new[] { "build-pdb", "24dj-bl.data", "5", "5", "10,11,15,16,20,21" });
            //CreatePDB(new[] { "build-pdb", "24dj-tl.data", "5", "5", "1,2,5,6,7,12" });
            //FastIDAStar();
        }

        static void FastIDAStar()
        {
            var puzzle = KorfPuzzles.Puzzles[87];
            Console.WriteLine("\n\n{0}: < {1} >, best: {2:n0}, korf: {3:n0}", puzzle.Number, string.Join(",", puzzle.InitialState), puzzle.Actual, puzzle.KorfNodesExpanded);
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
            int rows = Convert.ToInt32(args[2]);
            int cols = Convert.ToInt32(args[3]);
            byte[] pattern = args[4].Split(',').Select(m => Convert.ToByte(m)).ToArray();
            byte[] goal = Enumerable.Range(0, rows * cols).Select(Convert.ToByte).ToArray();
            timer = Stopwatch.StartNew();
            var pdb = PatternDatabase.Create(rows, cols, pattern, goal, ShowCreateStats, ExpandStateFloodBlank);
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
            var vertLeft = new PatternDatabase("disjoint-vert-left.data");
            var vertRight = new PatternDatabase("disjoint-vert-right.data");
            var heuristic = AdditivePdbHeuristic(vertLeft, vertRight);
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
            int boxSize = (int)Math.Sqrt(initialState.Length);
            noParent = new byte[initialState.Length];
            var bestPath = noSolution;
            while (bestPath == noSolution && nextBest != uint.MaxValue)
            {
                uint threshold = nextBest;
                nextBest = uint.MaxValue;
                bestPath = IDAStar(initialState, boxSize, noParent, goal, 0u, threshold);
                Console.WriteLine();
            }
            return bestPath.ToArray();
        }
        static Stack<byte[]> IDAStar(byte[] current, int boxSize, byte[] parent, byte[] goal, uint cost, uint upperbound)
        {
            if (AreSame(current, goal)) return new Stack<byte[]>(new[] { current });
            var newCost = cost + 1;
            var successors = ExpandStateOneBlank(boxSize, current);
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
                    var p = IDAStar(successor, boxSize, current, goal, newCost, upperbound);
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

        static List<byte[]> ExpandStateOneBlank(int boxSize, byte[] state)
        {
            var successors = new List<byte[]>(4);
            int e = 0;
            while (state[e] != 0) e += 1;
            if (e > (boxSize - 1)) successors.Add(CreateSuccessor(state, e, e - boxSize)); // up
            if (e % boxSize > 0) successors.Add(CreateSuccessor(state, e, e - 1)); // left
            if (e % boxSize < (boxSize - 1)) successors.Add(CreateSuccessor(state, e, e + 1)); // right
            if (e < (state.Length - boxSize)) successors.Add(CreateSuccessor(state, e, e + boxSize)); // down
            return successors;
        }
        static byte[] CreateSuccessor(byte[] state, int a, int b)
        {
            var successor = new byte[state.Length];
            for (int i = 0; i < successor.Length; i += 1)
            {
                successor[i] = state[i];
            }
            successor.Swap(a, b);
            return successor;
        }

        static List<byte[]> ExpandStateFloodBlank(int rows, int cols, byte[] state)
        {
            int boxSize = rows; // TODO: rewrite this to actually use both rows and cols
            var successors = new List<byte[]>(4 * boxSize * boxSize);
            var blanks = new byte[state.Length];
            bool madeChange = true;
            int zero = -1;
            while (madeChange)
            {
                madeChange = false;
                for (int i = 0; i < state.Length; i += 1)
                {
                    if (state[i] == 0) zero = i;
                    if (blanks[i] == 1 || (i == zero && blanks[i] == 0))
                    {
                        if (i > (boxSize - 1) && state[i - boxSize] == byte.MaxValue && blanks[i - boxSize] == 0) blanks[i - boxSize] = 1; // up
                        if (i % boxSize > 0 && state[i - 1] == byte.MaxValue && blanks[i - 1] == 0) blanks[i - 1] = 1; // left
                        if (i % boxSize < (boxSize - 1) && state[i + 1] == byte.MaxValue && blanks[i + 1] == 0) blanks[i + 1] = 1; // right
                        if (i < (state.Length - boxSize) && state[i + boxSize] == byte.MaxValue && blanks[i + boxSize] == 0) blanks[i + boxSize] = 1; // down
                        blanks[i] = 2;
                        madeChange = true;
                    }
                }
            }

            for (int e = 0; e < state.Length; e += 1)
            {
                if (blanks[e] != 2) continue;
                if (e > (boxSize - 1) && blanks[e - boxSize] != 2) successors.Add(CreateFloodSuccessor(state, zero, e, e - boxSize)); // up
                if (e % boxSize > 0 && blanks[e - 1] != 2) successors.Add(CreateFloodSuccessor(state, zero, e, e - 1)); // left
                if (e % boxSize < (boxSize - 1) && blanks[e + 1] != 2) successors.Add(CreateFloodSuccessor(state, zero, e, e + 1)); // right
                if (e < (state.Length - boxSize) && blanks[e + boxSize] != 2) successors.Add(CreateFloodSuccessor(state, zero, e, e + boxSize)); // down
            }
            return successors;
        }
        static byte[] CreateFloodSuccessor(byte[] state, int zero, int a, int b)
        {
            var successor = new byte[state.Length];
            for (int i = 0; i < successor.Length; i += 1)
            {
                successor[i] = state[i];
            }
            successor.Swap(zero, a);
            successor.Swap(a, b);
            return successor;
        }

        static Func<byte[], uint> AdditivePdbHeuristic(params PatternDatabase[] pdbs)
        {
            return state =>
            {
                uint best = uint.MinValue;
                for(int i = 0; i < pdbs.Length; i += 1)
                {
                    best += pdbs[i].Evaluate(state);
                }
                return best;
            };
        }
    }
}

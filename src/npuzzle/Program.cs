﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
            try
            {
                if (args.Length > 0)
                {
                    if (args.Length > 1)
                    {
                        outputFormat = args[1];
                    }
                    switch (args[0])
                    {
                        case "ida*":
                            DoIDAStar();
                            break;
                        case "compare":
                            CompareResults();
                            break;
                    }
                }
                else
                {
                    Debug();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Debug()
        {
            byte[] goal = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            var puzzle = KorfPuzzles.Puzzles[11];
            Console.WriteLine("\n\n{0}: < {1} >, best: {2:n0}", puzzle.Number, string.Join(",", puzzle.InitialState), puzzle.KorfNodesExpanded);
            timer = Stopwatch.StartNew();
            var solution = IDAStarDriver(puzzle.InitialState, goal, ManhattanDistance);
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

        static void CompareResults()
        {
            foreach (var korfPuzzle in KorfPuzzles.Puzzles)
            {
                string filename = string.Format(outputFormat, korfPuzzle.Number);
                var importantLine = File.ReadAllLines(filename)[1];
                var evalnodes = Convert.ToUInt64(importantLine.Substring(54));
                if (evalnodes > korfPuzzle.KorfNodesExpanded)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                }
                else if (evalnodes < korfPuzzle.KorfNodesExpanded)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                }
                Console.Write("{0,3}:  k={1,13:n0}  n={2,13:n0}", korfPuzzle.Number, korfPuzzle.KorfNodesExpanded, evalnodes);
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        static void DoIDAStar()
        {
            DoPuzzles(KorfPuzzles.Puzzles, IDAStarDriver);
        }

        static void DoPuzzles(KorfPuzzle[] puzzles, Func<byte[], byte[], Func<byte[], uint>, byte[][]> algorithm)
        {
            byte[] goal = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
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
                var solution = algorithm(korfPuzzle.InitialState, goal, ManhattanDistance);
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
                //nodesEvaluated += 1;
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

        static uint NoHeuristic(byte[] state)
        {
            return 0u;
        }

        static uint HammingDistance(byte[] state)
        {
            uint outOfPlace = 0;
            for(int i = 0; i < state.Length; i += 1)
                if (state[i] != i) outOfPlace += 1;
            return outOfPlace;
        }

        static uint ManhattanDistance(byte[] state)
        {
            uint minMovesRemaning = 0;
            for (uint i = 0; i < state.Length; i += 1)
            {
                byte value = state[i];
                if (value == 0) continue;
                uint ar = value / 4u;
                uint er = i / 4u;
                minMovesRemaning += ar > er ? ar - er : er - ar;
                uint ac = value % 4u;
                uint ec = i % 4u;
                minMovesRemaning += ac > ec ? ac - ec : ec - ac;
            }
            return minMovesRemaning;
        }
    }
}

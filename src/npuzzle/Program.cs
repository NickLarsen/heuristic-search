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
            byte[] goalBytes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            ulong goal = GetStateKey(goalBytes);
            var puzzle = KorfPuzzles.Puzzles[9];
            ulong initial = GetStateKey(puzzle.InitialState);
            Console.WriteLine("\n\n{0}: < {1} >, best: {2:n0}", puzzle.Number, string.Join(",", puzzle.InitialState), puzzle.KorfNodesExpanded);
            timer = Stopwatch.StartNew();
            var solution = IDAStarDriver(initial, goal, ManhattanDistance);
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

        static void DoPuzzles(KorfPuzzle[] puzzles, Func<ulong, ulong, Func<ulong, uint>, ulong[]> algorithm)
        {
            byte[] goalBytes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            ulong goal = GetStateKey(goalBytes);
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
                ulong initial = GetStateKey(korfPuzzle.InitialState);
                timer = Stopwatch.StartNew();
                var solution = algorithm(initial, goal, ManhattanDistance);
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

        static ulong GetStateKey(byte[] tiles)
        {
            ulong value = 0;
            for (int i = 0; i < 16; i += 1)
            {
                int shift = i * 4;
                ulong part = (ulong)tiles[i] << shift;
                value |= part;
            }
            return value;
        }

        static void PrintSolution(ulong[] solution, TextWriter writeLocation)
        {
            writeLocation.WriteLine("\nSolution found! time: {0}, length: {1}, eval: {2}", timer.Elapsed.ToString(timerFormat), solution.Length - 1, nodeCounter);
            foreach (var step in solution)
            {
                PrintState(step, writeLocation);
            }
        }

        static void PrintState(ulong state, TextWriter writeLocation)
        {
            const ulong mask = 0xf;
            for (int shift = 0; shift < 64; shift += 4)
            {
                ulong position = (state >> shift) & mask;
                writeLocation.Write("{0} ", position);
            }
            writeLocation.WriteLine();
        }

        private static readonly Stack<ulong> noSolution = new Stack<ulong>(); 
        private static uint nextBest;
        private static Func<ulong, uint> idash; 
        static ulong[] IDAStarDriver(ulong initialState, ulong goal, Func<ulong, uint> h)
        {
            idash = h;
            nextBest = idash(initialState);
            nodeCounter = 1;
            var bestPath = noSolution;
            while (bestPath == noSolution && nextBest != uint.MaxValue)
            {
                uint threshold = nextBest;
                nextBest = uint.MaxValue;
                //nodesEvaluated += 1;
                bestPath = IDAStar(initialState, 0ul, goal, 0u, threshold);
                Console.WriteLine();
            }
            return bestPath.ToArray();
        }
        static Stack<ulong> IDAStar(ulong current, ulong parent, ulong goal, uint cost, uint upperbound)
        {
            if (current == goal) return new Stack<ulong>(new[] { current });
            var successors = ExpandState(current);
            var newCost = cost + 1;
            foreach (var successor in successors.Where(s => s != parent))
            {
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
            if (parent == 0ul || (timer.ElapsedMilliseconds - lastMillis) > 1000)
            {
                var nextBestDisplay = nextBest == uint.MaxValue ? "inf" : nextBest.ToString();
                Console.Write("\rtime: {0}, upperbound: {1}, nextBest: {2}, eval: {3:n0}     ", timer.Elapsed.ToString(timerFormat), upperbound, nextBestDisplay, nodeCounter);
                lastMillis = timer.ElapsedMilliseconds;
            }
            return noSolution;
        }

        static List<ulong> ExpandState(ulong state)
        {
            var successors = new List<ulong>();
            int e = 0;
            while (((state >> e) & 15) != 0) e += 4;
            if (e > 15) successors.Add(SwapParts(state, e, e - 16)); // up
            if (e % 16 > 0) successors.Add(SwapParts(state, e, e - 4)); // left
            if (e % 16 < 12) successors.Add(SwapParts(state, e, e + 4)); // right
            if (e < 48) successors.Add(SwapParts(state, e, e + 16)); // down
            return successors;
        }

        static ulong SwapParts(ulong state, int fs, int ss)
        {
            ulong fvs = ((state >> fs) & 15) << ss;
            ulong svf = ((state >> ss) & 15) << fs;
            ulong unmoved = ~(15ul << fs) & ~(15ul << ss);
            ulong result = state & unmoved | svf | fvs;
            return result;
        }

        static uint NoHeuristic(ulong state)
        {
            return 0u;
        }

        static uint HammingDistance(ulong state)
        {
            int outOfPlace = 0;
            for(int i = 0, s = 0; i < 16; i += 1, s += 4)
                if ((uint)((state >> s) & 15) != i) outOfPlace += 1;
            return (uint)outOfPlace;
        }

        static uint ManhattanDistance(ulong state)
        {
            int minMovesRemaning = 0;
            for (int i = 0, s = 0; i < 16; i += 1, s += 4)
            {
                int value = (int)((state >> s) & 15);
                if (value == 0) continue;
                int ar = value/4;
                int ac = value%4;
                int er = i/4;
                int ec = i%4;
                minMovesRemaning += Math.Abs(ar - er) + Math.Abs(ac - ec);
            }
            return (uint)minMovesRemaning;
        }
    }
}

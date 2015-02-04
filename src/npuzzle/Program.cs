using System;
using System.Collections.Generic;
using System.Linq;

namespace npuzzle
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] goalBytes = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
            ulong goal = GetStateKey(goalBytes);
            //byte[] initialBytes = { 14, 13, 15, 7, 11, 12, 9, 5, 6, 0, 2, 1, 4, 8, 10, 3 };
            byte[] initialBytes = { 14,1,9,6,4,8,12,5,7,3,2,0,10,11,13,15 };
            ulong initial = GetStateKey(initialBytes);
            var solution = AStar(initial, goal, ManhattanDistance);
            if (solution.Length > 0)
            {
                foreach (var step in solution)
                {
                    PrintState(step);
                }
            }
            else
            {
                Console.WriteLine("No solution possible.");
            }
            Console.ReadLine();
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

        static void PrintState(ulong state)
        {
            const ulong mask = 0xf;
            for (int shift = 0; shift < 64; shift += 4)
            {
                ulong position = (state >> shift) & mask;
                Console.Write("{0} ", position);
            }
            Console.WriteLine();
        }

        static ulong[] AStar(ulong initialState, ulong goal, Func<ulong, int> h)
        {
            ulong nodesEvaluated = 0;
            ulong nodesExpanded = 1;
            var closed = new HashSet<ulong>();
            var open = new HashSet<ulong> {initialState};
            var cameFrom = new Dictionary<ulong, ulong>();
            var g = new Dictionary<ulong, int> {{initialState, 0}};
            var f = new Dictionary<ulong, int> {{initialState, g[initialState] + h(initialState)}};
            while (open.Count > 0)
            {
                nodesEvaluated += 1;
                var current = open
                    .OrderBy(s => f[s])
                    .ThenByDescending(s => g[s])
                    .First();
                if (current == goal)
                {
                    Console.WriteLine("eval: {0}, expanded: {1}", nodesEvaluated, nodesExpanded);
                    return ReconstructPath(cameFrom, goal);
                }
                open.Remove(current);
                closed.Add(current);
                int successorGScore = g[current] + 1;
                var successors = ExpandState(current);
                foreach (var successor in successors)
                {
                    if (closed.Contains(successor)) continue;
                    if (!open.Contains(successor) || successorGScore < g[successor])
                    {
                        nodesExpanded += 1;
                        cameFrom[successor] = current;
                        g[successor] = successorGScore;
                        f[successor] = successorGScore + h(successor);
                        if (!open.Contains(successor)) open.Add(successor);
                    }
                }
                Console.Write("\rbest: {2}, len(open): {3}, eval: {0}, expanded: {1}", nodesEvaluated, nodesExpanded, f[current], open.Count);
            }
            Console.WriteLine("eval: {0}, expanded: {1}", nodesEvaluated, nodesExpanded);
            return new ulong[0];
        }

        static List<ulong> ExpandState(ulong state)
        {
            var successors = new List<ulong>();
            int e = 0;
            while (((state >> e) & 15) != 0) e += 4;
            if (e > 15) successors.Add(SwapParts(state, e, e - 16));
            if (e < 48) successors.Add(SwapParts(state, e, e + 16));
            if (e % 16 > 0) successors.Add(SwapParts(state, e, e - 4));
            if (e % 16 < 12) successors.Add(SwapParts(state, e, e + 4));
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

        static int NoHeuristic(ulong state)
        {
            return 0;
        }

        static int HammingDistance(ulong state)
        {
            int outOfPlace = 0;
            for(int i = 0, s = 0; i < 16; i += 1, s += 4)
                if ((uint)((state >> s) & 15) != i) outOfPlace += 1;
            return outOfPlace;
        }

        static int ManhattanDistance(ulong state)
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
            return minMovesRemaning;
        }

        static ulong[] ReconstructPath(Dictionary<ulong, ulong> cameFrom, ulong current)
        {
            var path = new Stack<ulong>();
            path.Push(current);
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Push(current);
            }
            return path.ToArray();
        }
    }
}

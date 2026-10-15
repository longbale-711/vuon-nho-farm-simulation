using System;
using System.Collections.Generic;

namespace Orchard.Navigation
{
    public sealed class GridPathfinder
    {
        public int Width
        {
            get;
        }
        public int Height
        {
            get;
        }

        private readonly bool[] _blocked;
        private static readonly int[] s_horizontalOffsets =
        {
            1,
            -1,
            0,
            0,
            1,
            1,
            -1,
            -1
        };
        private static readonly int[] s_verticalOffsets =
        {
            0,
            0,
            1,
            -1,
            1,
            -1,
            1,
            -1
        };
        public GridPathfinder(int width, int height)
        {
            Width = width;
            Height = height;
            _blocked = new bool[width * height];
        }

        private int Index(Cell c) => c.Y * Width + c.X;
        public bool Walkable(Cell c) => c.X >= 0 && c.Y >= 0 && c.X < Width && c.Y < Height && !_blocked[Index(c)];
        public void Block(Cell c)
        {
            if (c.X >= 0 && c.Y >= 0 && c.X < Width && c.Y < Height)
            {
                _blocked[Index(c)] = true;
            }
        }

        private static int Heuristic(Cell left, Cell right)
        {
            int dx = Math.Abs(left.X - right.X);
            int dy = Math.Abs(left.Y - right.Y);
            return 10 * Math.Max(dx, dy) + 4 * Math.Min(dx, dy);
        }

        public List<Cell> Find(Cell start, Cell end)
        {
            if (!Walkable(start) || !Walkable(end))
            {
                return null;
            }

            int count = Width * Height;
            var costs = new int[count];
            var parent = new int[count];
            var closed = new bool[count];
            for (int i = 0; i < count; i++)
            {
                costs[i] = int.MaxValue;
                parent[i] = -1;
            }

            var open = new List<Cell>
            {
                start
            };
            costs[Index(start)] = 0;
            while (open.Count > 0)
            {
                int best = 0;
                int score = int.MaxValue;
                for (int i = 0; i < open.Count; i++)
                {
                    int candidate = costs[Index(open[i])] + Heuristic(open[i], end);
                    if (candidate < score)
                    {
                        score = candidate;
                        best = i;
                    }
                }

                var current = open[best];
                open.RemoveAt(best);
                int currentIndex = Index(current);
                if (closed[currentIndex])
                {
                    continue;
                }

                if (current.Equals(end))
                {
                    var result = new List<Cell>();
                    for (int traceIndex = currentIndex; traceIndex != -1; traceIndex = parent[traceIndex])
                    {
                        result.Add(new Cell(traceIndex % Width, traceIndex / Width));
                    }

                    result.Reverse();
                    return result;
                }

                closed[currentIndex] = true;
                for (int d = 0; d < s_horizontalOffsets.Length; d++)
                {
                    var next = new Cell(current.X + s_horizontalOffsets[d], current.Y + s_verticalOffsets[d]);
                    if (!Walkable(next))
                    {
                        continue;
                    }

                    bool diagonal = s_horizontalOffsets[d] != 0 && s_verticalOffsets[d] != 0;
                    if (diagonal && (!Walkable(new Cell(current.X + s_horizontalOffsets[d], current.Y)) || !Walkable(new Cell(current.X, current.Y + s_verticalOffsets[d]))))
                    {
                        continue;
                    }

                    int neighborIndex = Index(next);
                    int cost = costs[currentIndex] + (diagonal ? 14 : 10);
                    if (closed[neighborIndex] || cost >= costs[neighborIndex])
                    {
                        continue;
                    }

                    costs[neighborIndex] = cost;
                    parent[neighborIndex] = currentIndex;
                    open.Add(next);
                }
            }

            return null;
        }
    }
}

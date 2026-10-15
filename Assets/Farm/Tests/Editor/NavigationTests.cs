using System.Collections.Generic;
using NUnit.Framework;
using Orchard.Navigation;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Orchard.Tests
{
    public class NavigationTests
    {
        [Test]
        public void PathCannotCutDiagonalCorner()
        {
            var grid = new GridPathfinder(2, 2);
            grid.Block(new Cell(1, 0));
            grid.Block(new Cell(0, 1));
            Assert.IsNull(grid.Find(new Cell(0, 0), new Cell(1, 1)));
        }

        [Test]
        public void BlockedEndpointsAndDisconnectedDestinationsFail()
        {
            var grid = new GridPathfinder(5, 5);
            for (int y = 0; y < 5; y++)
            {
                grid.Block(new Cell(2, y));
            }

            Assert.IsNull(grid.Find(new Cell(0, 0), new Cell(4, 4)));
            Assert.IsNull(grid.Find(new Cell(2, 0), new Cell(0, 0)));
        }

        [Test]
        public void AStarMatchesDijkstraAcrossSeededObstacleMaps()
        {
            var random = new System.Random(1357);
            for (int map = 0; map < 25; map++)
            {
                var grid = new GridPathfinder(12, 12);
                for (int y = 0; y < 12; y++)
                {
                    for (int x = 0; x < 12; x++)
                    {
                        if ((x != 0 || y != 0) && (x != 11 || y != 11) && random.NextDouble() < 0.21)
                        {
                            grid.Block(new Cell(x, y));
                        }
                    }
                }

                var path = grid.Find(new Cell(0, 0), new Cell(11, 11));
                int expected = Dijkstra(grid, new Cell(0, 0), new Cell(11, 11));
                if (expected == int.MaxValue)
                {
                    Assert.IsNull(path);
                }
                else
                {
                    Assert.IsNotNull(path);
                    int cost = 0;
                    for (int i = 1; i < path.Count; i++)
                    {
                        cost += path[i].X != path[i - 1].X && path[i].Y != path[i - 1].Y ? 14 : 10;
                    }

                    Assert.AreEqual(expected, cost, "Map " + map);
                }
            }
        }

        private static int Dijkstra(GridPathfinder grid, Cell start, Cell end)
        {
            var distances = new Dictionary<Cell, int>
            {
                [start] = 0
            };
            var open = new List<Cell>
            {
                start
            };
            var closed = new HashSet<Cell>();
            while (open.Count > 0)
            {
                open.Sort((left, right) => distances[left].CompareTo(distances[right]));
                var current = open[0];
                open.RemoveAt(0);
                if (closed.Contains(current))
                {
                    continue;
                }

                closed.Add(current);
                if (current.Equals(end))
                {
                    return distances[current];
                }

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

                        var next = new Cell(current.X + dx, current.Y + dy);
                        if (!grid.Walkable(next) || closed.Contains(next))
                        {
                            continue;
                        }

                        if (dx != 0 && dy != 0 && (!grid.Walkable(new Cell(current.X + dx, current.Y)) || !grid.Walkable(new Cell(current.X, current.Y + dy))))
                        {
                            continue;
                        }

                        int candidate = distances[current] + (dx == 0 || dy == 0 ? 10 : 14);
                        if (!distances.ContainsKey(next) || candidate < distances[next])
                        {
                            distances[next] = candidate;
                            open.Add(next);
                        }
                    }
                }
            }

            return int.MaxValue;
        }

        [Test]
        public void EveryFarmHarvestAndMarketPointIsReachable()
        {
            var nav = new FarmNavigation();
            var points = new List<Vector3>();
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 2; col++)
                {
                    var p = new Vector3(col == 0 ? -2.8f : 2.8f, 0, -4 + row * 3.4f);
                    nav.AddObstacle(p, new Vector2(1.8f, 1.8f));
                    points.Add(p + new Vector3(0, 0, -1.6f));
                }
            }

            nav.AddObstacle(new Vector3(0, 0, 6.6f), new Vector2(10, 1));
            nav.AddObstacle(new Vector3(0, 0, 3.8f), new Vector2(1.15f, 1.15f));
            foreach (var point in points)
            {
                for (int lane = 0; lane < 3; lane++)
                {
                    Assert.IsNotNull(nav.Find(point, new Vector3((lane - 1) * 3, 0, 5.15f)));
                }
            }

            for (int lane = 0; lane < 3; lane++)
            {
                for (int rank = 0; rank < 4; rank++)
                {
                    Assert.IsNotNull(nav.Find(new Vector3(6.8f, 0, 12.4f), new Vector3((lane - 1) * 3, 0, 8.2f + rank * 1.1f)));
                }
            }
        }
    }
}

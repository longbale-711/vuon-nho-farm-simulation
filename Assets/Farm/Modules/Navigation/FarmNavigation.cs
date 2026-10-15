using System.Collections.Generic;
using UnityEngine;

namespace Orchard.Navigation
{
    public sealed class FarmNavigation
    {
        private const float Step = 0.4f;
        private static readonly Vector3 s_origin = new Vector3(-7.2f, 0, -7.2f);
        private readonly GridPathfinder _grid = new GridPathfinder(37, 56);
        public void AddObstacle(Vector3 center, Vector2 size, float clearance = 0.32f)
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    Vector3 point = World(new Cell(x, y));
                    if (Mathf.Abs(point.x - center.x) <= size.x / 2 + clearance && Mathf.Abs(point.z - center.z) <= size.y / 2 + clearance)
                    {
                        _grid.Block(new Cell(x, y));
                    }
                }
            }
        }

        private Cell CellAt(Vector3 p) => new Cell(Mathf.RoundToInt((p.x - s_origin.x) / Step), Mathf.RoundToInt((p.z - s_origin.z) / Step));
        private Vector3 World(Cell c) => s_origin + new Vector3(c.X * Step, 0, c.Y * Step);
        public List<Vector3> Find(Vector3 start, Vector3 destination)
        {
            var cells = _grid.Find(CellAt(start), CellAt(destination));
            if (cells == null)
            {
                return null;
            }

            var path = new List<Vector3>();
            // Follow every grid edge: a shortcut could cross an inflated obstacle.
            for (int i = 1; i < cells.Count; i++)
            {
                path.Add(World(cells[i]));
            }

            path.Add(new Vector3(destination.x, 0, destination.z));
            return path;
        }
    }
}

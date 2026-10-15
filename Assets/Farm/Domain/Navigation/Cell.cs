using System;

namespace Orchard.Navigation
{
    public readonly struct Cell : IEquatable<Cell>
    {
        public int X
        {
            get;
        }
        public int Y
        {
            get;
        }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Cell other) => X == other.X && Y == other.Y;
        public override bool Equals(object otherObject) => otherObject is Cell other && Equals(other);
        public override int GetHashCode() => X * 397 ^ Y;
    }
}

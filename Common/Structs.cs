using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Common
{
    internal readonly struct Direction
    {
        public int X { get; }
        public int Y { get; }

        private Direction(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static readonly Direction Up = new(0, -1);
        public static readonly Direction Down = new(0, 1);
        public static readonly Direction Left = new(-1, 0);
        public static readonly Direction Right = new(1, 0);

        public int[] ToArray() => new int[] { X, Y };
    }


}

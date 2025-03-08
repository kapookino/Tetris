using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Common
{
    internal static class Config
    {
        public const int gameWidth = 10;
        public const int gameHeight = 20;
        public const int gridWidth = 30;
        public const int gridHeight = 20;
        public const int renderWidth = 10;
        public const int renderHeight = 20;
        public const int bufferHeight = 129;
        public const int bufferWidth = 129;
        public const int windowWidth = 129;
        public const int windowHeight = 129;
        public static int[] startingCellCoordinate { get; private set; } = { 3, 0 };

    }

}

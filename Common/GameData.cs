using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Core;

namespace Tetris.Common
{
    internal class GameData
    {
        public int score { get; private set; } = 0;
        public static int level { get; private set; } = 0;
        public int rowsCleared { get; private set; } = 0;
        public int IShapes { get; private set; } = 0;
        public int OShapes { get; private set; } = 0;
        public int TShapes { get; private set; } = 0;
        public int SShapes { get; private set; } = 0;
        public int ZShapes { get; private set; } = 0;
        public int JShapes { get; private set; } = 0;
        public int LShapes { get; private set; } = 0;

        public GameData()
        {
            GameEvents.OnScoreClearRows += ScoreRows;
            GameEvents.OnCountShape += IncrementShapeCounts;
        }

        private void ScoreRows(int rows)
        {
            switch (rows)
            {
                case 0:
                    break;
                case 1:
                    score += 40 * (1 +level);
                    break;
                case 2:
                    score += 100 * (1 + level);
                    break;
                case 3:
                    score += 300 * (1 + level);
                    break;
                case 4:
                    score += 1200 * (1 + level);
                    break;
                default:
                    break;
            }

            CountRows(rows);
        }

        private void CountRows(int rows)
        {
            rowsCleared += rows;
            if(rowsCleared >= 1)
            {
                level++;
                rowsCleared = 0;
            }
        }

        private void IncrementShapeCounts(ShapeType input)
        {
            switch (input)
            {
                case ShapeType.I:
                    IShapes++;
                    break;
                case ShapeType.O:
                    OShapes++;
                    break;
                case ShapeType.T:
                    TShapes++;
                    break;
                case ShapeType.S:
                    SShapes++;
                    break;
                case ShapeType.Z:
                    ZShapes++;
                    break;
                case ShapeType.J:
                    JShapes++;
                    break;
                case ShapeType.L:
                    LShapes++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(input), $"Unexpected ShapeType: {input}");
            }
        }

    }
}

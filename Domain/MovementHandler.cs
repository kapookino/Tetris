using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Common;
using Tetris.Core;
using Tetris.Render;

namespace Tetris.Domain
{
    internal class MovementHandler
    {
        public MovementHandler() { }

        public bool ValidateSpawn(int[] spawnCoordinate, Shape shape, Grid grid)
        {
            foreach (int[] coordinate in shape.coordinateList)
            { 
                int newX = spawnCoordinate[0] + coordinate[0];
                int newY = spawnCoordinate[1] + coordinate[1];
                Cell? newCell = grid.GetCell((newX, newY));
                if (newX < 0 || newY < 0 || newX == Config.gameWidth || newY == Config.gameHeight || (newCell.HasShape() && newCell.shape != shape))
                {
                    return false;   
                }
            }
            return true;
        }
        public List<Cell> Move(int[] spawnCoordinate, Grid grid, Shape shape)
        {

            List<Cell> cells = new();
            foreach (int[] coordinate in shape.coordinateList)
            {
                int newX = spawnCoordinate[0]+coordinate[0];
                int newY =spawnCoordinate[1]+coordinate[1];
                Cell? activateCell = grid.GetCell((newX, newY));
                activateCell.Activate(shape);
                cells.Add(activateCell);
            }

            return cells;
        }

    }
}

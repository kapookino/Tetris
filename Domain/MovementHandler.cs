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

        // need to rethink MoveValid to be broader use: ideally just have a generic method that gets a spawnCoordinate and sees if 
        // the inputted shape can move there. The notion of direction would be input outside of this, and instead MoveValid will just take
        // the spawn coordinate

        public bool ValidateSpawn(int[] spawnCoordinate, Shape shape, Grid grid)
        {
            foreach (int[] coordinate in shape.coordinateList)
            { 
                int newX = spawnCoordinate[0] + coordinate[0];
                int newY = spawnCoordinate[1] + coordinate[1];
                Cell? newCell = grid.GetCell((newX, newY));
                if (newX < 0 || newY < 0 || newX == Config.gridWidth || newY == Config.gridHeight || (newCell.HasShape() && newCell.shape != shape))
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

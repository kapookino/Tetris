using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Core;
using Tetris.Render;
using Tetris.Common;

namespace Tetris.Domain
{
    /// <summary>
    /// Manages the current tetromino piece, its movement, rotation, and collision detection
    /// </summary>
    internal class ShapeController
    {
        private Grid grid;
        /// <summary>The active tetromino that the player is controlling</summary>
        public Shape? CurrentShape { get; private set; }
        
        private int[] spawnCoordinate;
        private List<Cell> shapeCells;
        private List<ShapeType> shapeBag = new();

        public ShapeController(Grid grid)
        {
            this.grid = grid;
            spawnCoordinate = Config.startingCellCoordinate;
            GameEvents.OnRequestMove += TryMove;
            GameEvents.OnRequestDrop += TryDrop;
            GameEvents.OnRequestRotate += TryRotateShape;
            GameEvents.OnRequestSpawnShape += SpawnShape;
            GameEvents.OnStateChange += ScoreShape;
            GameEvents.OnStateChange += ResetSpawnCoordinate;
            FillShapeBag();

        }
        private void SpawnShape()
        {
            SetCurrentShape(NewShape());
            SetShapeCells(CurrentShape);
        }
        private Shape NewShape()
        {
            ShapeType shapeType = shapeBag[0];
            Shape shape = new(shapeType);
            EmptyShapeBag(shapeType);
            return shape;
        }

        private void FillShapeBag()
        {
            foreach (ShapeType shapeType in Enum.GetValues(typeof(ShapeType)))
            {
                shapeBag.Add(shapeType);
                Utils.ShuffleList(shapeBag);
            }
        }

        private void EmptyShapeBag(ShapeType shapeType)
        {
            shapeBag.Remove(shapeType);
            if (shapeBag.Count == 0)
            {
                FillShapeBag();
            }
        }
        private void SetCurrentShape(Shape shape)
        {
            CurrentShape = shape;

        }

        private void ScoreShape(GameState gameState)
        {
            if (gameState == GameState.Freeze)
            {
                ActionQueue.TryEnqueue(ActionKey.Count, () => GameEvents.CountShape(CurrentShape.shapeType));
            }
        }

        private void TryMove(int[] direction)
        {
            List<Cell> oldShapeCells = new(shapeCells);
            foreach (Cell cell in shapeCells)
            {
                int newX = cell.location.Item1 + direction[0];
                int newY = cell.location.Item2 + direction[1];

                Cell? newCell = grid.GetCell((newX, newY));

                if (newX < 0 || newY < 0 || newX == Config.gridWidth || newY == Config.gridHeight || (newCell.HasShape() && newCell.shape != CurrentShape))
                {
                    if (direction[0] != 0)
                    {
                        Renderer.RenderDebug("Bounds", 10);

                    }
                    else
                    {
                        GameEvents.RequestChangeState(GameState.Freeze);
                    }
                    return;
                }
            }

            Move(direction);

            foreach (Cell cell in oldShapeCells)
            {
                if (!shapeCells.Contains(cell))
                {
                    cell.Deactivate();
                }
            }


        }

        private void Move(int[] direction)
        {
            SetSpawnCoordinate(new int[] { spawnCoordinate[0] + direction[0], spawnCoordinate[1] + direction[1] });

            List<Cell> cells = new();
            foreach (Cell cell in shapeCells)
            {
                //   cell.Deactivate();
                int newX = cell.location.Item1 + direction[0];
                int newY = cell.location.Item2 + direction[1];
                Cell? activateCell = grid.GetCell((newX, newY));
                activateCell.Activate(CurrentShape);
                cells.Add(activateCell);
            }

            shapeCells = cells;
        }
        private void SetShapeCells(Shape shape)
        {
            shapeCells = new();
            foreach (int[] coordinate in shape.coordinateList)
            {
                Cell? shapeCell = grid.GetCell((coordinate[0] + spawnCoordinate[0], coordinate[1] + spawnCoordinate[1]));
                shapeCells.Add(shapeCell);
                shapeCell.Activate(shape);
                
            }
        }

        private void TryRotateShape()
        {
            int findRotation = FindNextValidShapeRotation();

            if (findRotation != -1)
            {
                RotateShape(findRotation);
                foreach (Cell cell in shapeCells)
                {
                    cell.Deactivate();
                }
                SetShapeCells(CurrentShape);
            }
        }
        private int FindNextValidShapeRotation()
        {
            int rotationCheck = CurrentShape.rotation;

            for (int i = 0; i < CurrentShape.coordinateDictionary.Count; i++)
            {
                if (rotationCheck + 1 == CurrentShape.coordinateDictionary.Count)
                {
                    rotationCheck = 0;
                }
                else
                {
                    rotationCheck += 1;
                }

                if (CurrentShape.coordinateDictionary.TryGetValue(rotationCheck, out List<int[]>? coordinates))
                {
                    foreach (int[] coordinate in coordinates)
                    {
                        int newX = coordinate[0] + spawnCoordinate[0];
                        int newY = coordinate[1] + spawnCoordinate[1];

                        Cell? newCell = grid.GetCell((newX, newY));

                        if (newX < 0 || newY < 0 || newX == Config.gridWidth || newY == Config.gridHeight || (newCell.HasShape() && newCell.shape != CurrentShape))
                        {
                            return -1;
                        }
                    }

                    return rotationCheck;
                }
                else
                {
                    throw new Exception("Invalid rotation");
                }
            }
            return -1;
        }
        private void RotateShape(int rotation)
        {
            CurrentShape.SetRotation(rotation);
        }

        private void SetSpawnCoordinate(int[] input)
        {
            spawnCoordinate = input;
        }

        private void ResetSpawnCoordinate(GameState state)
        {
            if (state == GameState.Spawn)
            {
                SetSpawnCoordinate(Config.startingCellCoordinate);
            }
        }


    }
}

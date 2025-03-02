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
        readonly MovementHandler movementHandler;   
        private int[] spawnCoordinate;
        private List<Cell> shapeCells;
        private List<ShapeType> shapeBag = new();
        private int[] dropSpawnCoordinate;

        public ShapeController(Grid grid, MovementHandler movementHandler)
        {
            this.grid = grid;
            this.movementHandler = movementHandler;
            spawnCoordinate = Config.startingCellCoordinate;
            GameEvents.OnRequestMove += TryMove;
            GameEvents.OnRequestDrop += Drop;
            GameEvents.OnRequestRotate += TryRotateShape;
            GameEvents.OnRequestSpawnShape += SpawnShape;
            GameEvents.OnStateChange += ScoreShape;
            GameEvents.OnStateChange += ResetSpawnCoordinate;
            FillShapeBag();

        }
        private void SpawnShape()
        {
            SetCurrentShape(NewShape());
            bool spawnValid = movementHandler.ValidateSpawn(spawnCoordinate, CurrentShape, grid);
            SetShapeCells(CurrentShape);
             if(!spawnValid)
            {
                GameEvents.RequestChangeState(GameState.End);
            } else
            {
                GameEvents.RequestChangeState(GameState.Movement);
            }

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

        private void TryMove(int[] direction = null, int[] newSpawnCoordinate = null)
        {

            int[] checkSpawnCoordinate = newSpawnCoordinate;
            if(newSpawnCoordinate == null && direction == null)
            {
                throw new Exception("TryMove needs to have either a direction or a newSpawnCoordinate");
            }else if (newSpawnCoordinate == null)
            {

                checkSpawnCoordinate = new int[] { spawnCoordinate[0] + direction[0], spawnCoordinate[1] + direction[1] };
            } 
            
            // The cells to be deactivated if the movement is valid
            List<Cell> oldShapeCells = new(shapeCells);
            if(movementHandler.ValidateSpawn(checkSpawnCoordinate, CurrentShape, grid))
            {
               spawnCoordinate = checkSpawnCoordinate;
               shapeCells = movementHandler.Move(checkSpawnCoordinate, grid, CurrentShape);
            }
            else
            {
                if (direction[0] == 0)
                {
                    GameEvents.RequestChangeState(GameState.Freeze);
                    return;
                }
                return;
            }

            // Deactivate the prior cells
            foreach (Cell cell in oldShapeCells)
            {
                if (!shapeCells.Contains(cell))
                {
                    cell.Deactivate();
                }
            }


        }
        
        private void Drop()
        {
            SetDropCoordinate();
            TryMove(null, dropSpawnCoordinate);
            GameEvents.RequestChangeState(GameState.Freeze);
        }
        private void SetDropCoordinate()
        {
            dropSpawnCoordinate = FindDropCoordinate();
        }
        // Return the int[] value corresponding to the freeze point of the active shape
        private int[] FindDropCoordinate()
        {
            int[] priorSpawnCoordinate = spawnCoordinate;
            for(int i = spawnCoordinate[1] + 1; i < Config.gridHeight; i++) 
            {
                int[] checkSpawnCoordinate = new int[] { spawnCoordinate[0], i };
                bool validSpawn = movementHandler.ValidateSpawn(checkSpawnCoordinate,CurrentShape,grid);
                if (validSpawn)
                {
                    priorSpawnCoordinate = checkSpawnCoordinate;
                }
                else
                {
                    return priorSpawnCoordinate;
                }
            }

            throw new Exception("No valid drop point. " +
                "This should only be the case if the game " +
                "should be over but should be caught earlier");
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

        public void SetSpawnCoordinate(int[] input)
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

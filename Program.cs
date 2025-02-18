/*
 * TETRIS
 * 
 * This project is to recreate the classic game of Tetris. Primary learning goals:
 * 1. Reduce reliance on Static classes by utilizing Dependency Injection
 *      - Grid and Cells need to be reworked. The "grid" class can pass in a
 *      Config class, which will hold necessary variables
 *      
 * 2. Learn more about when to use abstract classes vs interface 
 *      - Perhaps a divvying up of responsibilities, such as the shapes (or "tetronominoes") properties
 *      like location being handled by an abstract class but the movement/rotation of shapes being handled
 *      by an interface
 * 3. Improve previously used renderer by only updating changed cells (rather than a full rewrite)
 *      - Look into String Builder and better practices around buffering
 * 
 * 
 */

// Shape represents the overarching "tetronomino"

using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks.Dataflow;
using System.Transactions;
using static Game;


Config config = new();
GameData gameData = new();
GridManager gridManager = new();
StateMachine stateMachine = new(gridManager);
Renderer renderer = new(gridManager, gameData);
InputManager inputManager = new(gridManager);
Game game = new(stateMachine, inputManager, renderer);
await game.RunGame();


public class Game
{

    InputManager inputManager { get; set; }
    StateMachine stateMachine { get; set; }
        private Renderer renderer {  get; set; }
        const int frameRate = 50;
        public int currentFrame { get; private set; } = 1;
    public Game(StateMachine stateMachine, InputManager inputManager, Renderer renderer)
    {
        this.renderer = renderer;
        this.stateMachine = stateMachine;
        this.inputManager = inputManager;
    }

    public async Task RunGame()
    {
        Task gameTask = Task.Run(() => GameLoop());
        Task inputTask = Task.Run(() => inputManager.InputLoop());
        await Task.WhenAll(inputTask, gameTask);
    }

        private void IncrementFrame()
        {
            currentFrame++;
        }

        public async Task GameLoop()
        {
        while (true)
            {
                ActionQueue.ProcessAction();
                renderer.RenderGameData();
                renderer.RenderGrid();
                //gameManager.StateDecisions(currentFrame);
                stateMachine.Update(currentFrame);
                await Task.Delay(frameRate);
                IncrementFrame();

            }

           
        }

    }
public static class GameEvents
{
 
    // Game state change event
    public static event Action<GameState> OnStateChange;
    public static event Action<int> OnScoreClearRows;
    public static event Action OnShiftDown;
    public static event Action OnClearShape;
    public static event Action<ShapeType> OnCountShape;
    public static event Action<int[]> OnRequestMove;
    public static event Action OnRequestRotate;

    // Methods to raise events
    public static void ChangeState(GameState newState) => OnStateChange?.Invoke(newState);
    public static void ScoreClearRows(int rowsCleared) => OnScoreClearRows?.Invoke(rowsCleared);
    public static void ShiftDown() => OnShiftDown?.Invoke();
    public static void CountShape(ShapeType shapeType) => OnCountShape?.Invoke(shapeType);
    public static void RequestMove(int[] direction) => OnRequestMove?.Invoke(direction);
    public static void RequestRotate() => OnRequestRotate?.Invoke();
    public static void ClearShape() => OnClearShape?.Invoke();
}

public static class ActionQueue
{
    private static ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

    public static void Enqueue(Action action)
    {
        _actions.Enqueue(action);

    }

    public static void ProcessAction()
    {
        while(_actions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }
}

#region State Machine and State Classes
public interface IGameState
{
    void Enter();
    void Update(int currentFrame);
    void Exit();
    void HandleInput();
}


public class StateMachine
{
    public IGameState currentState { get; private set; }
    private readonly Dictionary<GameState,IGameState> states;
    private readonly GridManager gridManager;

    public StateMachine(GridManager gridManager)
    {
        this.gridManager = gridManager;
        GameEvents.OnStateChange += TransitionTo;
        states = new()
        {
            { GameState.Start, new StartState(gridManager) },
            { GameState.Spawn, new SpawnState(gridManager) },
            { GameState.Movement, new MovementState(gridManager) },
            { GameState.Freeze, new FreezeState(gridManager) },
            { GameState.ClearRow, new ClearRowState(gridManager) },
            { GameState.Pause, new PauseState(gridManager) },
            { GameState.End, new EndState(gridManager) }
        };
        currentState = states[GameState.Start];
        currentState.Enter();
    }

    public void TransitionTo(GameState state)
    {
        currentState?.Exit();
        currentState = states[state];
        Renderer.RenderDebug($"Transitioning to {currentState}", 25);
        currentState.Enter();


    }

    public void Update(int currentFrame)
    {
        currentState.Update(currentFrame);
    }
}

public class StartState : IGameState
{
    private readonly GridManager gridManager;
    public StartState(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }
    public void Enter()
    {
 
    }
    public void Update(int currentFrame)


    {
        GameEvents.ChangeState(GameState.Spawn); 
    }

    public void Exit()
    {

    }

    public void HandleInput()
    {

    }
}

public class SpawnState : IGameState
{


    private readonly GridManager gridManager;
    public SpawnState(GridManager gridManager)
    {
        this.gridManager = gridManager;

    }
    public void Enter()
    {
            // refactor
            gridManager.SetSpawnCoordinate(Config.startingCellCoordinate);
            Shape shape = gridManager.NewShape();
            gridManager.SetCurrentShape(shape);
       


    }
    public void Update(int currentFrame)
    {
    
        GameEvents.ChangeState(GameState.Movement);

    }

    public void Exit()
    {
        
    }

    public void HandleInput()
    {

    }
}

public class MovementState : IGameState
{
    private readonly GridManager gridManager;
    public MovementState(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }
    public void Enter()
    {

    }
    public void Update(int currentFrame)
    {

        // refactor
        try
        {
            // remove - just accesses grid manager
        gridManager.DeactivateShapeCells();
        gridManager.SetCurrentShapeCoordinates();

        gridManager.ActivateShapeCells();
            
            
        } catch (Exception ex)
        {
            Renderer.RenderDebug($"movement update: {ex}", 21);
        }

        if (currentFrame % 3 == 0)
        {
            // remove - just accesses grid manager
            ActionQueue.Enqueue(() => GameEvents.RequestMove(Direction.Down.ToArray()));
        }
    }

    public void Exit()
    {

    }

    public void HandleInput()
    {

    }
}
public class FreezeState : IGameState
{
    private readonly GridManager gridManager;
    public FreezeState(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }
    public void Enter()
    {
        try
        {
            // remove - pass score updates as events?
            GameEvents.CountShape(gridManager.currentShape.shapeType);
           

        } catch (Exception ex)
        {
            Renderer.RenderDebug( $"Increment Shape count failed: {ex}", 15);
        }
        // refactor

        GameEvents.ClearShape();
    }
    public void Update(int currentFrame)
    {
        // refactor
        // remove - statemachine updates to be handled via event and will pass in gridmanager
        if (gridManager.CheckClearRow())
        {
            GameEvents.ChangeState(GameState.ClearRow);
           
        }
        else
        {
            GameEvents.ChangeState(GameState.Spawn);
        }
        

    }

    public void Exit()
    {

    }

    public void HandleInput()
    {

    }
}

public class ClearRowState : IGameState
{
    private readonly GridManager gridManager;
    public ClearRowState(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }
    public void Enter()
    {
        // remove rw event
        GameEvents.ScoreClearRows(gridManager.rowsToClear.Count);

        GameEvents.ShiftDown();
    }
    public void Update(int currentFrame)
    {
                                                                                                                                                                                                                       
        // remove rw event
        GameEvents.ChangeState(GameState.Spawn);
    }

    public void Exit()
    {
        // remove - just accesses grid manager
        gridManager.EmptyRowsToClear();
    }

    public void HandleInput()
    {

    }
}

public class PauseState : IGameState
{
    private readonly GridManager gridManager;
    public PauseState(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }
    public void Enter()
    {

    }
    public void Update(int currentFrame)
    {

    }

    public void Exit()
    {

    }

    public void HandleInput()
    {

    }
}
public class EndState : IGameState
{
    private readonly GridManager gridManager;
    public EndState(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }
    public void Enter()
    {

    }
    public void Update(int currentFrame)
    {

    }

    public void Exit()
    {

    }

    public void HandleInput()
    {

    }
}

#endregion


public enum GameState
{
    Start,
    Spawn,
    Movement,
    Freeze,
    ClearRow,
    Pause,
    End
}
public enum ShapeType
{
    I,
    O,
    T,
    S,
    Z,
    J,
    L
}

public enum MoveOption
{
    Move,
    Bounds,
    Freeze
}

public class Config
{
    public const int gridWidth = 10;
    public const int gridHeight = 20;
    public const int renderWidth = 10;
    public const int renderHeight = 20;
    public const int bufferHeight = 129;
    public const int bufferWidth = 129;
    public const int windowWidth = 129;
    public const int windowHeight = 129;
    public static int[] startingCellCoordinate { get; private set; } = { 3, 0 };

    public Config()
    {
        SetConsoleConfig();
    }

    static void SetConsoleConfig()
    {

        CursorVisible = false;
        SetBufferSize(Config.bufferWidth, Config.bufferHeight);
        SetWindowSize(Config.windowWidth, Config.windowHeight);
    }


}

// REFACTOR SHAPES INTO SUBCLASSES
public class Shape
{
    public ConsoleColor shapeColor { get; private set; }
    public int[] coordinates { get; private set; }
    public List<int[]>? coordinateList { get; private set; }
    public Dictionary<int, List<int[]>> coordinateDictionary { get; private set; }
    public ShapeType shapeType { get; private set; }

    public int rotation { get; private set; }


    public Shape(ShapeType input)
    {
        // Populate initial coordinates and color

        shapeType = input;
        rotation = 0;
        ShapeFactory(shapeType);
        coordinateList = new List<int[]>();
        if (coordinateDictionary.TryGetValue(rotation, out List <int[]> output)){

            coordinateList = output;
        }
        else
        {

        }
    }

    private void ShapeFactory(ShapeType shapeType)
    {
        switch (shapeType)
        {
            case ShapeType.I:
                CreateIShape();
                break;

            case ShapeType.O:
                CreateOShape();
                break;

            case ShapeType.T:
                CreateTShape();
                break;

            case ShapeType.S:
                CreateSShape();
                break;

            case ShapeType.Z:
                CreateZShape();
                break;

            case ShapeType.J:
                CreateJShape();
                break;

            case ShapeType.L:
                CreateLShape();
                break;

            default:
                throw new ArgumentException("No ShapeType provided");
                break;
        }


    }

    public void SetRotation(int input)
    {
        if (coordinateDictionary.TryGetValue(input, out List<int[]> output))
        {

            coordinateList = output;
            rotation = input;
        }
        else
        {

        }
    }

    #region Shape Definitions
    private void CreateIShape()
    {
        shapeColor = ConsoleColor.Cyan;


        coordinateDictionary = new()
        {
            { 0, new() { new[] { 0, 1 }, new[] { 1, 1 }, new[] { 2, 1 }, new[] { 3, 1 } } },
            { 1, new() { new[] { 3, 0 }, new[] { 3, 1 }, new[] { 3, 2 }, new[] { 3, 3 } } },
            { 2, new() { new[] { 0, 3 }, new[] { 1, 3 }, new[] { 2, 3 }, new[] { 3, 3 } } },
            { 3, new() { new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 }, new[] { 1, 3 } } }
        };
    }

    private void CreateOShape()
    {
        shapeColor = ConsoleColor.Yellow;
        coordinateDictionary = new()
        {
            { 0, new() { new[] { 1, 0 }, new[] { 2, 0 }, new[] { 1, 1 }, new[] { 2,1 } } },

        };

    }

    private void CreateTShape()
    {
        shapeColor = ConsoleColor.Magenta;
        coordinateDictionary = new()
        {
            { 0, new() { new[] { 1, 0 }, new[] { 0, 1 }, new[] { 1, 1 }, new[] { 2, 1 } } },
            { 1, new() { new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 1 } } },
            { 2, new() { new[] { 0, 1 }, new[] { 1, 1 }, new[] { 2, 1 }, new[] { 1, 2 } } },
            { 3, new() { new[] { 0, 1 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 } } }
        };

    }

    private void CreateSShape()
    {
        shapeColor = ConsoleColor.Green;
        coordinateDictionary = new()
        {
            { 0, new() { new[] { 1, 0 }, new[] { 2, 0 }, new[] { 0, 1 }, new[] { 1, 1 } } },
            { 1, new() { new[] { 1, 0 }, new[] { 1, 1 }, new[] { 2, 1 }, new[] { 2, 2 } } },
            { 2, new() { new[] { 0, 2 }, new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 1 } } },
            { 3, new() { new[] { 0, 0 }, new[] { 0, 1 }, new[] { 1, 1 }, new[] { 1, 2 } } }
        };

    }

    private void CreateZShape()
    {
        shapeColor = ConsoleColor.Red;
        coordinateDictionary = new()
        {
            { 0, new() { new[] { 0, 0 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 2, 1 } } },
            { 1, new() { new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 0 }, new[] { 2, 1 } } },
            { 2, new() { new[] { 0, 1 }, new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 2 } } },
            { 3, new() { new[] { 0, 1 }, new[] { 0, 2 }, new[] { 1, 1 }, new[] { 1, 0 } } }
        };

    }

    private void CreateJShape()
    {
        shapeColor = ConsoleColor.Blue;
        coordinateDictionary = new()
        {
            { 0, new() { new[] { 0, 0 }, new[] { 0, 1 }, new[] { 1, 1 }, new[] { 2, 1 } } },
            { 1, new() { new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 0 } } },
            { 2, new() { new[] { 0, 1 }, new[] { 1, 1 }, new[] { 2, 1 }, new[] { 2, 2 } } },
            { 3, new() { new[] { 0, 2 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 } } }
        };

    }

    private void CreateLShape()
    {
        shapeColor = ConsoleColor.White;
        coordinateDictionary = new()
        {
            { 0, new() { new[] { 0, 1 }, new[] { 1, 1 }, new[] { 2, 1 }, new[] { 2, 0 } } },
            { 1, new() { new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 2 } } },
            { 2, new() { new[] { 0, 1 }, new[] { 0, 2 }, new[] { 1, 1 }, new[] { 2, 1 } } },
            { 3, new() { new[] { 0, 0 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 } } }
        };

    }
#endregion

}
public class GridManager
{
    public int[] spawnCoordinate { get; private set; }
    public Shape? currentShape { get; private set; } = null;

    public List<Cell> shapeCells { get; private set; } = new(); // cells that currently contain a part of the Shape

    public List<Row> rows { get; private set; } = new();

    public List<Row> rowsToClear { get; private set; } = new();
    private List<int[]> currentShapeCoordinates { get; set; }

    private Dictionary<(int, int), Cell> _cells = new(); // for cell lookup



    public GridManager()
    {
        GameEvents.OnRequestMove += Move;
        GameEvents.OnRequestRotate += TryRotateShape;
        GameEvents.OnShiftDown += ShiftDown;
        GameEvents.OnClearShape += ClearCurrentShape;
        CreateCells();
    }

    public void SetSpawnCoordinate(int[] input)
    {
        spawnCoordinate = input;
    }
    public Cell GetCell((int, int) input)
    {
        if (_cells.TryGetValue(input, out Cell value))
        {
            return value;
        }
        else
        {
            throw new Exception($"No cell found at {input}");
        }
    }
    public void ClearCurrentShape()
    {
        currentShape = null;
        shapeCells.Clear();
        currentShapeCoordinates.Clear();
    }
    public void DeactivateShapeCells()
    {
        foreach (Cell cell in shapeCells)
        {
            cell.Deactivate();
        }
    }
    public void SetCurrentShape(Shape shape)
    {
        currentShape = shape;
    }

    public void SetCurrentShapeCoordinates()
    {
        currentShapeCoordinates = new List<int[]>();
        foreach (int[] coordinate in currentShape.coordinateList)
        {
            currentShapeCoordinates.Add(coordinate.Zip(spawnCoordinate, (a, b) => a + b).ToArray());
        }

    }

    public void ActivateShapeCells()
    {

        for (int i = 0; i < currentShapeCoordinates.Count; i++)
        {
            try
            {
                Cell cell = GetCell((currentShapeCoordinates[i][0], currentShapeCoordinates[i][1]));

                cell.Activate(currentShape);
                shapeCells.Add(cell);


            } catch (Exception ex)
            {
                Renderer.RenderDebug($"Exception caught: {ex.Message}", 15);
                throw;
                // throw new Exception($"Cell not found at coordinate {currentShapeCoordinates[i][0]},{currentShapeCoordinates[i][1]}");

            }
        }


    }
    void CreateCells()
    {
        for (int i = 0; i < Config.gridHeight; i++)
        {
            Row row = new Row(i);
            rows.Add(row);
            for (int j = 0; j < Config.gridWidth; j++)
            {
                Cell cell = new(j, i);
                _cells.Add((j, i), cell);
                row.addCell(cell);
            }
        }

    }
    public void Move(int[] direction)
    {

        switch (MoveValidate(direction))
        {
            case MoveOption.Move:



                SetSpawnCoordinate(new int[] { spawnCoordinate[0] + direction[0], spawnCoordinate[1] + direction[1] });
                break;
            case MoveOption.Bounds:

                break;
            case MoveOption.Freeze:
                GameEvents.ChangeState (GameState.Freeze);

                break;
        }
    }
    private MoveOption MoveValidate(int[] direction)
    {
        // Validate if within bounds
        int i = 0;
        //Renderer.RenderDebug("MoveValidate Reached", 11);
        foreach (int[] coordinate in currentShapeCoordinates)
        {
            i++;
            int xResult = coordinate[0] + direction[0];
            int yResult = coordinate[1] + direction[1];


            // Validate that movement is within the grid, other than at the bottom
            if (xResult < 0 || yResult < 0 || xResult == Config.gridWidth)
            {

                return MoveOption.Bounds;
            }
            // If movement is to bottom on grid, set a flag 
            if (yResult == Config.gridHeight)
            {
                return MoveOption.Freeze;
            }

            bool cellMoveValidity = CheckCellMoveValidity(xResult, yResult);


            Cell checkCell = GetCell((xResult, yResult));

            if (!cellMoveValidity)
                //checkCell.HasShape() && checkCell.shape != currentShape)
            {
                if (direction[0] != 0)
                {
                    return MoveOption.Bounds;

                }
                else
                {
                    return MoveOption.Freeze;
                }
            }

        }
        return MoveOption.Move;
    }

    public bool CheckCellMoveValidity(int xInput, int yInput)
    {
        try
        {
            Cell checkCell = GetCell((xInput, yInput));
            if(checkCell.HasShape() && checkCell.shape != currentShape)
            {
                return false;
            }

            return true;

        } catch (Exception ex)
        {

            return false;
        }
    }


    public bool CheckClearRow()
    {

        foreach (Row row in rows)
        {
            int i = 0;
            foreach (Cell cell in row.cells)
            {
                bool check = cell.HasShape();
                if (check)
                {
                    i++;

                }
            }
            if (i == Config.gridWidth)
            {
                rowsToClear.Add(row);
            }
        }

        if (rowsToClear.Count > 0)
        {
            return true;
        }

        return false;
    }
    public void ShiftDown()
    {
      //  for(int i = rowsToClear.Count - 1; i >= 0; i--)
        for (int i = 0; i < rowsToClear.Count; i++)
            {
            Row clearRow = rowsToClear[i];
            int clearRowNumber = clearRow.y;
            
            
       
            for(int j = rows.Count - 1; j >= 0; j--)
            {
                Row row = rows[j];
                if(row.y > clearRow.y)
                {
                    continue;
                } 
                else if(row.y == 0)
                {
                     foreach (Cell cell in row.cells)
                    {
                        cell.Deactivate();
                    }
                }
                else
                {
                    foreach (Cell cell in row.cells)
                    {
                        Cell copyCell = GetCell(((cell.location.Item1), cell.location.Item2 - 1));
                        cell.CopyAttributes(copyCell);
                        copyCell.Deactivate();
                        
                    }
                }

            }
        }
    }
    public void EmptyRowsToClear()
        {
            rowsToClear.Clear();
        }
    
    public void TryRotateShape()
    {
        int findRotation = FindNextValidShapeRotation();

        if (findRotation != -1)
        {
            RotateShape(findRotation);
        }
    }
    public int FindNextValidShapeRotation()
    {
        int rotationCheck = currentShape.rotation; 

        for(int i = 0; i < currentShape.coordinateDictionary.Count; i++)
        {
            if(rotationCheck + 1 == currentShape.coordinateDictionary.Count)
            {
                rotationCheck = 0;
            }
            else
            {
                rotationCheck += 1;
            }

            if(currentShape.coordinateDictionary.TryGetValue(rotationCheck, out List<int[]> coordinates))
            {
                foreach (int[] coordinate in coordinates)
                {
                    int xResult = coordinate[0] + spawnCoordinate[0];
                    int yResult = coordinate[1] + spawnCoordinate[1];
                    bool validMoveCell = CheckCellMoveValidity(xResult, yResult);
                    if (!validMoveCell)
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

    public void RotateShape(int rotation)
    {
        currentShape.SetRotation(rotation);
    }

 
    public Shape NewShape()
    {
        return new Shape((ShapeType)GetRandomShapeType());
        //     return new Shape(ShapeType.S);
    }


    private static int GetRandomShapeType()
    {
        Random rand = new();
        int randomShape = rand.Next(0, Enum.GetValues(typeof(ShapeType)).Length);
        return randomShape;
    }

}

public class Cell
{
    public (int, int) location { get; private set; } // location on grid
    public (int, int)? renderLocation { get; private set; } // location rendered 
    public bool Active { get; private set; } = false;


    public bool renderFlag { get; private set; } = true;
    
    public Shape? shape { get; private set; }


    public static ConsoleColor defaultCellColor = ConsoleColor.Black;
    public ConsoleColor cellColor { get; private set; } = defaultCellColor;

    public static Dictionary<(int, int), Cell> cells = new(); // for cell lookup
    public static List<Cell> startingCells = new(); // Cells selected prior to running game


    public Cell(int x, int y)
    {
        location = (x, y);
        
    }

    // may need to change the active 
    public void Activate(Shape shapeInput)
    {
        Active = true;
        cellColor = shapeInput.shapeColor;
            
        shape = shapeInput;
        SetRenderFlag(true);
    }

    public void Deactivate()
    {
        Active = false;
        ClearCell();
        SetRenderFlag(true);


    }

    public void SetRenderFlag(bool input)
    {
        renderFlag = input;
    }



    public override bool Equals(object obj)
    {
        if (obj is Cell other)
        {
            return this.location == other.location;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return location.GetHashCode();
    }

    public bool HasShape()
    {
        return shape != null;
    }

    public void ClearShape()
    {
        shape = null;

    }

    public void ClearCell()
    {
        ClearShape();
        ResetCellColor();

    }

    private void ResetCellColor()
    {
        cellColor = defaultCellColor;
    }

    public void CopyAttributes(Cell cell)
    {
        shape = cell.shape;
        cellColor = cell.cellColor;
        SetRenderFlag(true);

    }

}

public class Row
{
    public int y { get; private set; }

    public List<Cell> cells;

    public Row(int input)
    {
        y = input;
        cells = new List<Cell>();

    }

    public void addCell(Cell cell)
    {
        cells.Add(cell);
    }

}

public class GameData
{
    public int score { get; private set; } = 0;
    public int level { get; private set; } = 1;
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

    public void ScoreRows(int rows)
    {
        switch (rows)
        {
            case 0:
                break;
            case 1:
                score += 40 * level;
                break;
            case 2:
                score += 100 * level;
                break;
            case 3:
                score += 300 * level;
                break;
            case 4:
                score += 1200 * level;
                break;
            default:
                break;
        }
    }

    public void IncrementShapeCounts(ShapeType input)
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
    public class Renderer
{
   
    readonly GridManager _gridManager;

    readonly GameData gameData;
    public Renderer(GridManager gridManager, GameData gameData)
    {
        _gridManager = gridManager;
        this.gameData = gameData;
    }
    public void RenderGrid()
    {
        for (int h = -1; h < Config.gridHeight; h++)
        {
            for (int w = -1; w < Config.gridWidth; w++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                SetCursor(w + 1, h + 1);

                // handle first cell
                if (w == -1 && h == -1)
                {
                    Write(" ");
                }
                else
                {

                    if (w == -1)
                    {
                        Write((h) % 10);
                    }
                    else if (h == -1)
                    {

                        Write((w) % 10);
                    }
                    else
                    {
                        try
                        {
                            Cell cell = _gridManager.GetCell((w, h));
                            if (cell.renderFlag)
                            {
                                try
                                {
                                    RenderCell(cell); 
                                }
                                catch
                                {

                                    Renderer.RenderDebug($"Cannot render cell {w},{h}", 20);
                                }
                                try
                                {
                                    cell.SetRenderFlag(false);
                                }
                                catch
                                {
                                    Renderer.RenderDebug($"Cannot set render flag {w},{h}", 21);
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                           
                        }

                    }

                }


            }
        }
    }


    public void RenderCell(Cell input)
    {
        // SetCursor(input.location.Item1, input.location.Item2);


    
            Console.BackgroundColor = input.cellColor;
            Console.Write(".");
        
       /* else
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(".");
        } */
    }

    private void SetCursor(int x, int y)
    {
        SetCursorPosition(x, y);
    }

    public static void RenderDebug(string text, int line)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        SetCursorPosition(Config.renderWidth + 5, line);
        Console.Write(text);
    }

    public void RenderControls()
    {
        // Needs refactoring to improve flexibility
        Renderer.RenderDebug("Controls", 0);
        Renderer.RenderDebug("Move selector: WASD", 1);
        Renderer.RenderDebug("Move grid: Arrows", 2);
        Renderer.RenderDebug("Select cell: Spacebar", 3);
        Renderer.RenderDebug("Start/Pause: Enter", 4);
        Renderer.RenderDebug("Soft Reset: R", 5);
        Renderer.RenderDebug("Full Reset: C", 6);
        Renderer.RenderDebug("Quit: Esc", 7);

    }

    public void RenderGameData()
    {
        Renderer.RenderDebug($"Level: {gameData.level}", 0);
        Renderer.RenderDebug($"Score: {gameData.score}", 1);
        Renderer.RenderDebug($"I: {gameData.IShapes}", 2);
        Renderer.RenderDebug($"O: {gameData.OShapes}", 3);
        Renderer.RenderDebug($"T: {gameData.TShapes}", 4);
        Renderer.RenderDebug($"S: {gameData.SShapes}", 5);
        Renderer.RenderDebug($"Z: {gameData.ZShapes}", 6);
        Renderer.RenderDebug($"J: {gameData.JShapes}", 7);
        Renderer.RenderDebug($"L: {gameData.LShapes}", 8);

    }
}
public class InputManager
{
    private GridManager _gridManager { get; set; }
    private DateTime lastMoveTime = DateTime.MinValue;
    private TimeSpan moveCooldown = TimeSpan.FromMilliseconds(75);
    public InputManager(GridManager gridManager)
    {
        _gridManager = gridManager;
    }
    
    public async Task InputLoop()
    {

        while (true)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                ProcessKeyInput(key);
                
            }
                await Task.Delay(15);

        }
    }

    private void ProcessKeyInput(ConsoleKeyInfo key)
    {
        if (DateTime.Now - lastMoveTime < moveCooldown)
            return; // Prevents spamming moves before previous moves finish processing

        lastMoveTime = DateTime.Now; // Reset cooldown timer

        switch (key.Key)
        {
            case ConsoleKey.A:
                ActionQueue.Enqueue(() => GameEvents.RequestMove(Direction.Left.ToArray()));
                break;
            case ConsoleKey.D:
                ActionQueue.Enqueue(() => GameEvents.RequestMove(Direction.Right.ToArray()));
                break;
            case ConsoleKey.S:
                ActionQueue.Enqueue(() => GameEvents.RequestMove(Direction.Down.ToArray()));
                break;
            case ConsoleKey.W:
                ActionQueue.Enqueue(() => GameEvents.RequestRotate());
                break;
            default:
                break;
        }
    }
}



   


public readonly struct Direction
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

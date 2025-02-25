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
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks.Dataflow;
using System.Transactions;
using static Game;


Config config = new();
GameData gameData = new();
Grid grid = new();
RowManager rowManager = new(grid);
ShapeController shapeController = new(grid);
StateMachine stateMachine = new(grid);
Renderer renderer = new(gameData);
InputManager inputManager = new(grid);
Game game = new(stateMachine, inputManager, renderer);
await game.RunGame();


public class Game
{

    InputManager inputManager { get; set; }
    StateMachine stateMachine { get; set; }
        private Renderer renderer {  get; set; }
        const int frameRate = 100;
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
                renderer.RenderGrid();
        while (true)
            {
                ActionQueue.ProcessAction();
                renderer.RenderGameData();
                renderer.RenderCells();
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
    public static event Action OnRequestSpawnShape;
    public static event Action<Cell> OnRequestCellRender;
    public static event Action OnRequestCheckAndClearRows;

    // Methods to raise events
    public static void RequestChangeState(GameState newState) => OnStateChange?.Invoke(newState);
    public static void ScoreClearRows(int rowsCleared) => OnScoreClearRows?.Invoke(rowsCleared);
    public static void ShiftDown() => OnShiftDown?.Invoke();
    public static void CountShape(ShapeType shapeType) => OnCountShape?.Invoke(shapeType);
    public static void RequestMove(int[] direction) => OnRequestMove?.Invoke(direction);
    public static void RequestRotate() => OnRequestRotate?.Invoke();
    public static void ClearShape() => OnClearShape?.Invoke();
    public static void SpawnShape() => OnRequestSpawnShape?.Invoke();
    public static void RequestCellRender(Cell cell) => OnRequestCellRender?.Invoke(cell);
    public static void RequestCheckAndClearRows() => OnRequestCheckAndClearRows?.Invoke();
}

// The ActionQueue implements a ConcurrentQueue to ensure inputs from the InputManager are processed in a threadsafe manner
public static class ActionQueue
{
    private static ConcurrentQueue<(ActionKey actionKey,Action action)> _actions = new();
    private static ConcurrentDictionary<ActionKey, bool> _uniqueActions = new();

    public static void TryEnqueue(ActionKey actionKey, Action action)
    {
        if(_uniqueActions.TryAdd(actionKey, true))
        {
            Enqueue(actionKey,action);
        }
    }

    public static void Enqueue(ActionKey actionKey, Action action)
    {
        _actions.Enqueue((actionKey, action));

    }

    public static void ProcessAction()
    {
        bool moveDownOccurred = false;
        while(_actions.TryDequeue(out var action))
        {
            if(moveDownOccurred && (action.Item1 is ActionKey.Down or ActionKey.Right or ActionKey.Left))
            {
                continue;
            } 

            action.Item2.Invoke();
            
            if(action.Item1 == ActionKey.Down)
            {
                moveDownOccurred = true;    
            }

        }

        ClearDictionary();
    }

    private static void ClearDictionary()
    {
        _uniqueActions.Clear();
    }
}

#region State Machine and State Classes
public interface IGameState
{
    void Enter();
    void Update(int currentFrame);
    void Exit();

}


public class StateMachine
{
    public IGameState currentState { get; private set; }
    private readonly Dictionary<GameState,IGameState> states;

    public StateMachine(Grid gridManager)
    {
        
        GameEvents.OnStateChange += TransitionTo;
        states = new()
        {
            { GameState.Start, new StartState() },
            { GameState.Spawn, new SpawnState() },
            { GameState.Movement, new MovementState() },
            { GameState.Freeze, new FreezeState() },
            { GameState.Pause, new PauseState() },
            { GameState.End, new EndState() },
        };
        currentState = states[GameState.Start];
        currentState.Enter();
    }

    public void TransitionTo(GameState state)
    {
        currentState?.Exit();
        currentState = states[state];
        Renderer.RenderDebug($"Transitioning to {currentState}", 19);
        currentState.Enter();


    }

    public void Update(int currentFrame)
    {
        currentState.Update(currentFrame);
    }
}

public class StartState : IGameState
{

    public StartState()
    {

    }
    public void Enter()
    {
 
    }
    public void Update(int currentFrame)


    {
        GameEvents.RequestChangeState(GameState.Spawn); 
    }

    public void Exit()
    {

    }

}

public class SpawnState : IGameState
{

    public SpawnState()
    {

    }
    public void Enter()
    {
        

    }
    public void Update(int currentFrame)
    {
    
        GameEvents.SpawnShape();
        GameEvents.RequestChangeState(GameState.Movement);

    }

    public void Exit()
    {
        
    }


}

public class MovementState : IGameState
{

    public MovementState()
    {
   
    }
    public void Enter()
    {

    }
    public void Update(int currentFrame)
    {

        if (currentFrame % 4 == 0)
        {
           
            ActionQueue.TryEnqueue(ActionKey.Down, () => GameEvents.RequestMove(Direction.Down.ToArray()));
        }
    }

    public void Exit()
    {

    }


}
public class FreezeState : IGameState
{

    public FreezeState( )
    {
      
    }
    public void Enter()
    {
        
    }
    public void Update(int currentFrame)
    {
        GameEvents.RequestCheckAndClearRows();
    }

    public void Exit()
    {

    }


}



public class PauseState : IGameState
{

    public PauseState( )
    {
        
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

}
public class EndState : IGameState
{

    public EndState()
    {
       
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

}

#endregion

#region Enums
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

public enum ActionKey
{
    Up,
    Down,
    Left,
    Right,
    Rotate,
    Render,
    Count
}

#endregion

public class Config
{
    public const int gridWidth = 10;
    public const int gridHeight = 23;
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

public class ShapeController
{
    private Grid grid;
    public Shape? CurrentShape {  get; private set; }
    // Where the shape initially spawns and then changes rotate coordinates relative to
    private int[] spawnCoordinate;
    private List<Cell> shapeCells;
    public ShapeController(Grid grid)
    {
        this.grid = grid;
        spawnCoordinate = Config.startingCellCoordinate;
        GameEvents.OnRequestMove += TryMove;
        GameEvents.OnRequestRotate += TryRotateShape;
        GameEvents.OnRequestSpawnShape += SpawnShape;
        GameEvents.OnStateChange += ScoreShape;
        GameEvents.OnStateChange += ResetSpawnCoordinate;

    }
    public void SpawnShape()
    {
        SetCurrentShape(NewShape());
        SetShapeCells(CurrentShape);
    }
    public Shape NewShape()
    {
        return new Shape((ShapeType)GetRandomShapeType());
        //     return new Shape(ShapeType.S);
    }
    public void SetCurrentShape(Shape shape) 
    {
        CurrentShape = shape;

    }

    private void ScoreShape(GameState gameState)
    {
        if(gameState == GameState.Freeze)
        {
            ActionQueue.TryEnqueue( ActionKey.Count,() => GameEvents.CountShape(CurrentShape.shapeType));
        }
    }

    private void TryMove(int[] direction)
    {
        List<Cell> oldShapeCells = new(shapeCells);
        foreach(Cell cell in shapeCells)
        {
            int newX = cell.location.Item1 + direction[0];
            int newY = cell.location.Item2 + direction[1];

            Cell? newCell = grid.GetCell((newX,newY));

            if(newX < 0 || newY < 0 || newX == Config.gridWidth || newY == Config.gridHeight || (newCell.HasShape() && newCell.shape != CurrentShape))
            {
                if(direction[0] != 0)
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

        foreach(Cell cell in oldShapeCells)
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
            Cell activateCell = grid.GetCell((newX, newY));
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
            Cell shapeCell = grid.GetCell((coordinate[0] + spawnCoordinate[0], coordinate[1] + spawnCoordinate[1]));
            shapeCells.Add(shapeCell);
            shapeCell.Activate(shape);
            //ActionQueue.TryEnqueue(() => GameEvents.RequestCellRender(shapeCell));
        }
    }

    private static int GetRandomShapeType()
    {
        Random rand = new();
        int randomShape = rand.Next(0, Enum.GetValues(typeof(ShapeType)).Length);
        return randomShape;
    }
    public void TryRotateShape()
    {
        int findRotation = FindNextValidShapeRotation();

        if (findRotation != -1)
        {
            RotateShape(findRotation);
            foreach(Cell cell in shapeCells)
            {
                cell.Deactivate();
            }
            SetShapeCells(CurrentShape); 


        }
    }
    public int FindNextValidShapeRotation()
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

            if (CurrentShape.coordinateDictionary.TryGetValue(rotationCheck, out List<int[]> coordinates))
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
    public void RotateShape(int rotation)
    {
        CurrentShape.SetRotation(rotation);
    }

    public void SetSpawnCoordinate(int[] input)
    {
        spawnCoordinate = input;
    }

    private void ResetSpawnCoordinate(GameState state)
    {
        if(state == GameState.Spawn)
        {
        SetSpawnCoordinate(Config.startingCellCoordinate);

        }
    }

}



public class Grid
{
    public List<Row> rows { get; private set; } = new();
    private Dictionary<(int, int), Cell> cells = new(); // for cell lookup
    public Grid()
    {

        CreateCellsAndRows();
    }
    void CreateCellsAndRows()
    {
        for (int i = 0; i < Config.gridHeight; i++)
        {
            Row row = new Row(i);
            rows.Add(row);
            for (int j = 0; j < Config.gridWidth; j++)
            {
                ConsoleColor defaultCellColor = (i < 3) ? ConsoleColor.Gray : ConsoleColor.Black;
                Cell cell = new(j, i, defaultCellColor); 
                cells.Add((j, i), cell);
                row.addCell(cell);
                cell.SetRenderFlag(true);
            }
        }

    }
    public Cell? GetCell((int, int) input)
    {
        if (cells.TryGetValue(input, out Cell cell))
        {
            return cell;
        }
        else
        {
            return null;
        }
    }
    public bool IsCellOccupied(int x, int y) => cells[(x, y)].HasShape();
}

public class RowManager
{
    private Grid grid;

    public RowManager(Grid grid)
    {
        GameEvents.OnRequestCheckAndClearRows += CheckAndClearRows;
        this.grid = grid;

    }
    public void CheckAndClearRows()
    {
        List<Row> rowsToClear = new List<Row>();
        foreach (Row row in grid.rows)
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
            GameEvents.ScoreClearRows(rowsToClear.Count);
            ShiftDown(rowsToClear);
        }
        GameEvents.RequestChangeState(GameState.Spawn);

        
    }
    public void ShiftDown(List<Row> rows)
    {
      //  for(int i = rowsToClear.Count - 1; i >= 0; i--)
        for (int i = 0; i < rows.Count; i++)
            {
            Row clearRow = rows[i];
            int clearRowNumber = clearRow.y;
            
            for(int j = grid.rows.Count - 1; j >= 0; j--)
            {
                Row row = grid.rows[j];
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
                        Cell copyCell = grid.GetCell(((cell.location.Item1), cell.location.Item2 - 1));
                        cell.CopyAttributes(copyCell);
                        copyCell.Deactivate();
                        
                    }
                }

            }
        }
    }
}
public class Cell
{
    public (int, int) location { get; private set; } // location on grid
    public (int, int)? renderLocation { get; private set; } // location rendered 
    public bool Active { get; private set; } = false;


    public bool renderFlag { get; private set; }
    
    public Shape? shape { get; private set; }


    public ConsoleColor defaultCellColor { get; private set; }
    public ConsoleColor cellColor { get; private set; }


    public Cell(int x, int y, ConsoleColor defaultCellColor)
    {
        location = (x, y);
        this.defaultCellColor = defaultCellColor;
        this.cellColor = defaultCellColor;
        
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

    // refactor to add cell to render to the queue

    public void SetRenderFlag(bool input)
    {
        renderFlag = input;

        if(input)
        {
            ActionQueue.Enqueue(ActionKey.Render,() => GameEvents.RequestCellRender(this));
        }

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

    // refactor
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
   

    readonly GameData gameData;

    private HashSet<Cell> renderCells = new();

    public Renderer(GameData gameData)
    {
        
        this.gameData = gameData;
        GameEvents.OnRequestCellRender += StoreRenderCell;
        // set the cells to render the first time
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
                        Write(".");
                    }

                }

            }

        }
    }
    private void SetCursor(int x, int y)
    {
        SetCursorPosition(x, y);
    }
    void StoreRenderCell(Cell cell) => renderCells.Add(cell);
    public void RenderCells()
    {
        foreach(Cell cell in renderCells)
        {
            RenderCell(cell);
            
        }

        renderCells.Clear();
    }
    private void RenderCell(Cell cell)
    {
            // set cursor with offset to account for border
            SetCursor(cell.location.Item1 + 1, cell.location.Item2 + 1);
            Console.BackgroundColor = cell.cellColor;
            Console.Write(".");
            cell.SetRenderFlag(false);
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
    private Grid _gridManager { get; set; }
    private DateTime lastMoveTime = DateTime.MinValue;
    private TimeSpan moveCooldown = TimeSpan.FromMilliseconds(70);
    public InputManager(Grid gridManager)
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
                await Task.Delay(20);

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
                ActionQueue.TryEnqueue(ActionKey.Left,() => GameEvents.RequestMove(Direction.Left.ToArray()));
                break;
            case ConsoleKey.D:
                ActionQueue.TryEnqueue(ActionKey.Right, () => GameEvents.RequestMove(Direction.Right.ToArray()));
                break;
            case ConsoleKey.S:
                ActionQueue.TryEnqueue(ActionKey.Down, () => GameEvents.RequestMove(Direction.Down.ToArray()));
                break;
            case ConsoleKey.W:
                ActionQueue.TryEnqueue(ActionKey.Rotate, () => GameEvents.RequestRotate());
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

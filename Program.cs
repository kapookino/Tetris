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
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks.Dataflow;
using System.Transactions;

// Initiatilize services and dependencies


// The game itself runs via two Async Tasks, frameManagerTask which refreshes the game and renderer 
// at a set rate and processes actions added to a ConcurrentQueue. inputTask which handles receiving input and translating 
// that input into actions, which are added to the the ConcurrentQueue. The queue ensures all actions
// are processed in the appropriate order with thread safety. 




Game game = new();
await game.RunGame();
// IMPLEMENT A STATE MACHINE AND INTERFACE

public class Game
{
    Config config;
    GameManager gameManager;
    Renderer renderer;
    FrameManager frameManager;
    InputManager inputManager;

    public Game()
    {
        config = new();
        gameManager = new();
        renderer = new(gameManager._gridManager);
        frameManager = new(gameManager, renderer);
        inputManager = new(gameManager, gameManager._gridManager);
    }

    public async Task RunGame()
    {
        Task frameManagerTask = Task.Run(() => frameManager.RunGame());
        Task inputTask = Task.Run(() => inputManager.InputLoop());
        await Task.WhenAll(inputTask, frameManagerTask);
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
    private IGameState _currentState;
    private readonly Dictionary<GameState,IGameState> states;
    private readonly GameManager _gameManager;

    public StateMachine(GameManager gameManager)
    {
        _gameManager = gameManager;
        states = new()
        {
            { GameState.Start, new StartState(gameManager) },
            { GameState.Spawn, new SpawnState(gameManager) },
            { GameState.Movement, new MovementState(gameManager) },
            { GameState.Freeze, new FreezeState(gameManager) },
            { GameState.ClearRow, new ClearRowState(gameManager) },
            { GameState.Pause, new PauseState(gameManager) },
            { GameState.End, new EndState(gameManager) }
        };
        _currentState = states[GameState.Start];
        _currentState.Enter();
    }

    public void TransitionTo(GameState state)
    {
        _currentState?.Exit();
        _currentState = states[state];
        _currentState.Enter();
        _gameManager.SetGameState(state);  

    }

    public void Update(int currentFrame)
    {
        _currentState.Update(currentFrame);
    }
}

public class StartState : IGameState
{
    private readonly GameManager _gameManager;
    public StartState(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    public void Enter()
    {
       // _gameManager._gridManager.SetSpawnCoordinate(Config.startingCellCoordinate);
    }
    public void Update(int currentFrame)
    {
        _gameManager.stateMachine.TransitionTo(GameState.Spawn);
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
    private readonly GameManager _gameManager;
    public SpawnState(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    public void Enter()
    {

        try
        {
            _gameManager._gridManager.SetSpawnCoordinate(Config.startingCellCoordinate);
            Shape shape = _gameManager.NewShape();
            _gameManager._gridManager.SetCurrentShape(shape);
        }
        catch (Exception ex)
        {
            Renderer.RenderDebug($"Error: {ex}", 16);
        }

        _gameManager.stateMachine.TransitionTo(GameState.Movement);
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

public class MovementState : IGameState
{
    private readonly GameManager _gameManager;
    public MovementState(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    public void Enter()
    {

    }
    public void Update(int currentFrame)
    {
        try
        {

        _gameManager._gridManager.DeactivateShapeCells();
        _gameManager._gridManager.SetCurrentShapeCoordinates();
        _gameManager._gridManager.ActivateShapeCells();
        } catch (Exception ex)
        {
            Renderer.RenderDebug($"movement update: {ex}", 21);
        }

        if (currentFrame % 3 == 0)
        {
            _gameManager._gridManager.Move(Direction.Down.ToArray());
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
    private readonly GameManager _gameManager;
    public FreezeState(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    public void Enter()
    {
        _gameManager._gridManager.ClearCurrentShape();
    }
    public void Update(int currentFrame)
    {
        if (currentFrame % 3 == 0) {
            _gameManager.stateMachine.TransitionTo(GameState.Spawn);
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
    private readonly GameManager _gameManager;
    public ClearRowState(GameManager gameManager)
    {
        _gameManager = gameManager;
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

public class PauseState : IGameState
{
    private readonly GameManager _gameManager;
    public PauseState(GameManager gameManager)
    {
        _gameManager = gameManager;
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
    private readonly GameManager _gameManager;
    public EndState(GameManager gameManager)
    {
        _gameManager = gameManager;
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

public class GameManager
{
    public GameState state {  get; private set; }

    public GridManager _gridManager {  get; private set; }
    public StateMachine stateMachine { get; private set; }
  
    private int currentFrame;

    public GameManager()
    {
        _gridManager = new GridManager(this);
        stateMachine = new(this);

    }

    public void SetCurrentFrame(int input)
    {
        currentFrame = input;
    }
    public void SetGameState(GameState stateInput)
    {
        state = stateInput;
    }

    public Shape NewShape()
    {
        Renderer.RenderDebug("NewShape Called", 16);
        return new Shape((ShapeType)GetRandomShapeType());
    }

    private int GetRandomShapeType()
    {
        Random rand = new Random();
        int randomShape = rand.Next(0, Enum.GetValues(typeof(ShapeType)).Length);
        return randomShape;
    }

    private void Freeze()
    {
        // Stop shape
        try
        {
            _gridManager.ClearCurrentShape();
        }
        catch (Exception ex)
        {
            Renderer.RenderDebug($"Error: {ex}", 15);
        }

        SetGameState(GameState.Start);
        // Add points

        // check row clear, change state

    }

    private void RowClear()
    {
        // Row Clear animation
        
        // shift cells

        // add points

        // 
    }

    private void Pause()
    {
        // Unpause

        // New Game
    }

    private void End()
    {

    }


}

// This class 
public class FrameManager
{
    const int frameRate = 60;
    public int currentFrame {  get; private set; } = 1;

    private GameManager _gameManager;

    private Renderer _renderer;
    public FrameManager(GameManager gameManager, Renderer renderer)
    {
        _gameManager = gameManager;
        _renderer = renderer;
    }

    private void IncrementFrame()
    {
        currentFrame++;
    }

    public async Task RunGame()
    {
        while (_gameManager.state != GameState.End)
        {
            _renderer.RenderGrid();
            //_gameManager.StateDecisions(currentFrame);
            _gameManager.stateMachine.Update(currentFrame);
            await Task.Delay(frameRate);
            IncrementFrame();
            Renderer.RenderDebug($"{currentFrame}", 20);
            
        }
    }

}



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
    public const int bufferHeight = 29+100;
    public const int bufferWidth = 29+100;
    public const int windowWidth = 29 + 100;
    public const int windowHeight = 29 + 100;
    public static int[] startingCellCoordinate { get; private set; } = { 2, 0 };

    public Config()
    {
        SetConsoleConfig();
    }

    void SetConsoleConfig()
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
    public int[] pivotCoordinate { get; private set; }
    public List<int[]> coordinateList { get; private set; }
    public List<Cell> cells { get; private set; } 
    public ShapeType shapeType { get; private set; }
    public Shape(ShapeType input)
    {
        // Populate initial coordinates and color
        
        shapeType = input;
        ShapeFactory(shapeType);       
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

    #region Shape Definitions
    private void CreateIShape()
    {
        shapeColor = ConsoleColor.Cyan;
        coordinateList = new()
        {
            new int[] { 0, 0 },
            new int[] { 0, 1 },
            new int[] { 0, 2 },
            new int[] { 0, 3 },
        };
        pivotCoordinate = new int[]{ 0, 1 };
    }

     private void CreateOShape()
    {
        shapeColor = ConsoleColor.Yellow;
        coordinateList = new()
        {
            new int[] { 0, 0 },
            new int[] { 1, 0 },
            new int[] { 0, 1 },
            new int[] { 1, 1 },
        };
        pivotCoordinate = new int[] { 0, 0 };
    }

    private void CreateTShape()
    {
        shapeColor = ConsoleColor.Magenta;
        coordinateList = new()
        {
            new int[] { 0, 0 },
            new int[] { 1, 0 },
            new int[] { 2, 0 },
            new int[] { 1, 1 },
        };
        pivotCoordinate = new int[] { 1, 0 };
    }

    private void CreateSShape()
    {
        shapeColor = ConsoleColor.Green;
        coordinateList = new()
        {
            new int[] { 0, 1 },
            new int[] { 1, 1 },
            new int[] { 1, 0 },
            new int[] { 2, 0 },
        };
        pivotCoordinate = new int[] { 1, 1 };
    }

    private void CreateZShape()
    {
        shapeColor = ConsoleColor.Red;
        coordinateList = new()
        {
            new int[] { 0, 0 },
            new int[] { 1, 0 },
            new int[] { 1, 1 },
            new int[] { 2, 1 },
        };
        pivotCoordinate = new int[] { 1, 1 };
    }

    private void CreateJShape()
    {
        shapeColor = ConsoleColor.Blue;
        coordinateList = new()
        {
            new int[] { 1, 0 },
            new int[] { 1, 1 },
            new int[] { 1, 2 },
            new int[] { 0, 2 },
        };
        pivotCoordinate = new int[] { 1, 2 };
    }

    private void CreateLShape()
    {
        shapeColor = ConsoleColor.White;
        coordinateList = new()
        {
            new int[] { 0, 0 },
            new int[] { 0, 1 },
            new int[] { 0, 2 },
            new int[] { 1, 2 },
        };
        pivotCoordinate = new int[] { 0, 2 };
    }
#endregion

}

public class GridManager
{
    public int[] spawnCoordinate { get; private set; }
    private Shape? currentShape { get; set; } = null;

    public List<Cell> shapeCells { get; private set; } = new(); // cells that currently contain a part of the Shape

    private List<int[]> currentShapeCoordinates { get; set; }

    private Dictionary<(int, int), Cell> _cells = new(); // for cell lookup

    readonly GameManager _gameManager;

    public GridManager(GameManager gameManager)
    {
        _gameManager = gameManager;
        CreateCells();
        Renderer.RenderDebug("GridManager created", 1);

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
        Renderer.RenderDebug($"ActivateShapeCellsCalled", 13);
        for (int i = 0; i < currentShapeCoordinates.Count; i++)
        {
            try
            {
                Cell cell = GetCell((currentShapeCoordinates[i][0], currentShapeCoordinates[i][1]));
                cell.Activate(currentShape);
                shapeCells.Add(cell);
                Renderer.RenderDebug($"shapeCells added", 14);
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
        for (int i = 0; i < Config.gridWidth; i++)
        {
            for (int j = 0; j < Config.gridHeight; j++)
            {
                Cell cell = new(i, j);
                _cells.Add((i, j), cell);
            }
        }
        Renderer.RenderDebug("CreateCells Finished", 5);
    }

    public void Move(int[] direction)
    {

        switch(MoveValidate(direction))
        {
            case MoveOption.Move:


                Renderer.RenderDebug("Move", 12);
                SetSpawnCoordinate(new int[] { spawnCoordinate[0] + direction[0], spawnCoordinate[1] + direction[1] });
            break;
                case MoveOption.Bounds:
                Renderer.RenderDebug("Bounds", 12);
                break;
                case MoveOption.Freeze:
                _gameManager.stateMachine.TransitionTo(GameState.Freeze);
                Renderer.RenderDebug("Freeze", 12);
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
                Renderer.RenderDebug($"Result: {xResult}, {yResult} ", 6+i);

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
                
                Cell checkCell = GetCell((xResult, yResult));

                if (checkCell.Active && checkCell.shape != currentShape)
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
}

public class Cell
{
    public (int, int) location { get; private set; } // location on grid
    public (int, int)? renderLocation { get; private set; } // location rendered 
    public bool Active { get; private set; } = false;

    public bool renderFlag { get; private set; } = true;
    
    // REFACTOR TO REFERENCE AN EXISTING SHAPE, then add to shapes the ability to equate shapes
  

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
        cellColor = defaultCellColor;
        shape = null;
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

}

public class Renderer
{
   
    readonly GridManager _gridManager;
    public Renderer(GridManager gridManager)
    {
        _gridManager = gridManager;
     
    }
    public void RenderGrid()
    {
        for (int h = -1; h < Config.renderHeight; h++)
        {
            for (int w = -1; w < Config.renderWidth; w++)
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
                                RenderCell(cell);
                                cell.SetRenderFlag(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Renderer.RenderDebug($"Cannot render cell {w},{h}",20);
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
}
public class InputManager
{
    private GameManager _gameManager { get; set; }
    private GridManager _gridManager { get; set; }  
    public InputManager(GameManager gameManager, GridManager gridManager)
    {
        _gameManager = gameManager;
        _gridManager = gridManager;
    }
    
    public async Task InputLoop()
    {
        Renderer.RenderDebug("InputLoop started", 2);
        while (_gameManager.state != GameState.End)
        {
            Task handleInputTask = ProcessInput();

            try
            {
                await handleInputTask;
            }
            catch (Exception ex)
            {
                Renderer.RenderDebug($"Exception caught: {ex.Message}", 15);
            }
        }
    }

    private async Task ProcessInput()
    {
        Renderer.RenderDebug("Processinput started", 3);
        try
        {
            ConsoleKeyInfo key = await GetInput();

            KeyActionManager keyActionManager = new(_gameManager, _gridManager, key);
            Renderer.RenderDebug("Past keyactionmanager", 5);
          
            // TEST CALLS
        //    _gridManager.SetCurrentShapeCoordinates();
          //  _gridManager.ActivateShapeCells();
            //Renderer.RenderGrid();
        }
        catch (Exception ex)
        {
            Renderer.RenderDebug($"Exception caught: {ex.Message}", 15);
        }


    }

    private async Task<ConsoleKeyInfo> GetInput()
    {
        Renderer.RenderDebug("GetInput started", 4);
        try
        {
            return await Task.Run(() => Console.ReadKey(intercept: true));

        }
        catch (Exception ex)
        {
            Renderer.RenderDebug($"Exception caught: {ex.Message}", 15);
            throw;
        }

    }

    public class KeyActionManager
    {
        private GameManager _gameManager { get; set; }
        private GridManager _gridManager { get; set; }
        public KeyActionManager(GameManager gameManager, GridManager gridManager, ConsoleKeyInfo key)
        {
            _gameManager = gameManager;
            _gridManager = gridManager;
            ProcessKeyInput(key);
        }


        public void ProcessKeyInput(ConsoleKeyInfo key)
        {
            switch(key.Key)
            {
                case ConsoleKey.Enter:
                    EnterKey();
                    break;
                    case ConsoleKey.D:
                    DKey();
                    break;
                    case ConsoleKey.A:
                        AKey(); 
                    break;
                case ConsoleKey.S:
               //     SKey();
                    break;
                default:
                    break;
            }

        }
        public void EnterKey()
        {
            switch (_gameManager.state)
            {
                case GameState.Spawn:
                    //Renderer.SetStartingOffset();
                    _gameManager.SetGameState(GameState.Movement);
                    break;
                case GameState.Movement:
                    _gameManager.SetGameState(GameState.Pause);
                    break;
                case GameState.Pause:
                    _gameManager.SetGameState(GameState.Movement);
                    break;
            }
        }

        public void RKey()
        {
         //   _gridManager.MoveRight();
        }

        public void DKey()
        {
            Renderer.RenderDebug("D Key input", 17);
            _gridManager.Move(Direction.Right.ToArray());
        }

        public void AKey()
        {
            Renderer.RenderDebug("A Key input", 17);
            _gridManager.Move(Direction.Left.ToArray());
        }

        public void SKey()
        {
            _gridManager.Move(Direction.Down.ToArray());
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

﻿/*
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

using System.Threading.Tasks.Dataflow;

// Initiate 
Config config = new();
SetConsoleConfig();
GridManager grid = new();
Renderer.RenderGrid();
grid.SetCurrentShape(NewShape());
while (true)
{
grid.SetCurrentShapeCoordinates();
grid.ActivateShapeCells();
Renderer.RenderGrid();
ConsoleKeyInfo key = Console.ReadKey(intercept: true);
    grid.MoveDown();
}


void SetConsoleConfig()
{
    
    CursorVisible = false;
    SetBufferSize(Config.bufferWidth, Config.bufferHeight);
    SetWindowSize(Config.windowWidth, Config.windowHeight);
}

Shape NewShape()
{
    return new Shape((ShapeType)GetRandomShapeType());
}

int GetRandomShapeType()
{
Random rand = new Random();
    int randomShape = rand.Next(0,Enum.GetValues(typeof(ShapeType)).Length);
    return randomShape;
}
public class GameManager
{
    public GameState state {  get; private set; }
    public GameManager()
    {
        state = GameState.Start;
    }
    public void SetGameState(GameState stateInput)
    {
        state = stateInput;
    }
}

public enum GameState
{
    Start,
    Select,
    Run,
    ClearRow,
    Pause,
    Reset,
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




}
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

class GridManager
{
    public int[] spawnCoordinate { get; private set; } = Config.startingCellCoordinate;
    private Shape currentShape { get; set; }

    public List<Cell> shapeCells { get; private set; } = new(); // cells that currently contain a part of the Shape


    private List<int[]> currentShapeCoordinates { get; set; }
    public GridManager()
    {
        CreateCells();
       // Cell.IdentifyNeighbors();
    }

    private void DeactivateShapeCells()
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
        foreach(int[] coordinate in currentShape.coordinateList)
        {
            currentShapeCoordinates.Add(coordinate.Zip(spawnCoordinate,(a, b) => a + b).ToArray());
        }
        
    }

    public void ActivateShapeCells()
    {
        for(int i = 0; i < currentShapeCoordinates.Count; i++)
        {
          
            if (Cell.cells.TryGetValue((currentShapeCoordinates[i][0], currentShapeCoordinates[i][1]), out Cell cell))
                {
                    cell.Activate(currentShape.shapeColor);
                    shapeCells.Add(cell);
                }
            else
            {
                throw new Exception($"Cell not found at coordinate {currentShapeCoordinates[i][0]},{currentShapeCoordinates[i][1]}");
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
                Cell.cells.Add((i, j), cell);
            }
        }
    }

    public void MoveRight()
    {
        spawnCoordinate[0] = spawnCoordinate[0] + 1;
        DeactivateShapeCells();
    }

    public void MoveLeft()
    {
        spawnCoordinate[0] = spawnCoordinate[0] - 1;
        DeactivateShapeCells();
    }

    public void MoveDown()
    {
        spawnCoordinate[1] = spawnCoordinate[1] + 1;
        DeactivateShapeCells();
    }
}

public class Cell
{
    public (int, int) location { get; private set; } // location on grid
    public (int, int)? renderLocation { get; private set; } // location rendered 
    public bool Active { get; private set; } = false;
    public bool hasSelector { get; private set; } = false;
    public static ConsoleColor defaultCellColor = ConsoleColor.Black;
    public ConsoleColor cellColor { get; private set; } = defaultCellColor;

    public static Dictionary<(int, int), Cell> cells = new(); // for cell lookup
    public static List<Cell> startingCells = new(); // Cells selected prior to running game

    private List<Cell> _neighbors = new List<Cell>();


    public Cell(int x, int y)
    {
        location = (x, y);
    }

    // may need to change the active 
    public void Activate(ConsoleColor input = ConsoleColor.Black)
    {
        Active = true;
        cellColor = input;
    }

    public void Deactivate()
    {
        Active = false;

    }

    public static Cell GetCell((int, int) input)
    {


        if (Cell.cells.TryGetValue(input, out Cell value))
        {
            return value;
        }
        else
        {
            throw new Exception($"No cell found at {input}");
        }
    }
   

    public static void ResetCells()
    {

        foreach (Cell cell in Cell.cells.Values)
        {
            if (Cell.startingCells.Contains(cell))
            {
                cell.Activate();
            }
            else
            {
                cell.Deactivate();
            }

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

}

static class Renderer
{

    public static (int, int) renderStartLocation { get; private set; } = (0, 0);

    public static void RenderGrid()
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
                        RenderCell(Cell.GetCell((w, h)));
                    }

                }


            }
        }
    }


    public static void RenderCell(Cell input)
    {
        // SetCursor(input.location.Item1, input.location.Item2);


        if (input.Active)
        {
            Console.BackgroundColor = input.cellColor;
            Console.Write(" ");
        }
        else
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(".");
        }
    }

    static void SetCursor(int x, int y)
    {
        SetCursorPosition(x, y);
    }

    public static void RenderDebug(string text, int line)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        SetCursor(Config.renderWidth + 5, line);
        Console.Write(text);
    }

    public static void RenderControls()
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
    public InputManager(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
    
    public async Task HandleInputLoop()
    {

        while (_gameManager.state != GameState.End)
        {
            Task handleInputTask = HandleInput();

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

    private async Task HandleInput()
    {
        try
        {
            ConsoleKeyInfo key = await GetInput();


            if (inputDictionary.TryGetValue(key.Key, out var action))
            {
                action();

            }
            else
            {
                // Do nothing on invalid input
            }
        }
        catch (Exception ex)
        {
            Renderer.RenderDebug($"Exception caught: {ex.Message}", 15);
        }


    }

    private async Task<ConsoleKeyInfo> GetInput()
    {
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


    // Refactor to move all items to a KeyActionManager class with methods to assign input versus the dictionary
    private Dictionary<ConsoleKey, Action> inputDictionary = new()
            {
                  /*  { ConsoleKey.W, () => Select.MoveCursor(Direction.up) },
                    { ConsoleKey.A, () => grid.Move(Direction.left) },
                    { ConsoleKey.S, () => grid.Move(Direction.down) },
                    { ConsoleKey.D, () => grid.Move(Direction.right) },
                    { ConsoleKey.Spacebar, () => Select.ChooseCurrentCell() },
                   // { ConsoleKey.Enter, () => KeyActionManager.EnterKey() },
                    { ConsoleKey.R, () => KeyActionManager.RKey()},
                    { ConsoleKey.Escape, () => GameManager.SetGameState(GameState.End) },
                  */
            };

}
public class KeyActionManager
{
    private GameManager _gameManager { get; set; }
    public KeyActionManager(GameManager gameManager)
    {
        _gameManager = gameManager;
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
            default:
                break;
        }

    }
    public void EnterKey()
    {
        switch (_gameManager.state)
        {
            case GameState.Select:
                //Renderer.SetStartingOffset();
                _gameManager.SetGameState(GameState.Run);
                break;
            case GameState.Run:
                _gameManager.SetGameState(GameState.Pause);
                break;
            case GameState.Pause:
                _gameManager.SetGameState(GameState.Run);
                break;
        }
    }

    public void RKey()
    {
        _gameManager.SetGameState(GameState.Reset);
    }

    public void DKey()
    {
        _gameManager.SetGameState(GameState.Reset);
    }
}
    public static class Direction
    {
        public static (int, int) up = (0, -1);
        public static (int, int) down = (0, 1);
        public static (int, int) left = (-1, 0);
        public static (int, int) right = (1, 0);
    }
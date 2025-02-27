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
using Tetris.Core;
using Tetris.States;
using Tetris.Domain;
using Tetris.Render;

Config config = new();
GameData gameData = new();
Renderer renderer = new(gameData);
Grid grid = new();
RowManager rowManager = new(grid);
ShapeController shapeController = new(grid);
StateMachine stateMachine = new();
InputManager inputManager = new();
Game game = new(stateMachine, inputManager);
await game.RunGame();





#region Enums
public enum GameState
{
    Start,
    Spawn,
    Movement,
    Freeze,
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

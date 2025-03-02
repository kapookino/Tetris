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
using Tetris.Common;
using Tetris;

CursorVisible = false;
SetBufferSize(Config.bufferWidth, Config.bufferHeight);
SetWindowSize(Config.windowWidth, Config.windowHeight);

Logger logger = new();
GameData gameData = new();
Renderer renderer = new(gameData);
Grid grid = new();
RowManager rowManager = new(grid);
MovementHandler movementHandler = new();
ShapeController shapeController = new(grid, movementHandler);
StateMachine stateMachine = new();
InputManager inputManager = new();
Game game = new(stateMachine, inputManager);
await game.RunGame();

public static class Config
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

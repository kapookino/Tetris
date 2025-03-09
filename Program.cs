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

CursorVisible = false;
SetBufferSize(Config.bufferWidth, Config.bufferHeight);
SetWindowSize(Config.windowWidth, Config.windowHeight);


// Need to set up DI container but whatever

Logger logger = new();
GameData gameData = new();
Renderer renderer = new(gameData);
Grid grid = new();
RowManager rowManager = new(grid);
MovementHandler movementHandler = new();
ShapeBag shapeBag = new();
StateMachine stateMachine = new();
InputManager inputManager = new();
EnterController enterController = new();
ShapeController shapeController = new(grid, movementHandler, shapeBag);
Game game = new(stateMachine, inputManager);

await game.RunGame();


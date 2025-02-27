using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Domain;
using Tetris.States;

namespace Tetris.Core
{
    internal static class GameEvents
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
        public static event Action OnRequestRenderCells;
        public static event Action OnRequestRenderGameData;
        public static event Action OnRequestRenderGrid;


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
        public static void RequestRenderCells() => OnRequestRenderCells?.Invoke();
        public static void RequestRenderGameData() => OnRequestRenderGameData?.Invoke();
        public static void RequestRenderGrid() => OnRequestRenderGrid?.Invoke();
    }
}

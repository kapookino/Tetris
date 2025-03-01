using System;
using Tetris.Common;
using Tetris.Domain;
using Tetris.States;

namespace Tetris.Core
{
    internal static class GameEvents
    {
        #region Game State Events
        public static event Action<GameState> OnStateChange;
        public static event Action OnRequestPause;
        #endregion

        #region Scoring Events
        public static event Action<int> OnScoreClearRows;
        public static event Action<ShapeType> OnCountShape;
        #endregion

        #region Shape Movement Events
        public static event Action OnShiftDown;
        public static event Action OnClearShape;
        public static event Action<int[]> OnRequestMove;
        public static event Action OnRequestRotate;
        public static event Action OnRequestSpawnShape;
        public static event Action OnRequestDrop;
        #endregion

        #region Rendering Events
        public static event Action<Cell> OnRequestCellRender;
        public static event Action OnRequestRenderCells;
        public static event Action OnRequestRenderGameData;
        public static event Action OnRequestRenderGrid;
        public static event Action OnRequestNextShapeDisplay;
        #endregion

        #region Game Logic Events
        public static event Action OnRequestCheckAndClearRows;
        #endregion

        #region Game State Methods
        public static void RequestChangeState(GameState newState) => OnStateChange?.Invoke(newState);
        public static void RequestPause() => OnRequestPause?.Invoke();
        #endregion

        #region Scoring Methods
        public static void ScoreClearRows(int rowsCleared) => OnScoreClearRows?.Invoke(rowsCleared);
        public static void CountShape(ShapeType shapeType) => OnCountShape?.Invoke(shapeType);
        #endregion

        #region Shape Movement Methods
        public static void ShiftDown() => OnShiftDown?.Invoke();
        public static void RequestMove(int[] direction) => OnRequestMove?.Invoke(direction);
        public static void RequestRotate() => OnRequestRotate?.Invoke();
        public static void ClearShape() => OnClearShape?.Invoke();
        public static void SpawnShape() => OnRequestSpawnShape?.Invoke();
        public static void RequestDrop() => OnRequestDrop?.Invoke();
        #endregion

        #region Rendering Methods
        public static void RequestCellRender(Cell cell) => OnRequestCellRender?.Invoke(cell);
        public static void RequestRenderCells() => OnRequestRenderCells?.Invoke();
        public static void RequestRenderGameData() => OnRequestRenderGameData?.Invoke();
        public static void RequestRenderGrid() => OnRequestRenderGrid?.Invoke();
        public static void RequestNextShapeDisplay() => OnRequestNextShapeDisplay?.Invoke();
        #endregion

        #region Game Logic Methods
        public static void RequestCheckAndClearRows() => OnRequestCheckAndClearRows?.Invoke();
        #endregion
    }
}
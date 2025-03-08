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

        #endregion

        #region Scoring Events
        public static event Action<int> OnScoreClearRows;
 
        #endregion

        #region Shape Movement Events
        public static event Action OnShiftDown;
        public static event Action OnClearShape;
        public static event Action<int[], int[]> OnRequestMove;
        public static event Action OnRequestRotate;
        public static event Action OnRequestSpawnShape;
        public static event Action OnRequestDrop;
        public static event Action OnRequestEnter;
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
        public static event Action<Action> OnRequestTryEnqueue;
        #endregion


        #region Logging Events
        public static event Action<string,string> OnRequestLog;
        #endregion


        #region Game State Methods
        public static void RequestChangeState(GameState newState) => OnStateChange?.Invoke(newState);
        
        #endregion

        #region Scoring Methods
        public static void ScoreClearRows(int rowsCleared) => OnScoreClearRows?.Invoke(rowsCleared);
      
        #endregion

        #region Shape Movement Methods
        public static void ShiftDown() => OnShiftDown?.Invoke();
        public static void RequestMove(int[] direction, int[] newSpawnCoordinate) => OnRequestMove?.Invoke(direction,null);
        public static void RequestRotate() => OnRequestRotate?.Invoke();
        public static void ClearShape() => OnClearShape?.Invoke();
        public static void SpawnShape() => OnRequestSpawnShape?.Invoke();
        public static void RequestDrop() => OnRequestDrop?.Invoke();
        public static void RequestEnter() => OnRequestEnter?.Invoke();

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
        public static void RequestTryEnqueue(Action action) => OnRequestTryEnqueue?.Invoke(action);
        #endregion

        #region Logging Methods

        public static void RequestLog(string detail, string log) => OnRequestLog?.Invoke(detail,log);

        #endregion
    }
}
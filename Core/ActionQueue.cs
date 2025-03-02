using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Common;

namespace Tetris.Core

{
    internal static class ActionQueue
    {
        private static ConcurrentDictionary<ActionKey, bool> _uniqueActions = new();
        private static ConcurrentQueue<(ActionKey actionKey, Action action)> _actions = new();
        private static GameState currentGameState;

        static ActionQueue()
        {
            GameEvents.OnStateChange += SetCurrentState;
        }

        private static void SetCurrentState(GameState gameState)
        {
            currentGameState = gameState;
        }
        public static void TryEnqueue(ActionKey actionKey, Action action)
        {
  
            if (_uniqueActions.TryAdd(actionKey, true) )
            {
                Enqueue(actionKey, action);
            }
            else
            {

            }
        }

        public static void Enqueue(ActionKey actionKey, Action action)
        {
            _actions.Enqueue((actionKey, action));

        }

        public static void ProcessAction()
        {
            bool moveDownOccurred = false;
            while (_actions.TryDequeue(out var action))
            {
                GameEvents.RequestLog($"ActionQueue.ProcessAction()",$"Trying to process {action}");
                // Ensure that a Down movement action prevents further inputs
                if ((moveDownOccurred || currentGameState != GameState.Movement) && (action.Item1 is ActionKey.Down or ActionKey.Right or ActionKey.Left or ActionKey.Drop))
                {
                    GameEvents.RequestLog($"ActionQueue.ProcessAction()", $"{action} skipped");
                    continue;
                }

                GameEvents.RequestLog($"ActionQueue.ProcessAction()", $"{action} invoked");
                action.Item2.Invoke();


                if (action.Item1 == ActionKey.Down)
                {
                    GameEvents.RequestLog($"ActionQueue.ProcessAction()", $"moveDownOccurred = true");
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
}

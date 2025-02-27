using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Core

{
    internal static class ActionQueue
    {
        private static ConcurrentQueue<(ActionKey actionKey, Action action)> _actions = new();
        private static ConcurrentDictionary<ActionKey, bool> _uniqueActions = new();

        public static void TryEnqueue(ActionKey actionKey, Action action)
        {
            if (_uniqueActions.TryAdd(actionKey, true))
            {
                Enqueue(actionKey, action);
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
                if (moveDownOccurred && (action.Item1 is ActionKey.Down or ActionKey.Right or ActionKey.Left))
                {
                    continue;
                }

                action.Item2.Invoke();

                if (action.Item1 == ActionKey.Down)
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

}

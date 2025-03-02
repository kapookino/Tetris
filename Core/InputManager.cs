using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Domain;
using Tetris.Common;


namespace Tetris.Core
{
    internal class InputManager
    {
        private Grid _gridManager { get; set; }
        private DateTime lastMoveTime = DateTime.MinValue;
        private TimeSpan moveCooldown = TimeSpan.FromMilliseconds(70);
        public InputManager()
        {
            
        }

        public async Task InputLoop()
        {

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                    ProcessKeyInput(key);

                }
                await Task.Delay(20);

            }
        }

        private void ProcessKeyInput(ConsoleKeyInfo key)
        {
            if (DateTime.Now - lastMoveTime < moveCooldown)
                return; // Prevents spamming moves before previous moves finish processing

            lastMoveTime = DateTime.Now; // Reset cooldown refreshTimer

            switch (key.Key)
            {
                case ConsoleKey.A:
                    ActionQueue.TryEnqueue(ActionKey.Left, () => GameEvents.RequestMove(Direction.Left.ToArray(),null));
                    break;
                case ConsoleKey.D:
                    ActionQueue.TryEnqueue(ActionKey.Right, () => GameEvents.RequestMove(Direction.Right.ToArray(),null));
                    break;
                case ConsoleKey.S:
                    ActionQueue.TryEnqueue(ActionKey.Down, () => GameEvents.RequestMove(Direction.Down.ToArray(),null));
                    break;
                case ConsoleKey.W:
                    ActionQueue.TryEnqueue(ActionKey.Rotate, () => GameEvents.RequestRotate());
                    break;
                case ConsoleKey.Spacebar:
                    ActionQueue.TryEnqueue(ActionKey.Drop, () => GameEvents.RequestDrop());
                    break;
                default:
                    break;
            }
        }
    }
}

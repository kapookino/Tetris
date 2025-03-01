using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Core;
using Tetris.Common;

namespace Tetris.States
{
    internal class MovementState : IGameState
    {
        
        public MovementState()
        {

        }
        public void Enter()
        {

        }
        public void Update(long currentFrame)
        {

            if (currentFrame % (32 - GameData.level*2) == 0)
            {

                ActionQueue.TryEnqueue(ActionKey.Down, () => GameEvents.RequestMove(Direction.Down.ToArray()));
            }
        }

        public void Exit()
        {

        }
    }
}

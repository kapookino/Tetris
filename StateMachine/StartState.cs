using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Core;
using Tetris.Common;

namespace Tetris.States
{
    internal class StartState : IGameState
    {

        public StartState()
        {

        }
        public void Enter()
        {

        }
        public void Update(long currentFrame)


        {
            GameEvents.RequestChangeState(GameState.Spawn);
        }

        public void Exit()
        {

        }
    }
}
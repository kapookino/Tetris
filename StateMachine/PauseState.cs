using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Core;

namespace Tetris.States
{
    internal class PauseState : IGameState
    {

        public PauseState()
        {

        }
        public void Enter()
        {
            GameEvents.RequestPause();
        }
        public void Update(long currentFrame)
        {

        }

        public void Exit()
        {

        }
    }
}

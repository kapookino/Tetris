using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Core;

namespace Tetris.States
{
    internal class FreezeState : IGameState
    {
        public FreezeState()
        {

        }
        public void Enter()
        {
            GameEvents.RequestCheckAndClearRows();

        }
        public void Update(long currentFrame)
        {
        }

        public void Exit()
        {

        }
    }
}

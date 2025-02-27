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

        }
        public void Update(int currentFrame)
        {
            GameEvents.RequestCheckAndClearRows();
        }

        public void Exit()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Common;
using Tetris.Core;

namespace Tetris.States
{
    internal class FreezeState : IGameState
    {
        long freezeFrame = 0;
        public FreezeState()
        {

        }
        public void Enter()
        {
            GameEvents.RequestCheckAndClearRows();

        }
        public void Update(long currentFrame)
        {
            if(freezeFrame == 0)
            {
                freezeFrame = currentFrame;
            }

          //  if (currentFrame - freezeFrame == 10)
            //{
                GameEvents.RequestChangeState(GameState.Spawn);
            //}
        }

        public void Exit()
        {
            freezeFrame = 0;
        }
    }
}

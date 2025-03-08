using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.States

{
    internal interface IGameState
    {
        void Enter();
        void Update(long currentFrame);
        void Exit();

        

    }
}

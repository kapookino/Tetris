using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.States

{

    // If I were to refactor further, would likely as HandleInput() as an additional
    // method to potentially streamline items. As it stands, some states are basically
    // just holding areas and don't practically use these methods.

    internal interface IGameState
    {
        void Enter();
        void Update(long currentFrame);
        void Exit();

        

    }
}

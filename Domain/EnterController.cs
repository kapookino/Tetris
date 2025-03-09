using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Common;
using Tetris.Core;

namespace Tetris.Domain
{
    
    // What happens when you press "Enter". 
    internal class EnterController
    {
        GameState GameState { get; set; }
        public EnterController()
        {
            GameState = GameState.Start;
            GameEvents.OnRequestEnter += HandleEnter;
            GameEvents.OnStateChange += ChangeState;
        }

        private void HandleEnter()
        {
            switch (GameState)
            {
                case GameState.Start:
                GameEvents.RequestChangeState(GameState.Spawn);
                    break;
                case GameState.Movement:
                    GameEvents.RequestChangeState(GameState.Pause);
                    break;
                    case GameState.Pause:
                    GameEvents.RequestChangeState(GameState.Movement);
                    break;
                default:
                    break;
            }

        }

        private void ChangeState(GameState state)
        {
            GameState = state;
        }

    }
}

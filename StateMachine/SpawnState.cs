﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Core;
using Tetris.Common;

namespace Tetris.States
{
    internal class SpawnState : IGameState
    {
        public SpawnState()
        {

        }
        public void Enter()
        {

        }
        public void Update(long currentFrame)
        {

            GameEvents.RequestLog("SpawnState.Enter()","SpawnState Entered");
                GameEvents.SpawnShape();
                
            
        }

        public void Exit()
        {
            GameEvents.RequestNextShapeDisplay();
        }
    }
}

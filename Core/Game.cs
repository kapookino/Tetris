using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Domain;
using Tetris.States;

namespace Tetris.Core
{
    internal class Game
    {

        InputManager inputManager { get; set; }
        StateMachine stateMachine { get; set; }
        
        const int frameRate = 100;
        public int currentFrame { get; private set; } = 1;
        public Game(StateMachine stateMachine, InputManager inputManager)
        {
            
            this.stateMachine = stateMachine;
            this.inputManager = inputManager;
        }

        public async Task RunGame()
        {
            Task gameTask = Task.Run(() => GameLoop());
            Task inputTask = Task.Run(() => inputManager.InputLoop());
            await Task.WhenAll(inputTask, gameTask);
        }

        private void IncrementFrame()
        {
            currentFrame++;
        }

        public async Task GameLoop()
        {
            GameEvents.RequestRenderGrid();
            while (true)
            {
                ActionQueue.ProcessAction();
                GameEvents.RequestRenderGameData();
                GameEvents.RequestRenderCells();
                stateMachine.Update(currentFrame);
                await Task.Delay(frameRate);
                IncrementFrame();
            }
        }
    }
}

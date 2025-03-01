using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Tetris.Domain;
using Tetris.States;

namespace Tetris.Core
{
    internal class Game
    {

        InputManager inputManager { get; set; }
        StateMachine stateMachine { get; set; }

        private System.Timers.Timer refreshTimer;

        const int refreshRate = 64;
        public int currentFrame { get; private set; } = 1;
        public Game(StateMachine stateMachine, InputManager inputManager)
        {
            
            this.stateMachine = stateMachine;
            this.inputManager = inputManager;

            refreshTimer = new System.Timers.Timer(refreshRate);
            refreshTimer.Elapsed += OnTimedEvent;
            refreshTimer.AutoReset = true;

        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Process Game and Rendering
            ActionQueue.ProcessAction();
            GameEvents.RequestRenderGameData();
            GameEvents.RequestRenderCells();


            // Run Next update
            stateMachine.Update(currentFrame); 
            currentFrame++;
        }

        public async Task RunGame()
        {
            GameEvents.RequestRenderGrid();
            // Task gameTask = Task.Run(() => GameLoop());
            Task inputTask = Task.Run(() => inputManager.InputLoop());

            refreshTimer.Start();

            await inputTask;
        }
                    
    }
}

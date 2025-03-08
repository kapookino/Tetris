using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Core;
using Tetris.States;
using Tetris.Domain;
using Tetris.Render;
using Tetris.Common;

namespace Tetris.States

{
    internal class StateMachine
    {
        public IGameState currentState { get; private set; }
        public readonly Dictionary<GameState, IGameState> states;
        public StateMachine()
        {

            GameEvents.OnStateChange += TransitionTo;
            states = new()
        {
            { GameState.Start, new StartState() },
            { GameState.Spawn, new SpawnState() },
            { GameState.Movement, new MovementState() },
            { GameState.Freeze, new FreezeState() },
            { GameState.Pause, new PauseState() },
            { GameState.End, new EndState() },
        };
            currentState = states[GameState.Start];
            currentState.Enter();
        }

        public void TransitionTo(GameState state)
        {
            GameEvents.RequestLog($"StateMachine.TransitionTo({state})",$"TransitionTo called. CurrentState: {currentState}. Next state: {state}");
            currentState?.Exit();
            currentState = states[state];
            currentState.Enter();


        }

        public void Update(long currentFrame)
        {
            currentState.Update(currentFrame);
        }
    }
}

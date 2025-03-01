using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Common
{
    public enum GameState
    {
        Start,
        Spawn,
        Movement,
        Freeze,
        Pause,
        End
    }
    public enum ShapeType
    {
        I,
        O,
        T,
        S,
        Z,
        J,
        L
    }

    public enum ActionKey
    {
        Up,
        Down,
        Left,
        Right,
        Rotate,
        Render,
        Count,
        Drop
    }
}

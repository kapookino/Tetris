using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Core;
using Tetris.Common;

namespace Tetris.Domain
{
    internal class Cell
    {
        public (int, int) location { get; private set; } // location on grid
        public (int, int)? renderLocation { get; private set; } // location rendered 
        public bool Active { get; private set; } = false;
        public bool Ghost { get; private set; }
        public char icon { get; private set; } = ' ';
        public bool renderFlag { get; private set; }

        public Shape? shape { get; private set; }


        public ConsoleColor defaultCellColor { get; private set; }
        public ConsoleColor cellColor { get; private set; }


        public Cell(int x, int y, ConsoleColor defaultCellColor)
        {
            location = (x, y);
            
            this.defaultCellColor = defaultCellColor;
            this.cellColor = defaultCellColor;

        }

        // may need to change the active 
        public void Activate(Shape shapeInput)
        {
            Active = true;
            cellColor = shapeInput.shapeColor;

            shape = shapeInput;
            SetRenderFlag(true);
        }

        public void Deactivate()
        {
            Active = false;
            ClearCell();
            SetRenderFlag(true);
        }

        public void ActivateGhost() 
        {
            Ghost = true;
            icon = 'O';
            SetRenderFlag(true);
        }

        public void DeactivateGhost()
        {
            Ghost = false;
            icon = ' ';
            SetRenderFlag(true);
        }
        // refactor to add cell to render to the queue

        public void SetRenderFlag(bool input)
        {
            renderFlag = input;

            if (input)
            {
                ActionQueue.Enqueue(ActionKey.Render, () => GameEvents.RequestCellRender(this));
            }

        }
        public bool HasShape()
        {
            return shape != null;
        }
        public void ClearShape()
        {
            shape = null;
        }
        public void ClearCell()
        {
            ClearShape();
            ResetCellColor();
        }

        private void ResetCellColor()
        {
            cellColor = defaultCellColor;
        }

        // refactor
        public void CopyAttributes(Cell cell)
        {
            shape = cell.shape;
            cellColor = cell.cellColor;
            SetRenderFlag(true);
        }
        public override bool Equals(object obj)
        {
            if (obj is Cell other)
            {
                return this.location == other.location;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return location.GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Domain
{
    internal class Row
    {
        public int y { get; private set; }

        public List<Cell> cells;

        public Row(int input)
        {
            y = input;
            cells = new List<Cell>();

        }

        public void addCell(Cell cell)
        {
            cells.Add(cell);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Common;

namespace Tetris.Domain
{
    internal class Grid
    {
        public List<Row> rows { get; private set; } = new();
        private Dictionary<(int, int), Cell> cells = new(); // for cell lookup
        public Grid()
        {

            CreateCellsAndRows();
        }
        private void CreateCellsAndRows()
        {
            for (int i = 0; i < Config.gameHeight; i++)
            {
                Row row = new Row(i);
                rows.Add(row);
                for (int j = 0; j < Config.gameWidth; j++)
                {
                    ConsoleColor defaultCellColor = ConsoleColor.Black;
                    Cell cell = new(j, i, defaultCellColor);
                    cells.Add((j, i), cell);
                  //  if(i < Config.gameWidth)
                 //   {
                    row.addCell(cell);

                 //   }
                    cell.SetRenderFlag(true);
                }
            }

        }
        public Cell? GetCell((int, int) input)
        {
            if (cells.TryGetValue(input, out Cell? cell))
            {
                return cell;
            }
            else
            {
                return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Core;
using Tetris.Common;

namespace Tetris.Domain
{
    internal class RowManager
    {
        private Grid grid;

        public RowManager(Grid grid)
        {
            GameEvents.OnRequestCheckAndClearRows += CheckAndClearRows;
            this.grid = grid;

        }
        private void CheckAndClearRows()
        {
            List<Row> rowsToClear = new List<Row>();
            foreach (Row row in grid.rows)
            {
                int i = 0;
                foreach (Cell cell in row.cells)
                {
                    bool check = cell.HasShape();
                    if (check)
                    {
                        i++;
                    }
                }
                if (i == Config.gameWidth)
                {
                    rowsToClear.Add(row);
                }
            }


            if (rowsToClear.Count > 0)
            {
                GameEvents.ScoreClearRows(rowsToClear.Count);
                ShiftDown(rowsToClear);
            }
        }
        private void ShiftDown(List<Row> rows)
        {
            //  for(int i = rowsToClear.Count - 1; i >= 0; i--)
            for (int i = 0; i < rows.Count; i++)
            {
                Row clearRow = rows[i];
                int clearRowNumber = clearRow.y;

                for (int j = grid.rows.Count - 1; j >= 0; j--)
                {
                    Row row = grid.rows[j];
                    if (row.y > clearRow.y)
                    {
                        continue;
                    }
                    else if (row.y == 0)
                    {
                        foreach (Cell cell in row.cells)
                        {
                            cell.Deactivate();
                        }
                    }
                    else
                    {
                        foreach (Cell? cell in row.cells)
                        {
                            Cell? copyCell = grid.GetCell(((cell.location.Item1), cell.location.Item2 - 1));
                            cell.CopyAttributes(copyCell);
                            copyCell.Deactivate();

                        }
                    }

                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Core;
using Tetris.Domain;
using Tetris.Common;

namespace Tetris.Render
{
    internal class Renderer
    {


        readonly GameData gameData;

        private HashSet<Cell> renderCells = new();

        public Renderer(GameData gameData)
        {

            this.gameData = gameData;
            GameEvents.OnRequestCellRender += StoreRenderCell;
            GameEvents.OnRequestRenderCells += RenderCells;
            GameEvents.OnRequestRenderGameData += RenderGameData;
            GameEvents.OnRequestRenderGrid += RenderGrid;

        }
        public void RenderGrid()
        {
            string tetris = "**TETRIS**";
         
            for (int h = -1; h < Config.gridHeight; h++)

            {
                for (int w = -1; w < Config.gridWidth; w++)

                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    SetCursorPosition(w + 1, h + 1);



                        if (w == -1)
                        {
                            Write('|');
                        }
                        else if (h == -1)
                        {

                            Write(tetris[w] ); 
                        }
                        else
                        {
                            Write(".");
                        }

                    
                    Write('|');
                }

            }
           
            
        }

        void StoreRenderCell(Cell cell) => renderCells.Add(cell);
        public void RenderCells()
        {
            foreach (Cell cell in renderCells)
            {
                RenderCell(cell);
            }
            renderCells.Clear();
        }
        private void RenderCell(Cell cell)
        {
            // set cursor with offset to account for border
            SetCursorPosition(cell.location.Item1 + 1, cell.location.Item2 + 1);
            Console.BackgroundColor = cell.cellColor;
            Console.Write(" ");
            cell.SetRenderFlag(false);
        }
        public static void RenderDebug(string text, int line)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            SetCursorPosition(Config.renderWidth + 5, line);
            Console.Write(text);
        }
        public void RenderControls()
        {
            // Needs refactoring to improve flexibility
            Renderer.RenderDebug("Controls", 0);
            Renderer.RenderDebug("Move selector: WASD", 1);
            Renderer.RenderDebug("Move grid: Arrows", 2);
            Renderer.RenderDebug("Select cell: Spacebar", 3);
            Renderer.RenderDebug("Start/Pause: Enter", 4);
            Renderer.RenderDebug("Soft Reset: R", 5);
            Renderer.RenderDebug("Full Reset: C", 6);
            Renderer.RenderDebug("Quit: Esc", 7);

        }
        public void RenderGameData()
        {
            Renderer.RenderDebug($"Level: {GameData.level}", 0);
            Renderer.RenderDebug($"Score: {gameData.score}", 1);
            Renderer.RenderDebug($"I: {gameData.IShapes}", 2);
            Renderer.RenderDebug($"O: {gameData.OShapes}", 3);
            Renderer.RenderDebug($"T: {gameData.TShapes}", 4);
            Renderer.RenderDebug($"S: {gameData.SShapes}", 5);
            Renderer.RenderDebug($"Z: {gameData.ZShapes}", 6);
            Renderer.RenderDebug($"J: {gameData.JShapes}", 7);
            Renderer.RenderDebug($"L: {gameData.LShapes}", 8);

        }
    }
}

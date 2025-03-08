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
         
            for (int h = -1; h < Config.gameHeight; h++)

            {
                for (int w = -1; w < Config.gameWidth; w++)

                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    SetCursorPosition(w + 1, h + 1);



                        if (w == -1)
                        {
                            Write('|');
                        }
                        else if (h == -1)
                        {

                            Write(tetris[w]); 
                        }
                        else
                        {
                            Write(" ");
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
            Console.Write($"{cell.icon}");
            cell.SetRenderFlag(false);
        }
        public static void RenderDebug(string text, int line)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            SetCursorPosition(Config.renderWidth + 5, line);
            Console.Write(text);
        }
        public static void RenderControls()
        {
            // Needs refactoring to improve flexibility
            Renderer.RenderDebug("Controls", 4);
            Renderer.RenderDebug("Move left: A", 5);
            Renderer.RenderDebug("Move right: D",6);
            Renderer.RenderDebug("Rotate: W", 7);
            Renderer.RenderDebug("Soft Drop: S", 8);
            Renderer.RenderDebug("Hard Drop: Space", 9);


        }
        public void RenderGameData()
        {
            Renderer.RenderDebug($"Level: {GameData.level}", 0);
            Renderer.RenderDebug($"Score: {gameData.score}", 1);
        }
    }
}

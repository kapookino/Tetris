﻿using System;
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

        private GameState gameState;

        public Renderer(GameData gameData)
        {

            this.gameData = gameData;
            gameState = GameState.Start;
            GameEvents.OnStateChange += UpdateState;
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
        public static void RenderText(string text, int line)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            SetCursorPosition(Config.renderWidth + 5, line);
            Console.Write(text);
        }
        public static void RenderControls()
        {
            // Needs refactoring to improve flexibility
            Renderer.RenderText("Controls", 5);
            Renderer.RenderText("Move left: A", 6);
            Renderer.RenderText("Move right: D",7);
            Renderer.RenderText("Rotate: W", 8);
            Renderer.RenderText("Soft Drop: S", 9);
            Renderer.RenderText("Hard Drop: Space", 10);
            Renderer.RenderText("Pause: Enter", 11);


        }
        public void RenderGameData()
        {
            Renderer.RenderText($"Level: {GameData.level}", 0);
            Renderer.RenderText($"Score: {gameData.score}", 1);

            if(gameState == GameState.Start)
            {
                Renderer.RenderText($"Press Enter to Start a New Game", 3);
            } else if (gameState == GameState.Pause)
            {
                Renderer.RenderText($"GAME PAUSED - Press Enter to unpause", 3);
            }
            else
            {
                Renderer.RenderText($"                                    ", 3);
            }

        }

        private void UpdateState(GameState state)
        {
            gameState = state; 
        }
    }
}

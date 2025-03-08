using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Common;

namespace Tetris.Render
{
    internal class RenderGrid : IRenderer
    {
        public RenderGrid() { }

        public void Render()
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
    }
}

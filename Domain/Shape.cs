using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Common;

namespace Tetris.Domain
{
    internal class Shape
    {
        public ConsoleColor shapeColor { get; private set; }

        public List<int[]>? coordinateList { get; private set; }
        public Dictionary<int, List<int[]>> coordinateDictionary { get; private set; }
        public ShapeType shapeType { get; private set; }

        public int rotation { get; private set; }


        public Shape(ShapeType input)
        {
            // Populate initial coordinates and color

            shapeType = input;
            rotation = 0;
            ShapeFactory(shapeType);
            coordinateList = new List<int[]>();
            if (coordinateDictionary.TryGetValue(rotation, out List<int[]> output))
            {

                coordinateList = output;
            }
            else
            {

            }
        }

        private void ShapeFactory(ShapeType shapeType)
        {
            switch (shapeType)
            {
                case ShapeType.I:
                    CreateIShape();
                    break;

                case ShapeType.O:
                    CreateOShape();
                    break;

                case ShapeType.T:
                    CreateTShape();
                    break;

                case ShapeType.S:
                    CreateSShape();
                    break;

                case ShapeType.Z:
                    CreateZShape();
                    break;

                case ShapeType.J:
                    CreateJShape();
                    break;

                case ShapeType.L:
                    CreateLShape();
                    break;

                default:
                    throw new ArgumentException("No ShapeType provided");
                    
            }


        }

        public void SetRotation(int input)
        {
            if (coordinateDictionary.TryGetValue(input, out List<int[]>? output))
            {

                coordinateList = output;
                rotation = input;
            }
            else
            {

            }
        }

        #region Shape Definitions
        private void CreateIShape()
        {
            shapeColor = ConsoleColor.Cyan;


            coordinateDictionary = new()
        {
            { 0, new() { new[] { 0, 1 }, new[] { 1, 1 }, new[] { 2, 1 }, new[] { 3, 1 } } },
            { 1, new() { new[] { 3, 0 }, new[] { 3, 1 }, new[] { 3, 2 }, new[] { 3, 3 } } },
            { 2, new() { new[] { 0, 3 }, new[] { 1, 3 }, new[] { 2, 3 }, new[] { 3, 3 } } },
            { 3, new() { new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 }, new[] { 1, 3 } } }
        };
        }

        private void CreateOShape()
        {
            shapeColor = ConsoleColor.Yellow;
            coordinateDictionary = new()
        {
            { 0, new() { new[] { 1, 0 }, new[] { 2, 0 }, new[] { 1, 1 }, new[] { 2,1 } } },

        };

        }

        private void CreateTShape()
        {
            shapeColor = ConsoleColor.Magenta;
            coordinateDictionary = new()
        {
            { 0, new() { new[] { 1, 0 }, new[] { 0, 1 }, new[] { 1, 1 }, new[] { 2, 1 } } },
            { 1, new() { new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 1 } } },
            { 2, new() { new[] { 0, 1 }, new[] { 1, 1 }, new[] { 2, 1 }, new[] { 1, 2 } } },
            { 3, new() { new[] { 0, 1 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 } } }
        };

        }

        private void CreateSShape()
        {
            shapeColor = ConsoleColor.Green;
            coordinateDictionary = new()
        {
            { 0, new() { new[] { 1, 0 }, new[] { 2, 0 }, new[] { 0, 1 }, new[] { 1, 1 } } },
            { 1, new() { new[] { 1, 0 }, new[] { 1, 1 }, new[] { 2, 1 }, new[] { 2, 2 } } },
            { 2, new() { new[] { 0, 2 }, new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 1 } } },
            { 3, new() { new[] { 0, 0 }, new[] { 0, 1 }, new[] { 1, 1 }, new[] { 1, 2 } } }
        };

        }

        private void CreateZShape()
        {
            shapeColor = ConsoleColor.Red;
            coordinateDictionary = new()
        {
            { 0, new() { new[] { 0, 0 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 2, 1 } } },
            { 1, new() { new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 0 }, new[] { 2, 1 } } },
            { 2, new() { new[] { 0, 1 }, new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 2 } } },
            { 3, new() { new[] { 0, 1 }, new[] { 0, 2 }, new[] { 1, 1 }, new[] { 1, 0 } } }
        };

        }

        private void CreateJShape()
        {
            shapeColor = ConsoleColor.Blue;
            coordinateDictionary = new()
        {
            { 0, new() { new[] { 0, 0 }, new[] { 0, 1 }, new[] { 1, 1 }, new[] { 2, 1 } } },
            { 1, new() { new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 0 } } },
            { 2, new() { new[] { 0, 1 }, new[] { 1, 1 }, new[] { 2, 1 }, new[] { 2, 2 } } },
            { 3, new() { new[] { 0, 2 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 } } }
        };

        }

        private void CreateLShape()
        {
            shapeColor = ConsoleColor.White;
            coordinateDictionary = new()
        {
            { 0, new() { new[] { 0, 1 }, new[] { 1, 1 }, new[] { 2, 1 }, new[] { 2, 0 } } },
            { 1, new() { new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 }, new[] { 2, 2 } } },
            { 2, new() { new[] { 0, 1 }, new[] { 0, 2 }, new[] { 1, 1 }, new[] { 2, 1 } } },
            { 3, new() { new[] { 0, 0 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, 2 } } }
        };

        }
        #endregion

    }
}

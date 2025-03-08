using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Common;

namespace Tetris.Domain
{
    internal class ShapeBag
    {
        private List<ShapeType> shapeBag = new();

        public ShapeBag()
        {
            FillShapeBag();
        }

        private void FillShapeBag()
        {
            foreach (ShapeType shapeType in Enum.GetValues(typeof(ShapeType)))
            {
                shapeBag.Add(shapeType);
                Utils.ShuffleList(shapeBag);
            }
        }

        public Shape NewShape()
        {
            ShapeType shapeType = shapeBag[0];
            Shape shape = new(shapeType);
            EmptyShapeBag(shapeType);
            return shape;
        }
        private void EmptyShapeBag(ShapeType shapeType)
        {
            shapeBag.Remove(shapeType);
            if (shapeBag.Count == 0)
            {
                FillShapeBag();
            }
        }
    }
}

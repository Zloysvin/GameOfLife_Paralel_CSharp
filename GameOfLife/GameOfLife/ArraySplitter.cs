using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class ArraySplitter
    {
        private int height;
        private int width;
        private int parts;

        private int partsPerRow;
        private (int, int)[] partsDimensions;

        private int sizePartY;
        private int sizePartX;

        public ArraySplitter(int height, int width, int parts)
        {
            this.height = height;
            this.width = width;
            this.parts = parts;

            sizePartX = width / parts;

            partsPerRow = parts / 2;
            partsDimensions = new (int, int)[parts];

            InitSplitter();
        }

        private void InitSplitter()
        {
            for (int i = 0; i < parts; i++)
            {
                partsDimensions[i].Item1 = height;
                partsDimensions[i].Item2 = sizePartX;
            }

            partsDimensions[parts - 1].Item2 += (width - sizePartX * parts) / 2;
        }

        public CoordinatesPair GetCoordinatesPair(int i)
        {
            return new CoordinatesPair(i * sizePartX, 0,
                i * sizePartX + partsDimensions[i].Item2,
                partsDimensions[i].Item1);
        }
    }
}

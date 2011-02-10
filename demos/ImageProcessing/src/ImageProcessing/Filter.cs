using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessing
{
    public struct Filter
    {
        
        public readonly double [] Matrix;
        public readonly int Size;
        public readonly int Bias;

        public Filter(double[] matrix, int size = 3, int bias = 0) : this()
        {
            this.Matrix = matrix;
            this.Size = size;
            this.Bias = bias;

            var expectedLength = this.Size * this.Size;

            if (
                this.Size % 2 == 0 ||
                this.Matrix.Length != expectedLength
                )
                throw new ArgumentException("Matrix size is invalid!");

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessing
{
    public static class Filters
    {
        public static Filter Negative
        {
            get
            {
                return new Filter
                (
                    new double[] { -1 }, 1, 255
                );
            }
        }

        public static Filter Edge
        {
            get
            {
                return new Filter
                (
                    new double[] {
                         1,  2,  1,
                         0,  0,  0,
                        -1, -2, -1,
                    },
                    3,
                    0
                );
            }
        }

        public static Filter Sharpen
        {
            get
            {
                return new Filter
                (
                    new double[] {
                        -1, -1, -1,
                        -1,  9, -1,
                        -1, -1, -1
                    },
                    3,
                    0
                );
            }
        }

        public static Filter Blur
        {
            get
            {
                return new Filter
                (
                    new double[] {
                        1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1,
                        1, 1, 1, 1, 1
                    },
                    5,
                    0
                );
            }
        }
    }
}

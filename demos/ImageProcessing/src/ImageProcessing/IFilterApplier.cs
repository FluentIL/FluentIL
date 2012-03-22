using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageProcessing
{
    interface IFilterApplier
    {
        void Apply(Filter filter, Bitmap target);
    }
}

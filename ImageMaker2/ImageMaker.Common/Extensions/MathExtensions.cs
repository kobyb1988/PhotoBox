using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMaker.Common.Extensions
{
    public static class MathExtensions
    {
        public static double TwoDigits(this double source)
        {
            return Math.Round(source, 2);
        }

        public static double ThreeDigits(this double source)
        {
            return Math.Round(source, 3);
        }
    }
}

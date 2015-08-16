using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageMaker.Entities
{
    public enum PatternType : int
    {
        [Description("Simple")]
        Simple = 0,

        [Description("3_in_1")]
        ThreeInOne = 1,

        [Description("Stripe")]
        Stripe = 2
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SandBox
{
    public static class ResourceKeys
    {
        static ResourceKeys()
        {
            SomeBrush = new ComponentResourceKey();
        }
        public static ResourceKey SomeBrush { get; set; }
    }
}

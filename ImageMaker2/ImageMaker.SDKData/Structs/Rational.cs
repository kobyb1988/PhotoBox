using System;
using System.Runtime.InteropServices;

namespace ImageMaker.SDKData.Structs
{
    /// <summary>
    /// TODO - document
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rational
    {
        public int Numerator;
        public UInt32 Denominator;
    }
}

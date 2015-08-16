using System;
using System.Runtime.InteropServices;

namespace EDSDKLib.Structs
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

using System;
using System.Runtime.InteropServices;
using ImageMaker.SDKData.Miscellaneous;

namespace ImageMaker.SDKData.Structs
{
    /// <summary>
    /// TODO - document
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FocusInformation
    {
        public Rectangle ImageRectangle;
        public UInt32 PointNumber;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = GlobalConstants.FOCUS_POINTS_ARRAY_SIZE)]
        public FocusPoint[] FocusPoints;
    }
}

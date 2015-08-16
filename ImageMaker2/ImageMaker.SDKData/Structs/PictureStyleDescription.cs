using System;
using System.Runtime.InteropServices;
using ImageMaker.SDKData.Enums;

namespace ImageMaker.SDKData.Structs
{
    /// <summary>
    /// TODO - document
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PictureStyleDescription
    {
        public int Contrast;
        public UInt32 Sharpness;
        public int Saturation;
        public int ColorTone;
        public MonochromeFilterEffect MonochromeFilterEffect;
        public MonochromeTone MonochromeTone;
    }
}

﻿using System;
using System.Runtime.InteropServices;

namespace EDSDKLib.Structs
{
    /// <summary>
    /// TODO - document
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SaveImageSetting
    {
        public UInt32 JPEGQuality;
        IntPtr ICCProfileStream;
        public UInt32 Reserved;
    }
}

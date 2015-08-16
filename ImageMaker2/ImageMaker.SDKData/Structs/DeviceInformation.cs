using System;
using System.Runtime.InteropServices;
using ImageMaker.SDKData.Miscellaneous;

namespace ImageMaker.SDKData.Structs
{
    /// <summary>
    /// TODO - document
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceInformation
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = GlobalConstants.MAX_NAME)]
        public string PortName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = GlobalConstants.MAX_NAME)]
        public string DeviceDescription;

        public UInt32 DeviceSubType;

        public UInt32 Reserved;
    }
}

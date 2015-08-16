using System;
using System.Runtime.InteropServices;

namespace EDSDKLib.Structs
{
    /// <summary>
    /// Indicates the white balance compensation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WhiteBalanceShift
    {
        Int32 AmberBlueValue;
        Int32 GreenMagentaValue;
    }
}

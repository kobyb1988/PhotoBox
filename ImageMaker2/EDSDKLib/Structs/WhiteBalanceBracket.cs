using System.Runtime.InteropServices;
using EDSDKLib.Enums;

namespace EDSDKLib.Structs
{
    /// <summary>
    /// Indicates the white balance bracket amount.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WhiteBalanceBracket
    {
        BracketMode BracketMode;
        WhiteBalanceShift WhiteBalanceShift;
    }
}

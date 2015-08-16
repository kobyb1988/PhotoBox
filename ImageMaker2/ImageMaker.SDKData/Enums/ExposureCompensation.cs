using System.ComponentModel;

namespace ImageMaker.SDKData.Enums
{
    /// <summary>
    /// Indicates the exposure compensation.
    /// Exposure compensation refers to compensation relative to the position of the standard exposure mark (in the
    /// center of the exposure gauge).
    /// </summary>
    public enum ExposureCompensation : uint
    {
        [Description("+3")]
        Plus_3 = 0x18,
        [Description("+2 2/3")]
        Plus_2__2_3 = 0x15,
        [Description("+2 1/2")]
        Plus_2__1_2 = 0x14,
        [Description("+2 1/3")]
        Plus_2__1_3 = 0x13,
        [Description("+2")]
        Plus_2 = 0x10,
        [Description("+1 2/3")]
        Plus_1__2_3 = 0x0d,
        [Description("+1 1/2")]
        Plus_1__1_2 = 0x0c,
        [Description("+1 1/3")]
        Plus_1__1_3 = 0x0b,
        [Description("+1")]
        Plus_1 = 0x08,
        [Description("+2/3")]
        Plus_2_3 = 0x05,
        [Description("+1/2")]
        Plus_1_2 = 0x04,
        [Description("+1/3")]
        Plus_1_3 = 0x03,
        [Description("0")]
        Zero = 0x00,
        [Description("-1/3")]
        Minus_1_3 = 0xfd,
        [Description("-1/2")]
        Minus_1_2 = 0xfc,
        [Description("-2/3")]
        Minus_2_3 = 0xfb,
        [Description("-1")]
        Minus_1 = 0xf8,
        [Description("-1 1/3")]
        Minus_1__1_3 = 0xf5,
        [Description("-1 1/2")]
        Minus_1__1_2 = 0xf4,
        [Description("-1 2/3")]
        Minus_1__2_3 = 0xf3,
        [Description("-2")]
        Minus_2 = 0xf0,
        [Description("-2 1/3")]
        Minus_2__1_3 = 0xed,
        [Description("-2 1/2")]
        Minus_2__1_2 = 0xec,
        [Description("-2 2/3")]
        Minus_2__2_3 = 0xeb,
        [Description("-3")]
        Minus_3 = 0xe8,

        /// <summary>
        /// Not valid/no settings changes
        /// </summary>
        Unknown = 0xffffffff,
    }
}
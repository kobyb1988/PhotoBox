using System.ComponentModel;

namespace ImageMaker.SDKData.Enums
{
    /// <summary>
    /// Indicates the camera's aperture value.
    /// </summary>
    public enum ApertureValue : uint
    {
        [Description("0")]
        AV_00 = 0x00,
        [Description("1")]
        AV_1 = 0x08,
        [Description("1.1")]
        AV_1_1 = 0x0B,
        [Description("1.2")]
        AV_1_2 = 0x0C,
        [Description("1.2 (1/3)")]
        AV_1_2__1_3 = 0x0D,
        [Description("1.4")]
        AV_1_4 = 0x10,
        [Description("1.6")]
        AV_1_6 = 0x13,
        [Description("1.8")]
        AV_1_8 = 0x14,
        [Description("1.8 (1/3)")]
        AV_1_8__1_3 = 0x15,
        [Description("2")]
        AV_2 = 0x18,
        [Description("2.2")]
        AV_2_2 = 0x1B,
        [Description("2.5")]
        AV_2_5 = 0x1C,
        [Description("2.5 (1/3)")]
        AV_2_5__1_3 = 0x1D,
        [Description("2.8")]
        AV_2_8 = 0x20,
        [Description("3.2")]
        AV_3_2 = 0x23,
        [Description("3.5")]
        AV_3_5 = 0x24,
        [Description("3.5 (1/3)")]
        AV_3_5__1_3 = 0x25,
        [Description("4")]
        AV_4 = 0x28,
        [Description("4.5")]
        AV_4_5 = 0x2B,
        [Description("4.5 (1/3)")]
        AV_4_5__1_3 = 0x2C,
        [Description("5.0")]
        AV_5_0 = 0x2D,
        [Description("5.6")]
        AV_5_6 = 0x30,
        [Description("6.3")]
        AV_6_3 = 0x33,
        [Description("6.7")]
        AV_6_7 = 0x34,
        [Description("7.1")]
        AV_7_1 = 0x35,
        [Description("8")]
        AV_8 = 0x38,
        [Description("9")]
        AV_9 = 0x3B,
        [Description("9.5")]
        AV_9_5 = 0x3C,
        [Description("10")]
        AV_10 = 0x3D,
        [Description("11")]
        AV_11 = 0x40,
        [Description("13 (1/3)")]
        AV_13__1_3 = 0x43,
        [Description("13")]
        AV_13 = 0x44,
        [Description("14")]
        AV_14 = 0x45,
        [Description("16")]
        AV_16 = 0x48,
        [Description("18")]
        AV_18 = 0x4B,
        [Description("19")]
        AV_19 = 0x4C,
        [Description("20")]
        AV_20 = 0x4D,
        [Description("22")]
        AV_22 = 0x50,
        [Description("25")]
        AV_25 = 0x53,
        [Description("27")]
        AV_27 = 0x54,
        [Description("29")]
        AV_29 = 0x55,
        [Description("32")]
        AV_32 = 0x58,
        [Description("36")]
        AV_36 = 0x5B,
        [Description("38")]
        AV_38 = 0x5C,
        [Description("40")]
        AV_40 = 0x5D,
        [Description("45")]
        AV_45 = 0x60,
        [Description("51")]
        AV_51 = 0x63,
        [Description("54")]
        AV_54 = 0x64,
        [Description("57")]
        AV_57 = 0x65,
        [Description("64")]
        AV_64 = 0x68,
        [Description("72")]
        AV_72 = 0x6B,
        [Description("76")]
        AV_76 = 0x6C,
        [Description("80")]
        AV_80 = 0x6D,
        [Description("91")]
        AV_91 = 0x70,

        /// <summary>
        /// Not valid/no settings changes
        /// </summary>
        Unknown = 0xffffffff,
    }
}

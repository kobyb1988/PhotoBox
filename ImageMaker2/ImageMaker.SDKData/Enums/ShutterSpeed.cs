using System.ComponentModel;

namespace ImageMaker.SDKData.Enums
{
    /// <summary>
    /// Indicates the shutter speed.
    /// </summary>
    /// <remarks>Values labeled "__1_3" represent property values when the step set in the Custom Function is 1/3.</remarks>
    public enum ShutterSpeed : uint
    {
        [Description("Bulb")] //todo ??
        Bulb = 0x0c,
        [Description("30\"")]
        TV_30_Seconds = 0x10,
        [Description("25\"")]
        TV_25_Seconds = 0x13,
        [Description("20\"")]
        TV_20_Seconds = 0x14,
        [Description("20\" (1/3)")]
        TV_20_Seconds__1_3 = 0x15,
        [Description("15\"")]
        TV_15_Seconds = 0x18,
        [Description("13\"")]
        TV_13_Seconds = 0x1B,
        [Description("10\"")]
        TV_10_Seconds = 0x1C,
        [Description("10\" (1/3)")]
        TV_10_Seconds__1_3 = 0x1D,
        [Description("8\"")]
        TV_8_Seconds = 0x20,
        [Description("6\" (1/3)")]
        TV_6_Seconds__1_3 = 0x23,
        [Description("6\"")]
        TV_6_Seconds = 0x24,
        [Description("5\"")]
        TV_5_Seconds = 0x25,
        [Description("4\"")]
        TV_4_Seconds = 0x28,
        [Description("3\"2")]
        TV_3_2_Seconds = 0x2B,
        [Description("3\"")]
        TV_3_Seconds = 0x2C,
        [Description("2\"5")]
        TV_2_5_Seconds = 0x2D,
        [Description("2\"")]
        TV_2_Seconds = 0x30,
        [Description("1\"6")]
        TV_1_6_Seconds = 0x33,
        [Description("1\"5")]
        TV_1_5_Seconds = 0x34,
        [Description("1\"3")]
        TV_1_3_Seconds = 0x35,
        [Description("1\"")]
        TV_1_Seconds = 0x38,
        [Description("0\"8")]
        TV_0_8_Seconds = 0x3B,
        [Description("0\"7")]
        TV_0_7_Seconds = 0x3C,
        [Description("0\"6")]
        TV_0_6_Seconds = 0x3D,
        [Description("0\"5")]
        TV_0_5_Seconds = 0x40,
        [Description("0\"4")]
        TV_0_4_Seconds = 0x43,
        [Description("0\"3")]
        TV_0_3_Seconds = 0x44,
        [Description("0\"3 (1/3)")]
        TV_0_3_Seconds__1_3 = 0x45,
        [Description("1/4")]
        TV_4 = 0x48,
        [Description("1/5")]
        TV_5 = 0x4B,
        [Description("1/6")]
        TV_6 = 0x4C,
        [Description("1/6 (1/3)")]
        TV_6__1_3 = 0x4D,
        [Description("1/8")]
        TV_8 = 0x50,
        [Description("1/10 (1/3)")]
        TV_10__1_3 = 0x53,
        [Description("1/10")]
        TV_10 = 0x54,
        [Description("1/25")]
        TV_13 = 0x55,
        [Description("1/30")]
        TV_15 = 0x58,
        [Description("1/40")]
        TV_20__1_3 = 0x5B,
        [Description("1/45")]
        TV_20 = 0x5C,
        [Description("1/50")]
        TV_25 = 0x5D,
        [Description("1/60")]
        TV_30 = 0x60,
        [Description("1/80")]
        TV_40 = 0x63,
        [Description("1/90")]
        TV_45 = 0x64,
        [Description("1/100")]
        TV_50 = 0x65,
        [Description("1/125")]
        TV_60 = 0x68,
        [Description("1/160")]
        TV_80 = 0x6B,
        [Description("1/180")]
        TV_90 = 0x6C,
        [Description("1/200")]
        TV_100 = 0x6D,
        [Description("1/250")]
        TV_125 = 0x70,
        [Description("1/320")]
        TV_160 = 0x73,
        [Description("1/350")]
        TV_180 = 0x74,
        [Description("1/400")]
        TV_200 = 0x75,
        [Description("1/500")]
        TV_250 = 0x78,
        [Description("1/640")]
        TV_320 = 0x7B,
        [Description("1/750")]
        TV_350 = 0x7C,
        [Description("1/800")]
        TV_400 = 0x7D,
        [Description("1/1000")]
        TV_500 = 0x80,
        [Description("1/1250")]
        TV_640 = 0x83,
        [Description("1/1500")]
        TV_750 = 0x84,
        [Description("1/1600")]
        TV_800 = 0x85,
        [Description("1/2000")]
        TV_1000 = 0x88,
        [Description("1/2500")]
        TV_1250 = 0x8B,
        [Description("1/3000")]
        TV_1500 = 0x8C,
        [Description("1/3200")]
        TV_1600 = 0x8D,
        [Description("1/4000")]
        TV_2000 = 0x90,
        [Description("1/5000")]
        TV_2500 = 0x93,
        [Description("1/6400")]
        TV_3000 = 0x94,
        [Description("1/800")]
        TV_3200 = 0x95,
        [Description("")]
        TV_4000 = 0x98,
        [Description("")]
        TV_5000 = 0x9B,
        [Description("")]
        TV_6000 = 0x9C,
        [Description("")]
        TV_6400 = 0x9D,
        [Description("")]
        TV_8000 = 0xA0,

        /// <summary>
        /// Not valid/no settings changes
        /// </summary>
        Unknown = 0xffffffff,
    }
}
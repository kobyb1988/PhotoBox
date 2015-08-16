using System.ComponentModel;

namespace ImageMaker.SDKData.Enums
{
    /// <summary>
    /// Indicates Camera's ISO sensitivity values.
    /// </summary>
    public enum CameraISOSensitivity : uint
    {
        [Description("ISO 6")]
        ISO_6 = 0x00000028,
        [Description("ISO 12")]
        ISO_12 = 0x00000030,
        [Description("ISO 25")]
        ISO_25 = 0x00000038,
        [Description("ISO 50")]
        ISO_50 = 0x00000040,
        [Description("ISO 100")]
        ISO_100 = 0x00000048,
        [Description("ISO 125")]
        ISO_125 = 0x0000004b,
        [Description("ISO 160")]
        ISO_160 = 0x0000004d,
        [Description("ISO 200")]
        ISO_200 = 0x00000050,
        [Description("ISO 250")]
        ISO_250 = 0x00000053,
        [Description("ISO 320")]
        ISO_320 = 0x00000055,
        [Description("ISO 400")]
        ISO_400 = 0x00000058,
        [Description("ISO 500")]
        ISO_500 = 0x0000005b,
        [Description("ISO 640")]
        ISO_640 = 0x0000005d,
        [Description("ISO 800")]
        ISO_800 = 0x00000060,
        [Description("ISO 1000")]
        ISO_1000 = 0x00000063,
        [Description("ISO 1250")]
        ISO_1250 = 0x00000065,
        [Description("ISO 1600")]
        ISO_1600 = 0x00000068,
        [Description("ISO 3200")]
        ISO_3200 = 0x00000070,
        [Description("ISO 6400")]
        ISO_6400 = 0x00000078,
        [Description("ISO 12800")]
        ISO_12800 = 0x00000080,
        [Description("ISO 25600")]
        ISO_25600 = 0x00000088,

        /// <summary>
        /// Not valid/no settings changes
        /// </summary>
        Unknown = 0xffffffff,
    }
}

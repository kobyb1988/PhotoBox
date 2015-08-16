using System.ComponentModel;

namespace ImageMaker.SDKData.Enums
{
    public enum WhiteBalance : int
    {
        ClickingImageCoordinates = -1,
        FromAnotherImage = -2,

        [Description("Авто")]
        Auto = 0,
        [Description("Дневное освещение")]
        Daylight = 1,
        [Description("Облачно")]
        Cloudy = 2,
        [Description("Вольфрам")]
        Tangsten = 3,
        [Description("Флуоресцентный")]
        Fluorescent = 4,
        [Description("Строб")]
        Strobe = 5,
        [Description("Белая бумага")]
        WhitePaper = 6,
        [Description("Тень")]
        Shade = 8,
        [Description("Цветовая температура")]
        ColorTemp = 9,
        PC1 = 10,
        PC2 = 11,
        PC3 = 12,

        Manual2 = 15,
        Manual3 = 16,
        Manual4 = 18,
        Manual5 = 19,

        PC4 = 20,
        PC5 = 21,
    }
}

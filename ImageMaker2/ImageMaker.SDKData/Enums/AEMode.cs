using System.ComponentModel;

namespace ImageMaker.SDKData.Enums
{
    /// <summary>
    /// Indicates settings values of the camera in shooting mode.
    /// </summary>
    /// <remarks></remarks>
    public enum AEMode : uint
    {
        /// <summary>
        /// Program
        /// </summary>
        [Description("Program")]
        Program = 0,

        /// <summary>
        /// Shutter-Speed Priority
        /// </summary>
        [Description("Shutter-Speed Priority")]
        Tv = 1,

        /// <summary>
        /// Aperture Priority
        /// </summary>
        [Description("Aperture Priority")]
        Av = 2,

        /// <summary>
        /// Manual Exposure
        /// </summary>
        [Description("Manual Exposure")]
        Manual = 3,

        /// <summary>
        /// Bulb
        /// </summary>
        /// <remarks>
        /// For some models, the value of the property cannot be retrieved as AEMode. 
        /// In this case, Bulb is retrieved as the value of the shutter speed (Tv) property.
        /// </remarks>
        [Description("Bulb")]
        Bulb = 4,

        /// <summary>
        /// Auto Depth-of-Field
        /// </summary>
        [Description("AutoDepthOfField")]
        AutoDepthOfField = 5,

        /// <summary>
        /// Depth Of Field
        /// </summary>
        [Description("DepthOfField")]
        DepthOfField = 6,

        /// <summary>
        /// Camera settings registered
        /// </summary>
        [Description("Custom")]
        Custom = 7,

        /// <summary>
        /// Lock
        /// </summary>
        [Description("Lock")]
        Lock = 8,

        /// <summary>
        /// Automatic
        /// </summary>
        [Description("Automatic")]
        Automatic = 9,

        /// <summary>
        /// Night Scene Portrait
        /// </summary>
        [Description("NightScenePortrait")]
        NightScenePortrait = 10,

        /// <summary>
        /// Sports
        /// </summary>
        [Description("Sports")]
        Sports = 11,

        /// <summary>
        /// Portrait
        /// </summary>
        [Description("Portrait")]
        Portrait = 12,

        /// <summary>
        /// Landscape
        /// </summary>
        [Description("Landscape")]
        Landscape = 13,

        /// <summary>
        /// Close-Up
        /// </summary>
        [Description("CloseUp")]
        CloseUp = 14,

        /// <summary>
        /// Flash Off
        /// </summary>
        [Description("FlashOff")]
        FlashOff = 15,

        /// <summary>
        /// Creative Auto
        /// </summary>
        [Description("CreativeAuto")]
        CreativeAuto = 19,

        /// <summary>
        /// Photo In Movie (This value is valid for only Image.)
        /// </summary>
        PhotoInMovie = 21,

        /// <summary>
        /// Not valid/no settings changes
        /// </summary>
        Unknown = 0xFFFFFFFF,
    }
}

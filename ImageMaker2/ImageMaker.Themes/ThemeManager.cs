using System.Windows;

namespace ImageMaker.Themes
{
    public class ThemeManager
    {
        public static ResourceKey MainBackgroundColorKey { get { return new ComponentResourceKey(typeof(ThemeManager), "MainBackgroundColorKey"); } }

        public static ResourceKey MainBorderColorKey { get { return new ComponentResourceKey(typeof(ThemeManager), "MainBorderColorKey"); } }

        public static ResourceKey MainForegroundColorKey { get { return new ComponentResourceKey(typeof(ThemeManager), "MainForegroundColorKey"); } }

        public static ResourceKey OtherBackgroundColorKey { get { return new ComponentResourceKey(typeof(ThemeManager), "OtherBackgroundColorKey"); } }

        public static ResourceKey OtherBorderColorKey { get { return new ComponentResourceKey(typeof(ThemeManager), "OtherBorderColorKey"); } }

        public static ResourceKey OtherForegroundColorKey { get { return new ComponentResourceKey(typeof(ThemeManager), "OtherForegroundColorKey"); } }

        public static ResourceKey MainImageKey { get { return new ComponentResourceKey(typeof(ThemeManager), "MainImageKey"); } }

        public static ResourceKey OtherImagesKey { get { return new ComponentResourceKey(typeof(ThemeManager), "OtherImagesKey"); } }
    }
}

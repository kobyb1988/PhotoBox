using System.Windows;

namespace ImageMaker.Themes
{
    public class ThemeManager
    {
        public static ResourceKey MainBackgroundColorKey { get { return new ComponentResourceKey();} }

        public static ResourceKey MainBorderColorKey { get { return new ComponentResourceKey(); } }

        public static ResourceKey MainForegroundColorKey { get { return new ComponentResourceKey(); } }

        public static ResourceKey OtherBackgroundColorKey { get { return new ComponentResourceKey(); } }

        public static ResourceKey OtherBorderColorKey { get { return new ComponentResourceKey(); } }

        public static ResourceKey OtherForegroundColorKey { get { return new ComponentResourceKey(); } }
    }
}

using System.Windows.Media;

namespace ImageMaker.Common.Extensions
{
    public static class ColorExtensions
    {
        public static Color ColorFromString(this string colorStr, Color defaultColor = default(Color))
        {
            Color color;
            var convertFromString = ColorConverter.ConvertFromString(colorStr);
            if (convertFromString != null)
                color = (Color)convertFromString;
            else
            {
                color = defaultColor;
            }

            return color;
        }
    }
}

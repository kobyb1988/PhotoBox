using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ImageMaker.Common.Dto
{
    public class ThemeSettingsDto
    {
        public Color MainBackgroundColor { get; set; }

        public Color MainForegroundColor { get; set; }

        public Color MainBorderColor { get; set; }

        public Color OtherBackgroundColor { get; set; }

        public Color OtherForegroundColor { get; set; }

        public Color OtherBorderColor { get; set; }

        public Color OtherButtonColor { set; get; }

        public Color OtherForegroundButtonColor { set; get; }

        public Color OtherBackgroundCircleColor { set; get; }

        public byte[] BackgroundImage { get; set; }

        public byte[] OtherBackgroundImage { get; set; }
    }
}

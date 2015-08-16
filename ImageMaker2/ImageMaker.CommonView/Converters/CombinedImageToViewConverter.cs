using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Monads;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ImageMaker.CommonView.Helpers;
using ImageMaker.CommonViewModels;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;

namespace ImageMaker.CommonView.Converters
{
    public class CombinedImageToViewConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3)
                return null;

            List<TemplateImageData> mainBuffer = values[0] as List<TemplateImageData>;
            if (mainBuffer == null)
                return null;

            byte[] overlayBuffer = values[1] as byte[];
            byte[] backgroundBuffer = values[2] as byte[];

            ImageSource overlay = overlayBuffer.Return(x => x.ToImage(), null);
            ImageSource background = backgroundBuffer.Return(x => x.ToImage(), null);
            DrawingGroup group = new DrawingGroup();
            
            Pen pen = new Pen() {Brush = Brushes.Black, Thickness = 1};

            int width = (int) Math.Max(Math.Max(overlay.Return(x => x.Width, 0), background.Return(x => x.Width, 0)), 500);
            int height = (int) Math.Max(Math.Max(overlay.Return(x => x.Height, 0), background.Return(x => x.Height, 0)), 500);

            if (background != null)
                group.Children.Add(new ImageDrawing(background, new Rect(0, 0, width, height)));
            else
            {
                group.Children.Add(new GeometryDrawing(Brushes.White, pen, new RectangleGeometry(new Rect(0, 0, width, height))));    
            }

            mainBuffer.ForEach(x => 
                group.Children.Add(new GeometryDrawing(Brushes.Black, pen, new RectangleGeometry(new Rect(x.X * width, x.Y * height, x.Width * width, x.Height * height)))));

            if (overlay != null)
                group.Children.Add(new ImageDrawing(overlay, new Rect(0, 0, width, height)));
            

            return new DrawingImage(group);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

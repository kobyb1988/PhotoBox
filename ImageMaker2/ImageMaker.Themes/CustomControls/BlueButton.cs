using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageMaker.Themes.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ImageMaker.Themes.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ImageMaker.Themes.CustomControls;assembly=ImageMaker.Themes.CustomControls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:BlueButton/>
    ///
    /// </summary>
    public class BlueButton : Button
    {
        static BlueButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlueButton), new FrameworkPropertyMetadata(typeof(BlueButton)));
        }


        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OverlayBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register("OverlayBrush", typeof(Brush), typeof(BlueButton), new PropertyMetadata(Brushes.Transparent));



        public Brush AdvBackground
        {
            get { return (Brush)GetValue(AdvBackgroundProperty); }
            set { SetValue(AdvBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AdvBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AdvBackgroundProperty =
            DependencyProperty.Register("AdvBackground", typeof(Brush), typeof(BlueButton), new PropertyMetadata(Brushes.Transparent));


        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(double), typeof(BlueButton), new PropertyMetadata(0.0));




        public double InnerCornerRadius
        {
            get { return (double)GetValue(InnerCornerRadiusProperty); }
            set { SetValue(InnerCornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InnerCornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InnerCornerRadiusProperty =
            DependencyProperty.Register("InnerCornerRadius", typeof(double), typeof(BlueButton), new PropertyMetadata(0.0));

        
        public Align CornerAlign
        {
            get { return (Align)GetValue(CornerAlignProperty); }
            set { SetValue(CornerAlignProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerAlign.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerAlignProperty =
            DependencyProperty.Register("CornerAlign", typeof(Align), typeof(BlueButton), new PropertyMetadata(Align.BottomLeft | Align.TopLeft | Align.TopRight | Align.BottomRight));



        public bool ShowBackButton
        {
            get { return (bool)GetValue(ShowBackButtonProperty); }
            set { SetValue(ShowBackButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowBackButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowBackButtonProperty =
            DependencyProperty.Register("ShowBackButton", typeof(bool), typeof(BlueButton), new PropertyMetadata(false));




        public bool IsLeftOriented
        {
            get { return (bool)GetValue(IsLeftOrientedProperty); }
            set { SetValue(IsLeftOrientedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLeftOriented.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLeftOrientedProperty =
            DependencyProperty.Register("IsLeftOriented", typeof(bool), typeof(BlueButton), new PropertyMetadata(true));

        

    }
}

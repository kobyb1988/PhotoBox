using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.Common.Annotations;

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
    ///     <MyNamespace:TimeSelector/>
    ///
    /// </summary>
    public class TimeSelector : ComboBox
    {
        static TimeSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSelector), new FrameworkPropertyMetadata(typeof(TimeSelector)));
        }

        public TimeSelector()
        {
            var hours = Enumerable.Range(0, 24).Select(x => new Hour(TimeSpan.FromHours(x))).ToList();
            ItemsSource = hours;
            Time = hours.First();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TimeSelector), new PropertyMetadata(null));

        public Brush OverlayBrush
        {
            get { return (Brush)GetValue(OverlayBrushProperty); }
            set { SetValue(OverlayBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OverlayBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverlayBrushProperty =
            DependencyProperty.Register("OverlayBrush", typeof(Brush), typeof(TimeSelector), new PropertyMetadata(Brushes.Transparent));

        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(double), typeof(TimeSelector), new PropertyMetadata(0.0));



        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleFontSizeProperty =
            DependencyProperty.Register("TitleFontSize", typeof(double), typeof(TimeSelector), new PropertyMetadata(10.0));


        public CornerRadius InnerCornerRadius
        {
            get { return (CornerRadius)GetValue(InnerCornerRadiusProperty); }
            set { SetValue(InnerCornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InnerCornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InnerCornerRadiusProperty =
            DependencyProperty.Register("InnerCornerRadius", typeof(CornerRadius), typeof(TimeSelector), new PropertyMetadata(new CornerRadius(0)));

        public Align CornerAlign
        {
            get { return (Align)GetValue(CornerAlignProperty); }
            set { SetValue(CornerAlignProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerAlign.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerAlignProperty =
            DependencyProperty.Register("CornerAlign", typeof(Align), typeof(TimeSelector), 
            new PropertyMetadata(Align.BottomLeft | Align.TopLeft | Align.TopRight | Align.BottomRight));

      
        public Hour Time
        {
            get { return (Hour)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Time.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(Hour), typeof(TimeSelector), new PropertyMetadata(null));

        private RelayCommand _increaseCommand;
        private RelayCommand _decreaseCommand;


        public RelayCommand IncreaseCommand
        {
            get { return _increaseCommand ?? (_increaseCommand = new RelayCommand(Increase, CanIncrease)); }
        }

        public RelayCommand DecreaseCommand
        {
            get { return _decreaseCommand ?? (_decreaseCommand = new RelayCommand(Decrease, CanDecrease)); }
        }

        private bool CanDecrease()
        {
            return Time != null && Time.CanSubtract();
        }

        private void Decrease()
        {
            Time.SubtractMinute();
        }

        private bool CanIncrease()
        {
            return Time != null && Time.CanAdd();
        }

        private void Increase()
        {
            Time.AddMinute();
        }
    }

    public class TimeSpanToHourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan time = TimeSpan.Zero;
            if (value is TimeSpan)
                time = (TimeSpan) value;

            return new Hour(time);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Hour time = value as Hour;
            if (time == null)
                return TimeSpan.Zero;

            TimeSpan timeSpan;
            TimeSpan.TryParse(time.OriginalTime, out timeSpan);
            return timeSpan;
        }
    }

    public class Hour : INotifyPropertyChanged
    {
        private readonly TimeSpan _time;
        private TimeSpan _currentTime;

        private string _timeString;

        private const string CFormat = @"hh\:mm";

        public Hour(TimeSpan time)
        {
            _time = time;
            _currentTime = time;
            TimeString = OriginalTime = _time.ToString(CFormat);
        }

        public void AddMinute()
        {
            _currentTime = _currentTime.Add(TimeSpan.FromMinutes(1));
            TimeString = _currentTime.ToString(CFormat);
        }

        public bool CanAdd()
        {
            return _currentTime.Hours <= 23 && _currentTime.Minutes < 59;
        }

        public bool CanSubtract()
        {
            return _currentTime.Hours >= 0 && _currentTime.Minutes > 0;
        }

        public void SubtractMinute()
        {
            _currentTime = _currentTime.Subtract(TimeSpan.FromMinutes(1));
            TimeString = _currentTime.ToString(CFormat);
        }

        public string OriginalTime { get; set; }

        public string TimeString
        {
            get { return _timeString; }
            set
            {
                _timeString = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

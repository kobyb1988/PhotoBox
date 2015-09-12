using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Media;
using ImageMaker.Common.Dto;

namespace ImageMaker.Themes
{
    public class ThemeManager
    {
        private static Color _defaultColor;

        private static Color _mainBackgroundColor;
        private static Color _mainForegroundColor;
        private static Color _mainBorderColor;
        private static Color _otherBackgroundColor;
        private static Color _otherForegroundColor;
        private static Color _otherBorderColor;

        static ThemeManager()
        {
            var convertFromString = ColorConverter.ConvertFromString("#f4ad49");
            if (convertFromString != null)
                _defaultColor = (Color) convertFromString;
            else
            {
                _defaultColor = Color.FromRgb(255, 0, 0);
            }
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static void NotifyStaticPropertyChanged(string propertyName)
        {
            if (StaticPropertyChanged != null)
                StaticPropertyChanged(null, new PropertyChangedEventArgs(propertyName));
        }

        private static void NotifyStaticPropertyChanged<T>(Expression<Func<T>> memberExpression)
        {
            string memberName = ((MemberExpression) memberExpression.Body).Member.Name;
            if (StaticPropertyChanged != null)
                StaticPropertyChanged(null, new PropertyChangedEventArgs(memberName));
        }

        public static Color BackgroundColor
        {
            get { return _defaultColor; }
            set
            {
                if (_defaultColor == value)
                    return;

                _defaultColor = value;
                NotifyStaticPropertyChanged("BackgroundColor");
            }
        }

        public static Color MainBackgroundColor
        {
            get { return _mainBackgroundColor; }
            set
            {
                if (_mainBackgroundColor == value)
                    return;

                _mainBackgroundColor = value;
                NotifyStaticPropertyChanged(() => MainBackgroundColor);
            }
        }

        public static Color MainForegroundColor
        {
            get { return _mainForegroundColor; }
            set
            {
                if (_mainForegroundColor == value)
                    return;

                _mainForegroundColor = value;
                NotifyStaticPropertyChanged(() => MainForegroundColor);
            }
        }

        public static Color MainBorderColor
        {
            get { return _mainBorderColor; }
            set
            {
                if (_mainBorderColor == value)
                    return;

                _mainBorderColor = value;
                NotifyStaticPropertyChanged(() => MainBorderColor);
            }
        }

        public static Color OtherBackgroundColor
        {
            get { return _otherBackgroundColor; }
            set
            {
                if (_otherBackgroundColor == value)
                    return;

                _otherBackgroundColor = value;
                NotifyStaticPropertyChanged(() => OtherBackgroundColor);
            }
        }

        public static Color OtherForegroundColor
        {
            get { return _otherForegroundColor; }
            set
            {
                if (_otherForegroundColor == value)
                    return;

                _otherForegroundColor = value;
                NotifyStaticPropertyChanged(() => OtherForegroundColor);
            }
        }

        public static Color OtherBorderColor
        {
            get { return _otherBorderColor; }
            set
            {
                if (_otherBorderColor == value)
                    return;

                _otherBorderColor = value;
                NotifyStaticPropertyChanged(() => OtherBorderColor);
            }
        }

        public static void Change(Color color)
        {
            BackgroundColor = color;
        }

        public static void LoadTheme(ThemeSettingsDto theme)
        {
            MainBackgroundColor = theme.MainBackgroundColor;
            MainBorderColor = theme.MainBorderColor;
            MainForegroundColor = theme.MainForegroundColor;
            OtherBackgroundColor = theme.OtherBackgroundColor;
            OtherBorderColor = theme.OtherBorderColor;
            OtherForegroundColor = theme.OtherForegroundColor;
        }
    }
}

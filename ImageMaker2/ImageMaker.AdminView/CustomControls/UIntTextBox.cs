using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ImageMaker.AdminView.CustomControls
{
    public class UIntTextBox : TextBox
    {
        public uint MaxValue
        {
            get { return (uint)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(uint), typeof(UIntTextBox));

        public uint MinValue
        {
            get { return (uint)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(uint), typeof(UIntTextBox));

        public uint DefValue
        {
            get { return (uint)GetValue(DefValueProperty); }
            set { SetValue(DefValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefValueProperty =
            DependencyProperty.Register("DefValue", typeof(uint), typeof(UIntTextBox));


        static UIntTextBox()
        {
            //TextProperty.OverrideMetadata(typeof(UIntTextBox), new FrameworkPropertyMetadata(
            //    (o, args) => {},
            //    OnValueCoercion));
        }

        private static object OnValueCoercion(DependencyObject dependencyObject, object baseValue)
        {
            UIntTextBox tb = (UIntTextBox) dependencyObject;
            if (baseValue == null)
                return tb.DefValue.ToString(CultureInfo.InvariantCulture);

            double outVal = 0;
            if (!double.TryParse(baseValue.ToString().Replace(".", ","), out outVal) || outVal >= tb.MaxValue || outVal < tb.MinValue)
                return tb.DefValue.ToString(CultureInfo.InvariantCulture);

            return baseValue.ToString().ToString(CultureInfo.InvariantCulture);
        }
    }
}

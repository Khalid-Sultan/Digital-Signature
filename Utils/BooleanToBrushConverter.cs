﻿using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Digital_Signature_Verification.Utils
{
    public class BooleanToBrushConverter : IValueConverter
    {
        private readonly Brush bTrue = new SolidColorBrush(Color.FromArgb(255, 192, 255, 192));
        private readonly Brush bFalse = new SolidColorBrush(Color.FromArgb(255, 255, 192, 192));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                if ((bool)value)
                    return bTrue;
            return bFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Brush)value == bTrue;
        }
    }

}

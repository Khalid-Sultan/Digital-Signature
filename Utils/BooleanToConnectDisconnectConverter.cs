﻿using System;
using System.Windows.Data;

namespace Digital_Signature_Verification.Utils
{
    public class BooleanToConnectDisconnectConverter : IValueConverter
    {
        private const string strTrue = "Disconnect";
        private const string strFalse = "Connect";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                if ((bool)value)
                    return strTrue;
            return strFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString() == strTrue;
        }
    }

}

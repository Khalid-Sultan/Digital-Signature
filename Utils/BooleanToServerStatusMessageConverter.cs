using System;
using System.Windows.Data;

namespace Digital_Signature_Verification.Utils
{
    public class BooleanToServerStatusMessageConverter : IValueConverter
    {
        private const string strTrue = "Server is active";
        private const string strFalse = "Server is stopped";

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

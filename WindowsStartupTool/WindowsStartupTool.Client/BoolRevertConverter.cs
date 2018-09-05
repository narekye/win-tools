using System;
using System.Globalization;
using System.Windows.Data;

namespace WindowsStartupTool.Client
{
    public class BoolRevertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool flag) ? !flag : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

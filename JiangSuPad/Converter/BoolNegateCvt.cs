using System;
using System.Globalization;
using System.Windows.Data;

namespace JiangSuPad.Converter
{
    public class BoolNegateCvt: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = System.Convert.ToBoolean(value);
            return !data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

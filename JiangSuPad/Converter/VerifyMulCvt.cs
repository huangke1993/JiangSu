using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace JiangSuPad.Converter
{
    public class VerifyMulCvt : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.All(System.Convert.ToBoolean)) return true;
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace JiangSuPad.Validation
{
    public class IdNumValidation: ValidationRule
    {
        private const string ErrorMsg = "请填写有效的身份证号码";
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null) return new ValidationResult(false, ErrorMsg);
            return !Regex.IsMatch(value.ToString(), @"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", RegexOptions.IgnoreCase) ? new ValidationResult(false, ErrorMsg) : new ValidationResult(true, null);
        }
    }
}

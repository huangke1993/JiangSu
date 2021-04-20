using System.Globalization;
using System.Windows.Controls;

namespace JiangSuPad.Validation
{
    public class RequiredValidation : ValidationRule
    {
        private const string ErrorMsg = "该字段必填";
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(value==null)return new ValidationResult(false, ErrorMsg);
            if(string.IsNullOrWhiteSpace(value.ToString())) return new ValidationResult(false, ErrorMsg);
            return new ValidationResult(true,string.Empty);
        }
    }
}

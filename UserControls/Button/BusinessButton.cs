using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace UserControls.Button
{
    public class BusinessButton:System.Windows.Controls.Button
    {
        public static readonly DependencyProperty ImgPathProperty;
        public ImageSource ImgPath
        {
            get => (ImageSource)GetValue(ImgPathProperty);
            set => SetValue(ImgPathProperty, value);
        }
        public static readonly DependencyProperty DisableImgPathProperty;
        public ImageSource DisableImgPath
        {
            get => (ImageSource)GetValue(DisableImgPathProperty);
            set => SetValue(DisableImgPathProperty, value);
        }
        static BusinessButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BusinessButton), new FrameworkPropertyMetadata(typeof(BusinessButton)));
            ImgPathProperty = DependencyProperty.Register("ImgPath", typeof(ImageSource), typeof(BusinessButton));
            DisableImgPathProperty = DependencyProperty.Register("DisableImgPath", typeof(ImageSource), typeof(BusinessButton));
            RadiusProperty = DependencyProperty.Register("Radius", typeof(CornerRadius), typeof(BusinessButton));
        }
        public static readonly DependencyProperty RadiusProperty;
        [Bindable(true)]
        [Category("Appearance")]
        public CornerRadius Radius
        {
            get => (CornerRadius)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }
    }
}

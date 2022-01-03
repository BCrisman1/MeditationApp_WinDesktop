using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace EZMedit8.CustomControls
{
    public class CustomStackPanel : StackPanel
    {        
        public static readonly DependencyProperty LgbProperty = DependencyProperty.Register(
          "LinearGradientBrush", typeof(LinearGradientBrush), typeof(CustomStackPanel), new PropertyMetadata(default(StackPanel)));

        public LinearGradientBrush LinearGradientBrush { get => (LinearGradientBrush)GetValue(LgbProperty); set => SetValue(LgbProperty, value); }

    }
}

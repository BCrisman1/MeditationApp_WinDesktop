using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace EZMedit8.Extensions
{
    public static class ButtonExtensions
    {
        public static Binding GetBinding(this Button button, DependencyProperty dependencyProperty)
        {
            return BindingOperations.GetBinding(button, dependencyProperty);
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace EZMedit8.Extensions
{
    public static class TextBlockExtensions
    {
        public static Binding GetBinding(this TextBlock textBlock, DependencyProperty dependencyProperty)
        {
            return BindingOperations.GetBinding(textBlock, dependencyProperty);
        }
    }
}

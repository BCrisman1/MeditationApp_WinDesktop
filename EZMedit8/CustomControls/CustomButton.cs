using System.Windows;
using System.Windows.Controls.Primitives;

namespace EZMedit8.CustomControls
{
    public class CustomButton : RepeatButton
    {
        static CustomButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CustomButton),
                new FrameworkPropertyMetadata(typeof(CustomButton)));
        }

        public static readonly DependencyProperty TimeSegmentProperty = DependencyProperty.Register(
            "TimeSegment", typeof(int), typeof(CustomButton), new PropertyMetadata(default(int)));

        public int TimeSegment { get => (int)GetValue(TimeSegmentProperty); set => SetValue(TimeSegmentProperty, value); }

    }
}

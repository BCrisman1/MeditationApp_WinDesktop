using EZMedit8.CustomControls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace EZMedit8.Views
{
    /// <summary>
    /// Interaction logic for DurationWindow.xaml
    /// </summary>
    public partial class DurationWindow : Window, INotifyPropertyChanged
    {
        #region EVENTS
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region PROPERTIES
        private Color _color0;
        public Color Color0
        {
            get => _color0;
            set
            {
                _color0 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color0)));
            }
        }

        private Color _color1;
        public Color Color1
        {
            get => _color1;
            set
            {
                _color1 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color1)));
            }
        }

        private int _hours;
        public int Hours
        {
            get => _hours;
            set
            {
                _hours = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hours)));
            }
        }

        private int _minutes;
        public int Minutes
        {
            get => _minutes;
            set
            {
                _minutes = value;
                if (_minutes > 59) { _minutes = 0; Hours++; }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Minutes)));
            }
        }

        private int _seconds;
        public int Seconds
        {
            get => _seconds;
            set
            {
                _seconds = value;
                if (_seconds > 59) { _seconds = 0; Minutes++; }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Seconds)));
            }
        }

        public bool Confirmed { get; set; }
        #endregion

        #region FIELDS
        private LinearGradientBrush _lgb;
        #endregion

        #region METHODS: Constructor(s)
        private DurationWindow()
        {
            InitializeComponent();
        }

        public DurationWindow(LinearGradientBrush lgBrush) : this()
        {            
            _lgb = lgBrush;
        }

        public DurationWindow(TimeSpan timeSpan, LinearGradientBrush lgBrush) : this()
        {
            ParseTimeSpan(timeSpan);
            _lgb = lgBrush;
        }
        #endregion

        #region METHODS: Event Handlers
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_lgb is null) return;
            Color0 = _lgb.GradientStops[0].Color;
            Color1 = _lgb.GradientStops[1].Color;
        }

        private void BtnAddTime_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is CustomButton)) return;

            var binding = BindingOperations.GetBinding(sender as CustomButton, CustomButton.TimeSegmentProperty);
            if (binding is null) return;

            var property = typeof(DurationWindow).GetProperty(binding.Path.Path);
            var time = (int)property.GetValue(this);
            property.SetValue(this, ++time);            
        }

        private void BtnSubtractTime_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is CustomButton)) return;

            var binding = BindingOperations.GetBinding(sender as CustomButton, CustomButton.TimeSegmentProperty);
            if (binding is null) return;

            var property = typeof(DurationWindow).GetProperty(binding.Path.Path);
            var time = (int)property.GetValue(this);

            if (time < 1) return;

            property.SetValue(this, --time);
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }
        #endregion

        #region METHODS: Helpers
        private void ParseTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.Hours > 0) { Hours = timeSpan.Hours; }
            if (timeSpan.Minutes > 0) { Minutes = timeSpan.Minutes; }
            if (timeSpan.Seconds > 0) { Seconds = timeSpan.Seconds; }
        }
        #endregion
    }
}
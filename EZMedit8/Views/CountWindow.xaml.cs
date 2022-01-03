using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace EZMedit8.Views
{
    /// <summary>
    /// Interaction logic for CountWindow.xaml
    /// </summary>
    public partial class CountWindow : Window, INotifyPropertyChanged
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

        public bool Confirmed { get; set; }
        #endregion

        #region FIELDS
        private LinearGradientBrush _lgb;
        #endregion

        #region METHODS: Contructor(s)
        private CountWindow()
        {
            InitializeComponent();
        }

        public CountWindow(LinearGradientBrush lgBrush) : this()
        {
            _lgb = lgBrush;
        }
        #endregion

        #region METHODS: Event Handlers
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_lgb is null) return;
            Color0 = _lgb.GradientStops[0].Color;
            Color1 = _lgb.GradientStops[1].Color;
            MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
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

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is not TextBox) { return; }
            (sender as TextBox).CaretIndex = (sender as TextBox).Text.Length;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not TextBox) { return; }
            
            if (e.Key == Key.Enter) { BtnConfirm_Click(sender, e); }
            if (e.Key == Key.Escape) { BtnCancel_Click(sender, e); }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is not TextBox) { return; }
            e.Handled = e.Text.ToCharArray().Any(i => !char.IsDigit(i)) || ((sender as TextBox).Text.Length + e.Text.Length) >= 4;
        }
        #endregion
    }
}
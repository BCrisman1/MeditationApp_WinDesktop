using EZMedit8.Enums;
using EZMedit8.Models.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;


namespace EZMedit8.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region FIELDS
        private bool _leftMouseButtonDown;
        #endregion

        #region METHODS: Constructor(s)
        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //Logger.Instance.LogException((Exception)e.ExceptionObject);
        }
        #endregion

        #region METHODS: Event Handlers
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Statics.UserControlChangedHandler?.Invoke(this, new UserControlChangedEventArgs() { Control = AppUserControls.SetupControl });
            UpdateScreenDimensions();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) { _leftMouseButtonDown = true; }
            MoveWindow();
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released) 
            {
                _leftMouseButtonDown = false;
                UpdateScreenDimensions();
            }
        }
        #endregion

        #region METHODS: Helpers
        private async void MoveWindow()
        {
            await MoveWindowAsync();
        }

        private async Task MoveWindowAsync()
        {
            await Task.Delay(125);
            if (!_leftMouseButtonDown) { return; }
            while (_leftMouseButtonDown)
            {
                try 
                {
                    DragMove();
                    if (Statics.WindowStateMaximized) { Statics.ToggleWindowStateMaximized(this, false); }
                }
                catch (Exception ex) { Trace.WriteLine(ex.Message); }
                await Task.Delay(0);
            }
        }

        private void UpdateScreenDimensions()
        {            
            var workingArea = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea;
            Statics.ScreenDimensions = new Rect(
                new Point(workingArea.X, workingArea.Y),
                new Point(workingArea.Width, workingArea.Height)
                );            
        }
        #endregion
    }
}
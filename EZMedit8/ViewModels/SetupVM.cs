using EZMedit8.CustomControls;
using EZMedit8.Enums;
using EZMedit8.Extensions;
using EZMedit8.Models;
using EZMedit8.Models.Utilities;
using EZMedit8.Views;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;


namespace EZMedit8.ViewModels
{
    public class SetupVM : Bindable
    {
        #region PROPERTIES: Data
        private static SessionData _sessionData;
        public SessionData SessionData { get => _sessionData; set => SetProperty(ref _sessionData, value); }
        #endregion

        #region PROPERTIES: ICommand
        public ICommand EditFieldCommand { get; set; }
        public ICommand ClearFieldCommand { get; set; }
        public bool EditFieldCanExecute { get => _editFieldCanExecute; set => SetProperty(ref _editFieldCanExecute, value); }        

        public ICommand IntervalModeToggleCommand { get; set; }
        public bool IntervalModeToggleCanExecute { get => _intervalModeToggleCanExecute; set => SetProperty(ref _intervalModeToggleCanExecute, value); }

        public ICommand IntervalPropertyChangedCommand { get; set; }
        public bool IntervalPropertyChangedCanExecute { get => _intervalPropertyChangedCanExecute; set => SetProperty(ref _intervalPropertyChangedCanExecute, value); }

        public ICommand MeditationTimerPropertyChangedCommand { get; set; }
        public bool MeditationTimerPropertyChangedCanExecute { get => _meditationTimerPropertyChangedCanExecute; set => SetProperty(ref _meditationTimerPropertyChangedCanExecute, value); }
        #endregion

        #region PROPERTIES: Booleans
        private static bool _inited;
        public bool Inited { get => _inited; set => SetProperty(ref _inited, value); }
        #endregion

        #region FIELDS: Booleans
        private static bool _editFieldCanExecute = true;
        private static bool _intervalModeToggleCanExecute = true;        
        private static bool _intervalPropertyChangedCanExecute = true;
        private static bool _meditationTimerPropertyChangedCanExecute = true;
        #endregion

        #region METHODS: Constructor(s)
        public SetupVM()
        {
            Trace.WriteLine("SetupVM() instance created...");
            Init();
        }
        #endregion

        #region METHODS: Init
        private void Init()
        {
            SessionDataInit();
            CommandsInit();
            Statics.LoadSettings();
            Trace.WriteLine("SetupVM() instance initialized...");
        }

        private void SessionDataInit()
        {
            if (SessionData is null) { SessionData = SessionData.Instance; }
        }

        private void CommandsInit()
        {
            EditFieldCommand = new DelegateCommand(EditFieldExecute);
            ClearFieldCommand = new DelegateCommand(ClearFieldExecute);
            IntervalModeToggleCommand = new DelegateCommand(IntervalModeToggleExecute);
            IntervalPropertyChangedCommand = new DelegateCommand(IntervalPropertyChangedExecute);
            MeditationTimerPropertyChangedCommand = new DelegateCommand(MeditationTimerPropertyChangedExecute);
        }
        #endregion

        #region METHODS: Commands
        public void EditFieldExecute(object parameter)
        {
            if (!EditFieldCanExecute || parameter is not Button) { return; }
            EditFieldCanExecute = false;

            var button = parameter as Button;

            var binding = button.GetBinding(ContentControl.ContentProperty);
            var stageType = binding.GetStageType(this);
            var intervalMode = binding.GetIntervalMode(this);

            var parent = button.Parent as CustomStackPanel;
            var linearGradientBrush = parent.LinearGradientBrush;

            if (stageType == StageType.CountdownTimer) { TimePicker(binding, linearGradientBrush); }
            else if (stageType == StageType.Interval && !binding.Path.Path.Contains("Filename", StringComparison.OrdinalIgnoreCase))
            {
                if (intervalMode == IntervalMode.Delay) { TimePicker(binding, linearGradientBrush); }
                if (intervalMode == IntervalMode.Count) { SetCount(linearGradientBrush); }
            }
            else { FilePicker(binding, stageType); }

            EditFieldCanExecute = true;
        }

        public void ClearFieldExecute(object parameter)
        {
            if (!EditFieldCanExecute || parameter is not Button) { return; }
            EditFieldCanExecute = false;

            var button = parameter as Button;

            var binding = button.GetBinding(ContentControl.ContentProperty);            
            SetValue(binding, null);

            EditFieldCanExecute = true;
        }

        public void IntervalModeToggleExecute(object _)
        {
            if (!IntervalModeToggleCanExecute) { return; }
            IntervalModeToggleCanExecute = false;

            var lastIndex = Enum.GetValues(typeof(IntervalMode)).Cast<IntervalMode>().LastOrDefault();
            if (SessionData.Interval.IntervalMode == lastIndex) { SessionData.Interval.IntervalMode = 0; }
            else { SessionData.Interval.IntervalMode++; }

            IntervalModeToggleCanExecute = true;
        }

        public void IntervalPropertyChangedExecute(object parameter)
        {
            if (!IntervalPropertyChangedCanExecute) { return; }
            IntervalPropertyChangedCanExecute = false;

            if (parameter is TimeSpan) { SessionData.Interval.IntervalDelay = SessionData.Interval.TimeRemaining; }
            if (parameter is int && SessionData.Interval.TotalIntervals > 0 && SessionData.MeditationTimer.TimeRemaining != TimeSpan.Zero)
            { CalculateIntervalDelay(); }

            IntervalPropertyChangedCanExecute = true;
        }

        public void MeditationTimerPropertyChangedExecute(object parameter)
        {
            if (!MeditationTimerPropertyChangedCanExecute) { return; }
            MeditationTimerPropertyChangedCanExecute = false;

            if (parameter is TimeSpan && SessionData.MeditationTimer.TimeRemaining > TimeSpan.Zero && SessionData.Interval.TotalIntervals > 0) { CalculateIntervalDelay(); }

            MeditationTimerPropertyChangedCanExecute = true;
        }
        #endregion

        #region METHODS: Helpers
        private void FilePicker(Binding binding, StageType stageType)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (stageType == StageType.None)
            {
                ofd.Filter = Statics.FILE_DIALOG_FILTER_IMAGES;
                ofd.InitialDirectory = Statics.GetFolder(FolderType.Images);
            }
            else
            {
                ofd.Filter = Statics.FILE_DIALOG_FILTER_AUDIO;
                ofd.InitialDirectory = Statics.GetFolder(FolderType.AppCommonData);
            }

            if (ofd.ShowDialog() == false) return;

            SetValue(binding, ofd.FileName);
        }

        private void TimePicker(Binding binding, LinearGradientBrush brush)
        {
            string propertyName = string.Empty;
            object propertyObj = this;

            foreach (string text in binding.Path.Path.Split("."))
            {
                propertyName = text;
                var prop = propertyObj.GetType().GetProperty(text);
                var obj = prop.GetValue(propertyObj, null);
                if (!binding.Path.Path.Split(".").LastOrDefault().Equals(propertyName)) { propertyObj = obj; }
            }

            var timeSpan = (TimeSpan)propertyObj.GetType().GetProperty(propertyName).GetValue(propertyObj);

            var durationWindow = new DurationWindow(timeSpan, brush);
            durationWindow.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(i => i.IsActive);
            durationWindow.ShowDialog();

            if (!durationWindow.Confirmed) return;

            long hoursMs = durationWindow.Hours * 60 * 60 * 1000;
            long minutesMs = durationWindow.Minutes * 60 * 1000;
            long secondsMs = durationWindow.Seconds * 1000;
            TimeSpan timeSpanMs = TimeSpan.FromMilliseconds(hoursMs + minutesMs + secondsMs);

            SetValue(binding, timeSpanMs);
        }

        private void SetValue(Binding binding, object value)
        {
            string propertyName = string.Empty;
            object propertyObj = this;

            foreach (string text in binding.Path.Path.Split("."))
            {
                propertyName = text;
                var prop = propertyObj.GetType().GetProperty(text);
                var obj = prop.GetValue(propertyObj, null);
                if (!binding.Path.Path.Split(".").LastOrDefault().Equals(propertyName)) { propertyObj = obj; }
            }

            //if (propertyName.Contains("Filename") && value is null) { SessionData.BackgroundAudio.Filename = null; return; }
            propertyObj.GetType().GetProperty(propertyName).SetValue(propertyObj, value);
        }

        private void SetCount(LinearGradientBrush brush)
        {
            var countWindow = new CountWindow(brush);
            countWindow.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(i => i.IsActive);
            countWindow.DataContext = this;
            countWindow.ShowDialog();
        }

        private void CalculateIntervalDelay()
        {
            return;
            //if (SessionData?.MeditationTimer?.TimeRemaining is null || SessionData?.MeditationTimer?.TimeRemaining == TimeSpan.Zero) { return; }
            //if (SessionData?.Interval?.TotalIntervals == 0) { return; }

            //long intervalDelayMs = (long)((double)SessionData.MeditationTimer.TimeRemaining.TotalMilliseconds / SessionData.Interval.TotalIntervals);
            //Trace.WriteLine($"intervalDelayMs = {intervalDelayMs}");
            //SessionData.Interval.IntervalDelay = TimeSpan.FromMilliseconds(intervalDelayMs);
        }
        #endregion
    }
}
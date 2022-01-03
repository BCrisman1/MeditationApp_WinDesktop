using EZMedit8.Enums;
using EZMedit8.Models;
using EZMedit8.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;


namespace EZMedit8.ViewModels
{
    public class SessionVM : Bindable
    {
        #region PROPERTES: Session Data
        private string _imageFilename = null;
        public string ImageFilename { get => _imageFilename; set => SetProperty(ref _imageFilename, value); }
        private List<StageData> StageList { get; set; }
        private StageData BackgroundStage { get; set; }
        private StageData IntervalStage { get; set; }
        #endregion

        #region PROPERTIES: ICommand
        public ICommand PauseCommand { get; set; }
        public bool PauseCanExecute { get => _pauseCommandCanExecute; set => SetProperty(ref _pauseCommandCanExecute, value); }
        
        public ICommand StopCommand { get; set; }
        public bool StopCanExecute { get => _stopCommandCanExecute; set => SetProperty(ref _stopCommandCanExecute, value); }

        public ICommand ToggleWindowStateCommand { get; set; }
        public bool ToggleWindowStateCanExecute { get => _toggleWindowStateCanExecute; set => SetProperty(ref _toggleWindowStateCanExecute, value); }
        #endregion

        #region FIELDS: Command Booleans
        private static bool _pauseCommandCanExecute = true;
        private static bool _stopCommandCanExecute = true;
        private bool _toggleWindowStateCanExecute = true;
        #endregion

        #region FIELDS: Class Variables
        private static bool _inited;
        private int _stageIndex;
        #endregion

        #region METHODS: Constructor(s)
        public SessionVM()
        {
            System.Diagnostics.Trace.WriteLine("SessionVM() instance created.");
            Init();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }
        #endregion

        #region METHODS: Event Handlers
        private async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            await Logger.Instance.LogException((Exception)e.ExceptionObject);
            NavigateToSetup();
        }
        #endregion

        #region METHODS: Init
        private void Init()
        {
            if (_inited) { return; }
            _inited = true;
            CommandsInit();
            StageDataInit();
            BeginSession();
        }

        private void CommandsInit()
        {
            PauseCommand = new DelegateCommand(PauseExecute);
            StopCommand = new DelegateCommand(StopExecute);
            ToggleWindowStateCommand = new DelegateCommand(ToggleWindowStateExecute);
        }

        private void StageDataInit()
        {
            ImageFilename = SessionData.Instance.BackgroundImage.Filename;
            StageList = SessionData.Instance.PopulateStageList();

            IntervalInit();
        }

        private void IntervalInit()
        {
            if (StageList is null || !StageList.Any(i => i.StageType == StageType.Interval)) { return; }

            IntervalStage = StageList.Where(i => i.StageType == StageType.Interval).FirstOrDefault();
            _ = StageList.Remove(IntervalStage);
        }
        #endregion

        #region METHODS: Session
        private void BeginSession()
        {
            if (StageList is null || !StageList.Any()) { return; }
            RunCurrentStage();
        }

        private void RunCurrentStage()
        {
            if (_stageIndex >= StageList.Count()) { return; }

            var stage = StageList[_stageIndex];

            if (stage == null) { /* ERROR */ return; }

            if (stage.StageType == StageType.Background) { RunLoopingStage(stage); }
            else
            {
                stage.Completed += Stage_Completed;

                new System.Threading.Thread(() =>
                {
                    if (stage.StageType == StageType.PlayAudio) { stage.Start(); }
                    if (stage.StageType == StageType.CountdownTimer) { RunTimedStage(stage); }
                })
                { IsBackground = true, Priority = System.Threading.ThreadPriority.Highest }.Start();
            }
        }

        private void RunLoopingStage(StageData stage)
        {
            BackgroundStage = stage;
            stage.Start();
            Stage_Completed(this, null);
        }

        private void RunTimedStage(StageData stage)
        {
            UpdateTimeRemaining(stage);
            stage.Timer.Tick += new EventHandler((object sender, EventArgs e) => UpdateTimeRemaining(stage));
            stage.Start();
            if (!stage.PrepStage) { IntervalStage?.Start(); }
        }

        private void UpdateTimeRemaining(StageData stage)
        {
            TimeRemaining = stage?.TimeRemaining.ToString("c");
        }

        private void Stage_Completed(object sender, EventArgs e)
        {
            if (sender is StageData) { (sender as StageData).Dispose(); }

            if (++_stageIndex == StageList.Count()) { NavigateToSetup(); }
            else { RunCurrentStage(); }
        }
        
        private async void StageCleanup()
        {
            if (StageList == null || !StageList.Any()) { return; }           
            
            if (IntervalStage != null)
            {
                try { IntervalStage.Dispose(); }
                catch (Exception ex) { await Logger.Instance.LogException(ex); }
            }

            foreach (StageData stage in StageList)
            {
                try { stage.Dispose(); }
                catch (Exception ex) { await Logger.Instance.LogException(ex); }
            }
        }

        private void NavigateToSetup()
        {
            _inited = false;

            Application.Current.Dispatcher.Invoke(new Action(() => 
            {
                StageCleanup();
                Statics.UserControlChangedHandler?.Invoke(this, new UserControlChangedEventArgs() { Control = AppUserControls.SetupControl });
            }));
            
        }
        #endregion

        #region METHODS: Commands
        public void PauseExecute(object _)
        {
            if (!PauseCanExecute) { return; }
            PauseCanExecute = false;

            if (BackgroundStage is not null) { BackgroundStage.Pause(); }
            if (IntervalStage is not null) { IntervalStage.Pause(); }
            if (BackgroundStage != StageList[_stageIndex] && IntervalStage != StageList[_stageIndex]) { StageList[_stageIndex].Pause(); }

            PauseCanExecute = true;
        }

        public void StopExecute(object _)
        {
            if (!StopCanExecute) { return; }
            StopCanExecute = false;

            NavigateToSetup();

            StopCanExecute = true;
        }

        public void ToggleWindowStateExecute(object _)
        {
            if (!ToggleWindowStateCanExecute) { return; }
            ToggleWindowStateCanExecute = false;

            Statics.WindowStateMaximized = !Statics.WindowStateMaximized;
            Statics.ToggleWindowStateMaximized?.Invoke(Application.Current.MainWindow, Statics.WindowStateMaximized);

            ToggleWindowStateCanExecute = true;
        }
        #endregion

        #region PROPERTIES: DataBinding
        private string _timeRemaining;
        public string TimeRemaining { get => _timeRemaining; set => SetProperty(ref _timeRemaining, value); }
        #endregion
    }
}
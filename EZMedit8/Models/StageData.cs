using EZMedit8.Enums;
using EZMedit8.Models.Utilities.NAudio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Threading;


namespace EZMedit8.Models
{
    public class StageData : INotifyPropertyChanged
    {
        #region EVENTS
        public event EventHandler Completed;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region PROPERTIES: Public
        public string Filename { get => _filename; set => SetProperty(ref _filename, value); }
        public WaveData WaveData { get; private set; }
        public TimeSpan TimeRemaining { get => _timeRemaining; set => SetProperty(ref _timeRemaining, value); }
        public TimeSpan IntervalDelay { get => _intervalDelay; set => SetProperty(ref _intervalDelay, value); }
        public DispatcherTimer Timer { get; private set; }
        public StageType StageType { get => _stageType; set => SetProperty(ref _stageType, value); }
        public IntervalMode IntervalMode { get => _intervalMode; set => SetProperty(ref _intervalMode, value); }
        public bool PrepStage { get => _prepStage; set => SetProperty(ref _prepStage, value); }
        public bool Disposed { get; private set; }
        public int TotalIntervals { get => _totalIntervals; set => SetProperty(ref _totalIntervals, value); }
        #endregion

        #region FIELDS: Property Variables
        private string _filename;
        private TimeSpan _timeRemaining;
        private TimeSpan _intervalDelay;
        private StageType _stageType;
        private IntervalMode _intervalMode;
        private bool _prepStage;
        private int _totalIntervals;
        #endregion

        #region FIELDS: Independent        
        private TimeSpan _totalIntervalDuration;
        private int _tickCounter;
        private int _intervalAudioPlayCounter;
        protected bool _locked;
        #endregion

        #region METHODS: Constructor(s)
        public StageData()
        {
            Filename = string.Empty;
        }

        public StageData(StageData stageData)
        {
            System.Text.StringBuilder filename = new(stageData.Filename);
            Filename = filename.ToString();

            double timeRemainingMs = stageData.TimeRemaining.TotalMilliseconds;
            TimeRemaining = TimeSpan.FromMilliseconds(timeRemainingMs);

            double intervalDelayMs = stageData.IntervalDelay.TotalMilliseconds;
            IntervalDelay = TimeSpan.FromMilliseconds(intervalDelayMs);

            StageType = stageData.StageType;
            IntervalMode = stageData.IntervalMode;
            PrepStage = stageData.PrepStage;
            TotalIntervals = stageData.TotalIntervals;
        }
        #endregion

        #region METHODS: Object Init
        public void Init()
        {
            WaveDataInit(_filename);
            TimerInit();
            _locked = true;
        }

        private void WaveDataInit(string filename)
        {
            if (string.IsNullOrEmpty(filename) || StageType == StageType.None) { return; }

            WaveData = new WaveData(filename, StageType);

            if (WaveData == null || WaveData.WaveOut == null) { return; }
            if (StageType == StageType.Interval) { WaveData.WaveOut.PlaybackStopped += Interval_PlaybackStopped; }
            if (StageType == StageType.PlayAudio) { WaveData.WaveOut.PlaybackStopped += AudioOnly_PlaybackStopped; }
        }

        private void TimerInit()
        {
            if (StageType == StageType.Interval)
            {
                Trace.WriteLine($"IntervalDelay = {IntervalDelay}");
                Trace.WriteLine($"TotalIntervals = {TotalIntervals}");
                _totalIntervalDuration = IntervalDelay;                
                if (IntervalMode == IntervalMode.Count) { IntervalDelay = _totalIntervalDuration / 2; }             
            }

            Timer = new DispatcherTimer();
            Timer.Dispatcher.Thread.Priority = System.Threading.ThreadPriority.Normal;
            Timer.Tick += Timer_Tick;
            Timer.Interval = StageType == StageType.Interval ? IntervalDelay : new TimeSpan(0, 0, 1);
        }
        #endregion

        #region METHODS: Event Handlers (Private)
        private void Timer_Tick(object sender, EventArgs e)
        {
            _locked = false;

            CountDownTimer_Tick(sender, e);
            IntervalTimer_Tick(sender, e);

            _locked = true;
        }

        private void Interval_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (WaveData?.WaveStream == null || Disposed) { return; }
            WaveData.WaveStream.Position = 0;
        }

        private void AudioOnly_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Completed?.Invoke(sender, e);
        }
        #endregion

        #region METHODS: Helpers (Private)
        private void CountDownTimer_Tick(object sender, EventArgs e)
        {
            if (StageType != StageType.CountdownTimer) { return; }

            if (TimeRemaining == TimeSpan.Zero)
            {
                TimerCountdownComplete(sender, e);
                return;
            }

            TimeRemaining = TimeRemaining.Subtract(TimeSpan.FromSeconds(1));
        }

        private void IntervalTimer_Tick(object sender, EventArgs e)
        {
            if (StageType != StageType.Interval) { return; }

            if (_intervalAudioPlayCounter == TotalIntervals)
            {
                TimerCountdownComplete(sender, e);
                return;
            }

            if (WaveData != null && WaveData.WaveOut != null)
            {
                PlayAudio();
                Timer.Interval = IntervalDelay = _totalIntervalDuration;
            }

            _intervalAudioPlayCounter++;
        }

        private void TimerCountdownComplete(object sender, EventArgs e)
        {
            _locked = true;
            StopTimer();
            Completed?.Invoke(sender, e);
            if (StageType == StageType.Interval) { Trace.WriteLine($"Total Intervals Sounded = {_intervalAudioPlayCounter}"); }            
        }

        private bool SetProperty<T>(ref T backingData, T value, [CallerMemberName] string callerName = "")
        {
            if (_locked) { return false; }
            if (EqualityComparer<T>.Default.Equals(backingData, value)) { return false; }
            backingData = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerName));
            return true;
        }

        private void PlayAudio()
        {
            if (WaveData == null || WaveData.WaveOut == null) { Completed?.Invoke(this, null); return; }
            if (WaveData.WaveOut.PlaybackState == PlaybackState.Playing) { return; }
            WaveData.WaveOut.Play();
        }

        private void StopAudio()
        {
            if (WaveData == null || WaveData.WaveOut == null) { return; }
            WaveData.WaveOut.Stop();
        }

        private void PauseAudio()
        {
            if (WaveData == null || WaveData.WaveOut == null) { return; }
            if (WaveData.WaveOut.PlaybackState != PlaybackState.Playing) { return; }
            WaveData.WaveOut.Pause();
        }

        private void StartTimer()
        {
            if (Timer == null) { return; }
            Timer.Start();
        }

        private void StopTimer()
        {
            if (Timer == null) { return; }
            Timer.Stop();
        }
        #endregion

        #region METHODS: Public Instance
        private bool _started;
        public bool Start()
        {
            if (!_locked) return false;
            if (StageType == StageType.PlayAudio || StageType == StageType.Background) { PlayAudio(); }
            if (StageType == StageType.CountdownTimer || StageType == StageType.Interval) { StartTimer(); }
            _started = true;
            return true;
        }

        public void Stop()
        {
            if (StageType == StageType.PlayAudio || StageType == StageType.Background) { StopAudio(); }
            if (StageType == StageType.CountdownTimer || StageType == StageType.Interval) { StopTimer(); }
        }

        private bool _paused;
        public void Pause()
        {      
            if (!_started) { return; }
            if (_paused)
            {
                if (StageType == StageType.PlayAudio || StageType == StageType.Background) { PlayAudio(); }
                if (StageType == StageType.CountdownTimer || StageType == StageType.Interval) { StartTimer(); }
                _paused = false;
            }
            else
            {
                if (StageType == StageType.PlayAudio || StageType == StageType.Background) { PauseAudio(); }
                if (StageType == StageType.CountdownTimer || StageType == StageType.Interval) { StopTimer(); }
                _paused = true;
            }
        }

        public void Dispose()
        {
            if (Disposed) { return; }

            _locked = false;
            _started = false;

            Stop();

            if (WaveData != null)
            {
                WaveData.Dispose();
                WaveData = null;
            }

            if (Timer != null) { Timer = null; }

            //TimeRemaining = _totalDurationStorage;
            Disposed = true;
        }

        public void Unlock()
        {
            _locked = false;
        }
        #endregion
    }

    public class WaveData
    {
        #region PROPERTIES
        public WaveStream WaveStream { get; private set; }
        public IWavePlayer WaveOut { get; private set; }
        #endregion

        #region METHODS: Constructor(s)
        public WaveData(string filename, StageType stageType)
        {
            if (string.IsNullOrEmpty(filename) || stageType == StageType.None) { return; }

            WaveStreamInit(filename);
            WaveOutInit(stageType);
        }
        #endregion

        #region METHODS: Object Init
        public void ReInit(string filename, StageType stageType)
        {
            WaveStreamInit(filename);
            WaveOutInit(stageType);
        }

        private void WaveStreamInit(string filename)
        {
            try
            {
                WaveStream = new Mp3FileReader(filename);
                System.Diagnostics.Trace.WriteLine($"Mp3FileReader({filename}) successful!");
                return;
            }
            catch (Exception) { System.Diagnostics.Trace.WriteLine($"Mp3FileReader({filename}) failed!"); }

            try
            {
                WaveStream = new WaveFileReader(filename);
                System.Diagnostics.Trace.WriteLine($"WaveFileReader({filename}) successful!");
                return;
            }
            catch (Exception) { System.Diagnostics.Trace.WriteLine($"WaveFileReader({filename}) failed!"); }

            WaveStream = null;
        }

        private void WaveOutInit(StageType stageType)
        {
            if (WaveStream is null) { return; }

            if (stageType == StageType.Background) { BackgroundInit(); }
            else
            {
                WaveOut = new WaveOutEvent();
                WaveOut.Init(WaveStream);
            }
        }

        private void BackgroundInit()
        {
            WaveStream = new LoopStream(WaveStream);
            WaveOut = new WaveOut();
            WaveOut.Init(WaveStream);
        }
        #endregion

        #region METHODS: Public Instance        
        public void Dispose()
        {
            if (WaveOut != null) { WaveOut.Dispose(); }
            if (WaveStream != null) { WaveStream.Dispose(); }
        }
        #endregion
    }

    public enum StageType
    {
        None,
        PlayAudio,
        CountdownTimer,
        Background,
        Interval
    }
}

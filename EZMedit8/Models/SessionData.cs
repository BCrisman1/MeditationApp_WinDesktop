using EZMedit8.Attributes;
using EZMedit8.Extensions;
using EZMedit8.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;


namespace EZMedit8.Models
{
    public class SessionData : Bindable
    {
        #region PROPERTIES: Constants / Background        
        private static StageData _backgroundAudio = new() { StageType = StageType.Background };
        [PropertyOrder(0)]
        public StageData BackgroundAudio { get => _backgroundAudio; set => SetProperty(ref _backgroundAudio, value); }

        private static OtherData _backgroundImage = new();
        [PropertyOrder(1)]
        public OtherData BackgroundImage { get => _backgroundImage; set => SetProperty(ref _backgroundImage, value); }
        #endregion

        #region PROPERTIES: Prepare Stage
        private static StageData _preparationStart = new() { StageType = StageType.PlayAudio, PrepStage = true };
        [PropertyOrder(2)]
        public StageData PreparationStart { get => _preparationStart; set => SetProperty(ref _preparationStart, value); }

        private static StageData _preparationTimer = new() { StageType = StageType.CountdownTimer, PrepStage = true };
        [PropertyOrder(3)]
        public StageData PreparationTimer { get => _preparationTimer; set => SetProperty(ref _preparationTimer, value); }

        private static StageData _preparationEnd = new() { StageType = StageType.PlayAudio, PrepStage = true };
        [PropertyOrder(4)]
        public StageData PreparationEnd { get => _preparationEnd; set => SetProperty(ref _preparationEnd, value); }
        #endregion

        #region PROPERTIES: Meditation Stage
        private static StageData _meditationStart = new() { StageType = StageType.PlayAudio, PrepStage = true };
        [PropertyOrder(5)]
        public StageData MeditationStart { get => _meditationStart; set => SetProperty(ref _meditationStart, value); }

        private static StageData _meditationTimer = new() { StageType = StageType.CountdownTimer };
        [PropertyOrder(6)]
        public StageData MeditationTimer { get => _meditationTimer; set => SetProperty(ref _meditationTimer, value); }

        private static StageData _meditationEnd = new() { StageType = StageType.PlayAudio, PrepStage = true };
        [PropertyOrder(7)]
        public StageData MeditationEnd { get => _meditationEnd; set => SetProperty(ref _meditationEnd, value); }
        #endregion

        #region PROPERTIES: Interval
        private static StageData _interval = new() { StageType = StageType.Interval };
        [PropertyOrder(8)]
        public StageData Interval { get => _interval; set => SetProperty(ref _interval, value); }
        #endregion

        #region METHODS: Constructor(s)
        private SessionData()
        {
            System.Diagnostics.Trace.WriteLine("SessionData() initalized!");            
        }
        #endregion

        #region METHODS: Accessors
        public List<StageData> PopulateStageList()
        {
            CalculateTotalIntervals();
            CalculateIntervalDelay();

            var dataTemp = typeof(SessionData).GetProperties().OrderBy(i => i.PropertyOrder())       // IOrderedEnumerable<PropertyInfo> (Sorted by PropertyOrderAttribute.Order value)
                                               .Where(i => i.GetValue(this) is StageData)            // IEnumerable<PropertyInfo> (StageData Objects)
                                               .Select(i => (StageData)i.GetValue(this))             // IEnumerable<StageData>
                                               .ToList();                                            // List<StageData>

            List<StageData> stageList = new();

            dataTemp.ForEach(i => stageList.Add(new(i)));                                            // Create new List<StageData>
            stageList.ForEach(i => i.Init());                                                        // List<StageData> (All StageData Objects Init'd)

            return stageList;
        }

        public void ResetData()
        {
            var stageData = typeof(SessionData).GetProperties().OrderBy(i => i.PropertyOrder())      // IOrderedEnumerable<PropertyInfo> (Sorted by PropertyOrderAttribute.Order value)
                                               .Where(i => i.GetValue(this) is StageData)            // IEnumerable<PropertyInfo> (StageData Objects)
                                               .Select(i => (StageData)i.GetValue(this))             // IEnumerable<StageData>
                                               .ToArray();                                           // StageData[]

            var otherData = typeof(SessionData).GetProperties().OrderBy(i => i.PropertyOrder())
                                               .Where(i => i.GetValue(this) is OtherData)
                                               .Select(i => (OtherData)i.GetValue(this))
                                               .ToArray();

            for (int i = 0; i < stageData.Length; i++) 
            {
                stageData[i].Filename = string.Empty;
                stageData[i].TimeRemaining = new();
                stageData[i].IntervalDelay = new();
                stageData[i].TotalIntervals = 0;
                
            }
            for (int i = 0; i < otherData.Length; i++) 
            {
                otherData[i].Filename = string.Empty;                
            }
        }

        private void CalculateTotalIntervals()
        {
            if (Interval.IntervalMode != Enums.IntervalMode.Delay) { return; }
            if (MeditationTimer.TimeRemaining == TimeSpan.Zero || Interval.IntervalDelay == TimeSpan.Zero) { return; }

            Interval.IntervalDelay = TimeSpan.FromSeconds(Interval.IntervalDelay.Seconds);
            Interval.TotalIntervals = (int)Math.Floor(MeditationTimer.TimeRemaining.TotalSeconds / Interval.IntervalDelay.Seconds);
        }

        private void CalculateIntervalDelay()
        {
            if (Interval.IntervalMode != Enums.IntervalMode.Count) { return; }
            if (MeditationTimer.TimeRemaining == TimeSpan.Zero || Interval.TotalIntervals == 0) { return; }

            double intervalDelayMs = MeditationTimer.TimeRemaining.TotalMilliseconds / Interval.TotalIntervals;
            Interval.IntervalDelay = TimeSpan.FromMilliseconds(intervalDelayMs);            
        }

        private static readonly Lazy<SessionData> _lazySessionDataInstance = new Lazy<SessionData>(() => new SessionData());
        public static SessionData Instance => _lazySessionDataInstance.Value;
        #endregion
    }
}
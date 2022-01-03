using EZMedit8.Enums;
using EZMedit8.Models;
using System.Linq;
using System.Windows.Data;


namespace EZMedit8.Extensions
{
    public static class BindingExtensions
    {
        public static StageType GetStageType(this Binding binding, object caller)
        {
            object obj = binding.GetObject(caller);
            return obj is StageData ? (obj as StageData).StageType : StageType.None;
        }

        public static IntervalMode GetIntervalMode(this Binding binding, object caller)
        {
            object obj = binding.GetObject(caller);
            return obj is StageData ? (obj as StageData).IntervalMode : IntervalMode.Count;
        }

        private static object GetObject(this Binding binding, object caller)
        {
            object obj = caller;

            foreach (string text in binding.Path.Path.Split("."))
            {
                var prop = obj.GetType().GetProperty(text);
                var objTemp = prop.GetValue(obj, null);
                if (!binding.Path.Path.Split(".").LastOrDefault().Equals(text)) { obj = objTemp; }
            }

            return obj;
        }
    }
}

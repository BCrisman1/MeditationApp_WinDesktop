using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EZMedit8.CustomEventArgs;

namespace EZMedit8.Models.Utilities
{
    /// <summary>
    /// Serves as the base for classes which will contain members that will participate in data binding more sophisticated than OneTime binding.
    /// Implements INotifyPropertyChanged
    /// </summary>
    public class Bindable : INotifyPropertyChanged
    {
        #region Set Property Value
        /// <summary>
        /// Verifies whether the value has changed, updates the stored value and notifies subscribers that the value has changed
        /// </summary>
        /// <typeparam name="T">The type of stored value</typeparam>
        /// <param name="backingData">The stored value</param>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The name of the property being updated</param>
        /// <returns>True if the value as changed and is successfully updated; False if the value has not changed.</returns>
        protected virtual bool SetProperty<T>(ref T backingData, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingData, value)) { return false; }

            var oldValue = backingData;

            backingData = value;
            OnPropertyChanged(oldValue, value, propertyName);

            return true;
        }

        #region Changed
        /// <summary>
        /// Verifies whether the value has changed, updates the stored value and notifies subscribers that the value has changed.
        /// Performs additional work after the value is changed as specified by the passed onChanged Action.
        /// </summary>
        /// <typeparam name="T">The type of stored value</typeparam>
        /// <param name="backingData">The stored value</param>
        /// <param name="value">The new value</param>
        /// <param name="onChanged">Additional logic to execute after the value has changed</param>
        /// <param name="propertyName">The name of the property being updated</param>
        /// <returns>True if the value as changed and is successfully updated; False if the value has not changed.</returns>
        protected virtual bool SetProperty<T>(ref T backingData, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingData, value)) { return false; }

            var oldValue = backingData;

            backingData = value;
            OnPropertyChanged(oldValue, value, propertyName);
            onChanged?.Invoke();
            
            return true;
        }

        /// <summary>
        /// Verifies whether the value has changed, updates the stored value and notifies subscribers that the value has changed.
        /// Checks the passed predicate before updating the value, and does not change the value if the predicate evaluates false.
        /// Performs additional work after the value is changed as specified by the passed onChanged Action.
        /// </summary>
        /// <typeparam name="T">The type of stored value</typeparam>
        /// <param name="backingData">The stored value</param>
        /// <param name="value">The new value</param>
        /// <param name="predicate">Filter logic used to determine whether the value should be updated</param>
        /// <param name="onChanged">Additional logic to execute after the value has changed</param>
        /// <param name="propertyName">The name of the property being updated</param>
        /// <returns>True if the value as changed and is successfully updated; False if the value has not changed or the predicate evaluates False.</returns>
        protected virtual bool SetProperty<T>(ref T backingData, T value, Func<bool> predicate, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingData, value)) { return false; }
            if (!predicate()) { return false; }

            var oldValue = backingData;

            backingData = value;
            OnPropertyChanged(oldValue, value, propertyName);
            onChanged?.Invoke();
            
            return true;
        }

        /// <summary>
        /// Verifies whether the value has changed, updates the stored value and notifies subscribers that the value has changed.
        /// Checks the passed predicate before updating the value, and does not change the value if the predicate evaluates false.
        /// Performs additional work after the value is changed as specified by the passed onChanged Action.
        /// </summary>
        /// <typeparam name="T">The type of stored value</typeparam>
        /// <param name="backingData">The stored value</param>
        /// <param name="value">The new value</param>
        /// <param name="coerceValue">Logic used to coerce the value before being updated</param>
        /// <param name="onChanged">Additional logic to execute after the value has changed</param>
        /// <param name="propertyName">The name of the property being updated</param>
        /// <returns>True if the value as changed and is successfully updated; False if the value has not changed or the predicate evaluates False.</returns>
        protected virtual bool SetProperty<T>(ref T backingData, T value, Func<T,T> coerceValue, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingData, value)) { return false; }
            
            var oldValue = backingData;
            backingData = coerceValue(value);
            OnPropertyChanged(oldValue, value, propertyName);
            onChanged?.Invoke();
            
            return true;
        }
        #endregion

        #region Changing and Changed
        /// <summary>
        /// Verifies whether the value has changed, updates the stored value and notifies subscribers that the value has changed.
        /// Performs additional work after the value is changed as specified by the passed onChanged Action.
        /// </summary>
        /// <typeparam name="T">The type of stored value</typeparam>
        /// <param name="backingData">The stored value</param>
        /// <param name="value">The new value</param>
        /// <param name="onChanging">Additional logic to execute before the value has changed</param>
        /// <param name="onChanged">Additional logic to execute after the value has changed</param>
        /// <param name="propertyName">The name of the property being updated</param>
        /// <returns>True if the value as changed and is successfully updated; False if the value has not changed.</returns>
        protected virtual bool SetProperty<T>(ref T backingData, T value, Action onChanging, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingData, value)) { return false; }

            var oldValue = backingData;

            onChanging?.Invoke();
            backingData = value;
            OnPropertyChanged(oldValue, value, propertyName);
            onChanged?.Invoke();
            
            return true;
        }

        /// <summary>
        /// Verifies whether the value has changed, updates the stored value and notifies subscribers that the value has changed.
        /// Checks the passed predicate before updating the value, and does not change the value if the predicate evaluates false.
        /// Performs additional work after the value is changed as specified by the passed onChanged Action.
        /// </summary>
        /// <typeparam name="T">The type of stored value</typeparam>
        /// <param name="backingData">The stored value</param>
        /// <param name="value">The new value</param>
        /// <param name="predicate">Filter logic used to determine whether the value should be updated</param>
        /// <param name="onChanging">Additional logic to execute before the value has changed</param>
        /// <param name="onChanged">Additional logic to execute after the value has changed</param>
        /// <param name="propertyName">The name of the property being updated</param>
        /// <returns>True if the value as changed and is successfully updated; False if the value has not changed or the predicate evaluates False.</returns>
        protected virtual bool SetProperty<T>(ref T backingData, T value, Func<bool> predicate, Action onChanging, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingData, value)) { return false; }
            if (!predicate()) { return false; }

            var oldValue = backingData;

            onChanging?.Invoke();
            backingData = value;
            OnPropertyChanged(oldValue, value, propertyName);
            onChanged?.Invoke();

            return true;
        }

        /// <summary>
        /// Verifies whether the value has changed, updates the stored value and notifies subscribers that the value has changed.
        /// Checks the passed predicate before updating the value, and does not change the value if the predicate evaluates false.
        /// Performs additional work after the value is changed as specified by the passed onChanged Action.
        /// </summary>
        /// <typeparam name="T">The type of stored value</typeparam>
        /// <param name="backingData">The stored value</param>
        /// <param name="value">The new value</param>
        /// <param name="coerceValue">Logic used to coerce the value before being updated</param>
        /// <param name="onChanging">Additional logic to execute before the value has changed</param>
        /// <param name="onChanged">Additional logic to execute after the value has changed</param>
        /// <param name="propertyName">The name of the property being updated</param>
        /// <returns>True if the value as changed and is successfully updated; False if the value has not changed or the predicate evaluates False.</returns>
        protected virtual bool SetProperty<T>(ref T backingData, T value, Func<T, T> coerceValue, Action onChanging, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingData, value)) { return false; }
            
            var oldValue = backingData;

            onChanging?.Invoke();
            backingData = coerceValue(value);
            OnPropertyChanged(oldValue, value, propertyName);
            onChanged?.Invoke();

            return true;
        }
        #endregion
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(object oldValue, object newValue, [CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new RichPropertyChangedEventArgs(oldValue, newValue, propertyName)); }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
        #endregion
    }
}

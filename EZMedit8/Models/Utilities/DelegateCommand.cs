﻿using System;
using System.Windows.Input;

namespace EZMedit8.Models.Utilities
{
    /// <summary>
    /// An implementation of ICommand which accepts a Action&lt;object&gt; where object serves as the command parameter
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute) : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        { return _canExecute == null || _canExecute(parameter); }

        public void Execute(object parameter) { _execute(parameter); }

        public void RaiseCanExecuteChanged() { CanExecuteChanged?.Invoke(this, EventArgs.Empty); }
    }

    /// <summary>
    /// An implementation of ICommand which accepts a Action&lt;T&gt; where T serves as the command parameter
    /// </summary>
    public class DelegateCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> execute) : this(execute, null) { }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        { return _canExecute == null || _canExecute((T)parameter); }

        public void Execute(object parameter) { _execute((T)parameter); }

        public void RaiseCanExecuteChanged() { CanExecuteChanged?.Invoke(this, EventArgs.Empty); }
    }
}

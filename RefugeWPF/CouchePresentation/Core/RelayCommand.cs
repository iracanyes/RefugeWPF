using System;
using System.Diagnostics;
using System.Windows.Input;

namespace RefugeWPF.CouchePresentation.Core
{
    class RelayCommand<T>: ICommand
    {
        private readonly Action<T> _execute;

        public RelayCommand(Action<T> execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter) {

            Debug.WriteLine($"RelayCommand.Execute - parameter : {parameter?.GetType()}");

            if(parameter != null)
                _execute((T) parameter);
        }
    }
}

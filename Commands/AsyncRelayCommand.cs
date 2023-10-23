using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Community_House_Management.Commands
{
    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<object, Task> _executeAction;
        private readonly Predicate<object> _canexecuteAction;

        public AsyncRelayCommand(Func<object, Task> executeAction)
        {
            _executeAction = executeAction;
            _canexecuteAction = null;
        }

        public AsyncRelayCommand(Func<object, Task> executeAction, Predicate<object> canexecuteAction)
            : this(executeAction)
        {
            _executeAction = executeAction;
            _canexecuteAction = canexecuteAction;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canexecuteAction == null ? true : _canexecuteAction(parameter);
        }

        public async void Execute(object parameter)
        {
            await _executeAction(parameter);
        }
    }
}

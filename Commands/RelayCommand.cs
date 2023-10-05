using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Community_House_Management.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _executeAction;
        private readonly Predicate<object> _canexecuteAction;

        public RelayCommand(Action<object> executeAction)
        {
            _executeAction = executeAction;
            _canexecuteAction = null;
        }
        public RelayCommand(Action<object> executeAction, Predicate<object> canexecuteAction) : this(executeAction)
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
        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }
    }
}

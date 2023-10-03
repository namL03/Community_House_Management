using Community_House_Management.Stores;
using Community_House_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.Commands
{
    public class NavigateCommand<T> : CommandBase where T : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private Type _objectType;
        public NavigateCommand(NavigationStore navigationStore, Type objectType)
        {
            _navigationStore = navigationStore;
            if (objectType == null || !typeof(T).IsAssignableFrom(objectType))
            {
                throw new ArgumentException("Invalid object type");
            }
            _objectType = objectType;
        }

        public override void Execute(object parameter)
        {

        }
    }
}

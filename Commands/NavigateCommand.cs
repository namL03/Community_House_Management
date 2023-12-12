using Community_House_Management.Models;
using Community_House_Management.Stores;
using Community_House_Management.ViewModels;
using Community_House_Management.ViewModels.StartupViewModels.EventManagementViewModels;
using Community_House_Management.ViewModels.StartupViewModels.ResidentManagementViewModels;
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
        private bool isLoggedIn;

        public NavigateCommand(NavigationStore navigationStore, Type objectType, bool isLoggedIn)
        {
            _navigationStore = navigationStore;
            if (objectType == null || !typeof(T).IsAssignableFrom(objectType))
            {
                throw new ArgumentException("Invalid object type");
            }
            _objectType = objectType;
            this.isLoggedIn = isLoggedIn;
        }

        public override void Execute(object parameter)
        {
            object[] args = new object[] { _navigationStore };
            if (_objectType.Equals(typeof(EventDetailsViewModel)))
            {
                if(parameter is EventModel eventParam)
                {
                    _navigationStore.CurrentViewModel = new EventDetailsViewModel(_navigationStore, eventParam, isLoggedIn);
                }
            }
            else if (_objectType.Equals(typeof(ResidentDetailsViewModel)))
            {
                if(parameter is PersonModel personParam)
                {
                    _navigationStore.CurrentViewModel = new ResidentDetailsViewModel(_navigationStore, personParam, isLoggedIn);
                }
            }
            
            
        }
    }
}

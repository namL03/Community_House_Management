using Community_House_Management.Commands;
using Community_House_Management.Stores;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Community_House_Management.ViewModels.StartupViewModels
{
    public class FacilityManagementViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private bool isLoggedIn;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                if (value != isLoggedIn)
                {
                    isLoggedIn = value;
                    OnPropertyChanged(nameof(IsLoggedIn));
                }
            }
        }
        private bool isAddFacilityClicked;
        public bool IsAddFacilityClicked
        {
            get { return isAddFacilityClicked; }
            set
            {
                isAddFacilityClicked = value;
                OnPropertyChanged(nameof(IsAddFacilityClicked));
            }
        }
        public ICommand OpenAddFacilityCommand { get; }
        public FacilityManagementViewModel(NavigationStore navigationStore, bool isLoggedIn) 
        { 
            _navigationStore = navigationStore;
            this.isLoggedIn = isLoggedIn;
            OpenAddFacilityCommand = new RelayCommand(ExecuteOpenAddFacilityCommand, CanExecuteOpenAddFacilityCommand);
        }
        private void ExecuteOpenAddFacilityCommand(object parameter)
        {
            IsAddFacilityClicked = !IsAddFacilityClicked;
        }
        private bool CanExecuteOpenAddFacilityCommand(object parameter)
        {
            if (isLoggedIn == true) return true;
            else return false;
        }
    }
}

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
    public class HouseholdManagementViewModel : ViewModelBase
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
        private bool isAddHouseholdClicked;
        public bool IsAddHouseholdClicked
        {
            get { return isAddHouseholdClicked; }
            set
            {
                isAddHouseholdClicked = value;
                OnPropertyChanged(nameof(IsAddHouseholdClicked));
            }
        }
        public ICommand OpenAddHouseholdCommand { get; }
        public HouseholdManagementViewModel(NavigationStore navigationStore, bool IsLoggedIn) 
        {
            _navigationStore = navigationStore;
            this.isLoggedIn = IsLoggedIn;
            OpenAddHouseholdCommand = new RelayCommand(ExecuteOpenAddHouseholdCommand, CanExecuteOpenAddHouseholdCommand);
        }
        private void ExecuteOpenAddHouseholdCommand(object parameter)
        {
            IsAddHouseholdClicked = !IsAddHouseholdClicked;
        }
        private bool CanExecuteOpenAddHouseholdCommand(object parameter)
        {
            if (isLoggedIn == true) return true;
            else return false;
        }
    }
}

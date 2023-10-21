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
    public class ResidentManagementViewModel : ViewModelBase
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
        private bool isAddResidentClicked;
        public bool IsAddResidentClicked
        {
            get { return isAddResidentClicked; }
            set 
            { 
                isAddResidentClicked = value;
                OnPropertyChanged(nameof(IsAddResidentClicked));
            }
        }
        public ICommand OpenAddResidentCommand { get; }
        
        public ResidentManagementViewModel(NavigationStore navigationStore, bool IsLoggedIn) 
        {
            //Console.WriteLine("Resident " + IsLoggedIn);
            _navigationStore = navigationStore;
            this.isLoggedIn = IsLoggedIn;
            OpenAddResidentCommand = new RelayCommand(ExecuteOpenAddResidentCommand, CanExecuteOpenAddResidentCommand);
        }
        private void ExecuteOpenAddResidentCommand(object parameter)
        {
            //Console.WriteLine("open");
            IsAddResidentClicked = !IsAddResidentClicked;
        }
        private bool CanExecuteOpenAddResidentCommand(object parameter)
        {
            if (IsLoggedIn == true) return true;
            else return false;
        }
    }
}

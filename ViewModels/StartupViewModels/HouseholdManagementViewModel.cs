using Community_House_Management.Commands;
using Community_House_Management.Services;
using Community_House_Management.Stores;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Community_House_Management.ViewModels.StartupViewModels
{
    public class HouseholdManagementViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private Service service = new Service();
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
        private string headerCitizenId;
        public string HeaderCitizenId
        {
            get { return headerCitizenId;}
            set
            {
                headerCitizenId = value;
                OnPropertyChanged(nameof(HeaderCitizenId));
            }
        }
        private int householdId;
        public int HouseholdId
        {
            get { return householdId; }
            set
            {
                householdId = value;
                OnPropertyChanged(nameof(HouseholdId));
            }
        }
        public ICommand OpenAddHouseholdCommand { get; }
        public ICommand AddNewHouseholdCommand { get; }
        public HouseholdManagementViewModel(NavigationStore navigationStore, bool IsLoggedIn) 
        {
            _navigationStore = navigationStore;
            this.isLoggedIn = IsLoggedIn;
            OpenAddHouseholdCommand = new RelayCommand(ExecuteOpenAddHouseholdCommand, CanExecuteOpenAddHouseholdCommand);
            AddNewHouseholdCommand = new AsyncRelayCommand(ExecuteAddNewHouseholdCommand, CanExecuteAddNewHouseholdCommand);
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
        private async Task ExecuteAddNewHouseholdCommand(object parameter)
        {
            if (CanExecuteAddNewHouseholdCommand(parameter))
            {
                try
                {
                    bool addedSuccessfully = await service.AddNewHouseholdAsync(HeaderCitizenId);
                    Console.WriteLine("HOUSEHOLD IS ADDED");

                    if (addedSuccessfully)
                    {
                        await LoadHousehold();
                        HeaderCitizenId = string.Empty;                  
                    }
                    else
                    {
                        Console.WriteLine("Person with the same Citizen ID already exists.");
                        System.Windows.MessageBox.Show("Person with the same CitizenId already exists.");
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error adding new person: {ex.Message}");
                }
            }
        }
        private async Task LoadHousehold()
        {

        }
        private bool CanExecuteAddNewHouseholdCommand(object parameter)
        {
            return HeaderCitizenId != null && HouseholdId > 0;
        }
    }
}

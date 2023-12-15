using Community_House_Management.Commands;
using Community_House_Management.Models;
using Community_House_Management.Services;
using Community_House_Management.Stores;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Community_House_Management.ViewModels.StartupViewModels.HouseholdManagementViewModels
{
    public class ModifyMemberInformationViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private Service service = new Service();
        private PersonModel _personModel;
        private HouseholdModel _household;
        public HouseholdModel Household
        {
            get { return _household; }
            set 
            { 
                _household = value;
                OnPropertyChanged(nameof(_household));
            }
        }
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
        
        private PersonModel person;
        public PersonModel Person
        {
            get { return person; }
            set 
            { 
                person = value;
                OnPropertyChanged(nameof(Person));
            }
        }
        private PersonModel _header;
        public PersonModel Header
        {
            get { return _header; }
            set 
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        public string PersonName
        {
            get { return _personModel.Name; }
            set { }
        }
        private ObservableCollection<string> stateList;
        public ObservableCollection<string> StateList
        {
            get { return stateList; }
            set
            {
                stateList = value;
                OnPropertyChanged(nameof(StateList));
            }
        }

        private string newStateDisplayed;
        public string NewStateDisplayed
        {
            get { return newStateDisplayed; }
            set
            {
                newStateDisplayed = value;
                OnPropertyChanged(nameof(NewStateDisplayed));
            }
        }
        private int newState;
        public int NewState
        {
            get { return newState; }
            set
            {
                if (NewStateDisplayed == "Sinh hoạt") newState = 1;
                else newState = 0;
            }
        }
        private string newCitizenId;
        public string NewCitizenId
        {
            get { return newCitizenId; }
            set
            {
                newCitizenId = value;
                OnPropertyChanged(nameof(NewCitizenId));
            }
        }
        private string newAddress;
        public string NewAddress
        {
            get { return  newAddress; }    
            set
            {
                newAddress = value;
                OnPropertyChanged(nameof(NewAddress));
            }
        }
        public ICommand ToHouseholdDetailsViewCommand { get; }

        public ICommand SaveChangeInformationCommand { get; }
        public ModifyMemberInformationViewModel(NavigationStore navigationStore, PersonModel personModel, bool isLoggedIn)
        {
            _navigationStore = navigationStore;
            _personModel = personModel;
            this.IsLoggedIn = isLoggedIn;
            ToHouseholdDetailsViewCommand = new RelayCommand(ExecuteToHouseholdDetailsViewCommand);
            SaveChangeInformationCommand = new AsyncRelayCommand(ExecuteSaveChangeInformationCommand);
            _ = LoadPersonInformation();
            StateList = new ObservableCollection<string>
            {
                "Sinh hoạt",
                "Tạm vắng"
            };
            //_ = LoadHousehold();
        }
        private async Task LoadPersonInformation()
        {
            Person = await service.GetPersonByCitizenIdAsync(_personModel.CitizenId);
            Header = Person.Header;
            _ = LoadHousehold();
        }
        private async Task LoadHousehold()
        {
            Household = await service.GetHouseholdAsync(Person.Header.CitizenId);
        }
        private async Task ExecuteSaveChangeInformationCommand(object parameter)
        {
            try
            {
                bool isSaved = await service.ChangePersonInformationAsync(Header.CitizenId, new PersonModel
                {
                    Name = Header.Name,
                    CitizenId = NewCitizenId,
                    Address = NewAddress,
                    State = NewState
                });

                if (isSaved)
                {
                    System.Windows.MessageBox.Show("Changes saved successfully.");
              
                }
                else
                {
                    System.Windows.MessageBox.Show("Error saving changes. Please check your input.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error saving changes: {ex.Message}");
            }
        }


        private void ExecuteToHouseholdDetailsViewCommand(object parameter)
        {
            //Console.WriteLine(_personModel.Header.CitizenId);
            HouseholdDetailsViewModel householdDetailsViewModel = new HouseholdDetailsViewModel(_navigationStore, _household, this.isLoggedIn);
            _navigationStore.CurrentViewModel = householdDetailsViewModel;
        }        
    }
}

using Community_House_Management.Commands;
using Community_House_Management.Models;
using Community_House_Management.Services;
using Community_House_Management.Stores;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
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
    public class HouseholdDetailsViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private HouseholdModel _householdModel;
        private Service service = new Service();
        private HouseholdModel newHousehold;
        public HouseholdModel NewHousehold
        {
            get
            {
                return newHousehold;
            }
            set
            {
                newHousehold = value;
                OnPropertyChanged(nameof(NewHousehold));
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
        public PersonModel Header
        {
            get { return _householdModel.Header; }
            set { }
        }
        private List<PersonModel> members;
        public List<PersonModel> Members
        {
            get { return members; }
            set 
            {
                members = value;
                OnPropertyChanged(nameof(Members));
            }
        }
        private List<PersonModel> newMembers;
        public List<PersonModel> NewMembers
        {
            get { return newMembers; }
            set
            {
                newMembers = value;
                OnPropertyChanged(nameof(NewMembers));
            }
        }
    
        private string enteredCitizenId;
        public string EnteredCitizenId
        {
            get { return enteredCitizenId;}
            set
            {
                enteredCitizenId = value;
                OnPropertyChanged(nameof(EnteredCitizenId));
            }
        }
        private PersonModel personFound;
        public PersonModel PersonFound
        {
            get { return personFound;}
            set
            {
                personFound = value;
                OnPropertyChanged(nameof(PersonFound));
            }
        }
        public ICommand DeleteHouseholdCommand { get; }
        public ICommand ToHouseholdManagementViewCommand { get; }
        public ICommand GetPersonByCitizenIdCommand { get; }
        public ICommand AddPersonToHouseholdCommand { get; }
        public ICommand ToModifyMemberInformationViewCommand { get; }
        public HouseholdDetailsViewModel(NavigationStore navigationStore, HouseholdModel householdModel, bool isLoggedIn)
        {
            _navigationStore = navigationStore;
            this.isLoggedIn = isLoggedIn;
            _householdModel = householdModel;
            NewMembers = new List<PersonModel>();
            DeleteHouseholdCommand = new AsyncRelayCommand(ExecuteDeleteHouseholdCommand);
            ToHouseholdManagementViewCommand = new RelayCommand(ExecuteToHouseholdManagementViewCommand);
            GetPersonByCitizenIdCommand = new AsyncRelayCommand(ExecuteGetPersonByCitizenIdCommand);
            AddPersonToHouseholdCommand = new AsyncRelayCommand(ExecuteAddPersonToHouseholdCommand);
            ToModifyMemberInformationViewCommand = new NavigateCommand<ModifyMemberInformationViewModel>(_navigationStore, typeof(ModifyMemberInformationViewModel), this.isLoggedIn);
            _ = LoadMembers();
        }
        private async Task LoadMembers()
        {
            NewHousehold = await service.GetHouseholdAsync(_householdModel.Header.CitizenId);
            Members = NewHousehold.Members;
            OnPropertyChanged(nameof(Members));
            OnPropertyChanged(nameof(NewHousehold));
        }
        private async Task ExecuteDeleteHouseholdCommand(object parameter)
        {
            bool isDeleted = await service.DeleteHouseholdAsync(Header.CitizenId);
            if (isDeleted)
            {
                HouseholdManagementViewModel householdManagementViewModel = new HouseholdManagementViewModel(_navigationStore, this.IsLoggedIn);
                _navigationStore.CurrentViewModel = householdManagementViewModel;
            }
            else
            {
                System.Windows.MessageBox.Show("ERROR, something went wrong");
            }
        }
        private void ExecuteToHouseholdManagementViewCommand(object parameter)
        {
            HouseholdManagementViewModel householdManagementViewModel = new HouseholdManagementViewModel(_navigationStore, this.isLoggedIn);
            _navigationStore.CurrentViewModel = householdManagementViewModel;
        }
        private async Task ExecuteGetPersonByCitizenIdCommand(object parameter)
        {
            PersonFound = await service.GetPersonByCitizenIdAsync(EnteredCitizenId);
            if(PersonFound == null)
            {
                System.Windows.MessageBox.Show("Citizen ID not found!");
            }
            OnPropertyChanged(nameof(PersonFound));
        }
        private async Task ExecuteAddPersonToHouseholdCommand(object parameter)
        {
            try
            {
                NewMembers.Clear();
                NewMembers.Add(PersonFound);
                bool addedSuccessfully = await service.AddMembersAsync(Header.CitizenId, NewMembers);

                if (addedSuccessfully)
                {                 
                    _householdModel.Members.Add(PersonFound);
                    foreach (var member in _householdModel.Members)
                    {
                        Console.WriteLine(member.Name);
                    }
                    await LoadMembers();
                    EnteredCitizenId = string.Empty;

                    System.Windows.MessageBox.Show("Person has been added successfully");
                    //IsAddResidentClicked = false;
                }
                else
                {
                    System.Windows.MessageBox.Show("Error, please check informations");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error adding new person: {ex.Message}");
            }            
        }
    }
}

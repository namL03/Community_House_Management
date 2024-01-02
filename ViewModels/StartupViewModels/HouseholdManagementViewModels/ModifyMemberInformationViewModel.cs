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
                UpdateNewState();
            }
        }
        private int? newState;
        public int? NewState
        {
            get { return newState; }
            set
            {
                newState = value;
                OnPropertyChanged(nameof(NewState));
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
        private string newName;
        public string NewName
        {
            get { return newName; }
            set
            {
                newName = value;
                OnPropertyChanged(nameof(NewName));
            }
        }
        private List<string> removeMembersList;
        public List<string> RemoveMembersList
        {
            get { return removeMembersList;}
            set
            {
                removeMembersList = value;
                OnPropertyChanged(nameof(RemoveMembersList));
            }
        }
        public ICommand ToHouseholdDetailsViewCommand { get; }
        public ICommand DeleteMemberFromHouseholdCommand { get; }
        public ICommand SaveChangeStateCommand { get; }
        public ModifyMemberInformationViewModel(NavigationStore navigationStore, PersonModel personModel, bool isLoggedIn)
        {
            _navigationStore = navigationStore;
            _personModel = personModel;
            this.IsLoggedIn = isLoggedIn;
            ToHouseholdDetailsViewCommand = new RelayCommand(ExecuteToHouseholdDetailsViewCommand);
            DeleteMemberFromHouseholdCommand = new AsyncRelayCommand(ExecuteDeleteMemberFromHouseholdCommand);
            SaveChangeStateCommand = new AsyncRelayCommand(ExecuteSaveChangeStateCommand);
            RemoveMembersList = new List<string>();
            _ = LoadPersonInformation();
            NewStateDisplayed = _personModel.StateDisplayed;
            StateList = new ObservableCollection<string>
            {
                "Tạm trú",
                "Tạm vắng"
            };
            NewName = _personModel.Name;
            NewAddress = _personModel.Address;
            NewCitizenId = _personModel.CitizenId;
            //_ = LoadHousehold();
        }
        private async Task LoadPersonInformation()
        {
            Person = await service.GetPersonByCitizenIdAsync(_personModel.CitizenId);
            RemoveMembersList.Add(Person.CitizenId);
            if (Person.HeaderId == null) Console.WriteLine("null");
            else Console.WriteLine(Person.HeaderId);
            Header = Person.Header;
            _ = LoadHousehold();
        }
        private async Task LoadHousehold()
        {
            Household = await service.GetHouseholdAsync(Person.Header.CitizenId);
        }
        
        private void ExecuteToHouseholdDetailsViewCommand(object parameter)
        {
            //Console.WriteLine(_personModel.Header.CitizenId);
            HouseholdDetailsViewModel householdDetailsViewModel = new HouseholdDetailsViewModel(_navigationStore, _household, this.isLoggedIn);
            _navigationStore.CurrentViewModel = householdDetailsViewModel;
        }
        private void UpdateNewState()
        {
            // Update NewState based on the selected value in the combo box
            NewState = (NewStateDisplayed == "Tạm trú") ? 1 : 0;
            OnPropertyChanged(nameof(NewStateDisplayed));
            
            Console.WriteLine(NewState);
        }
        private async Task ExecuteDeleteMemberFromHouseholdCommand(object parameter)
        {

            bool isRemoved = await service.RemoveMembersAsync(Header.CitizenId, RemoveMembersList);
            if (isRemoved)
            {
                MessageBox.Show("Loại bỏ nhân khẩu thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                HouseholdDetailsViewModel householdDetailsViewModel = new HouseholdDetailsViewModel(_navigationStore, _household, this.isLoggedIn);
                _navigationStore.CurrentViewModel = householdDetailsViewModel;
            }
            else
            {
                MessageBox.Show("Không thể loại bỏ chủ hộ!", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task ExecuteSaveChangeStateCommand(object parameter)
        {
            _personModel.State = newState;
            bool isSaved = await service.ChangeStateAsync(_personModel);
            if (isSaved)
            {
                MessageBox.Show("Thay đổi trạng thái nhân khẩu thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                HouseholdDetailsViewModel householdDetailsViewModel = new HouseholdDetailsViewModel(_navigationStore, _household, this.isLoggedIn);
                _navigationStore.CurrentViewModel = householdDetailsViewModel;
            }
            else
            {
                //System.Windows.MessageBox.Show("Error saving changes. Please check your input.");
            }
        }
    }
}

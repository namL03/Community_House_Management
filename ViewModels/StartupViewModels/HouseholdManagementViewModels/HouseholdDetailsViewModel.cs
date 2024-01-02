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
        public int? NumberOfMembers => Members == null ? null : Members.Count;
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
                OnPropertyChanged(nameof(NumberOfMembers));
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
        private int enteredState;
        public int EnteredState
        {
            get => enteredState;
            set
            {
                enteredState = value;
                OnPropertyChanged(nameof(EnteredState));
                OnPropertyChanged(nameof(DisplayedState));
            }
        }
        public string DisplayedState
        {
            get
            {
                return EnteredState == 0 ? "Tạm vắng" : "Tạm trú";
            }
            set
            {
                if (value == "Tạm trú") EnteredState = 1;
                else EnteredState = 0;
                OnPropertyChanged(nameof(DisplayedState));
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
        private int numberOfActiveMembers;
        public int NumberOfActiveMembers
        {
            get { return numberOfActiveMembers; }
            set
            {
                numberOfActiveMembers = value;
                OnPropertyChanged(nameof(NumberOfActiveMembers));
            }
        }
        private int numberOfNotActiveMembers;
        public int NumberOfNotActiveMembers
        {
            get { return numberOfNotActiveMembers; }
            set
            {
                numberOfNotActiveMembers = value;
                OnPropertyChanged(nameof(NumberOfNotActiveMembers));
            }
        }
        private bool isStateListEnabled;

        public bool IsStateListEnabled
        {
            get => isStateListEnabled;
            set
            {
                isStateListEnabled = value;
                OnPropertyChanged(nameof(IsStateListEnabled));
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
            DeleteHouseholdCommand = new AsyncRelayCommand(ExecuteDeleteHouseholdCommand, CanExecuteDeleteHouseholdCommand);
            ToHouseholdManagementViewCommand = new RelayCommand(ExecuteToHouseholdManagementViewCommand);
            GetPersonByCitizenIdCommand = new AsyncRelayCommand(ExecuteGetPersonByCitizenIdCommand);
            AddPersonToHouseholdCommand = new AsyncRelayCommand(ExecuteAddPersonToHouseholdCommand, CanExecuteAddPersonToHouseholdCommand);
            //ToModifyMemberInformationViewCommand = new NavigateCommand<ModifyMemberInformationViewModel>(_navigationStore, typeof(ModifyMemberInformationViewModel), this.isLoggedIn);
            ToModifyMemberInformationViewCommand = new RelayCommand(ExecuteToModifyMemberInformationViewCommand, CanExecuteToModifyMemberInformationViewCommand);
            _ = LoadMembers();
            IsStateListEnabled = false;
            EnteredState = 1;
        }
        private void ExecuteToModifyMemberInformationViewCommand(object parameter)
        {
            if (parameter is PersonModel personParam)
            {
                _navigationStore.CurrentViewModel = new ModifyMemberInformationViewModel(_navigationStore, personParam, isLoggedIn);
            }
        }
        private bool CanExecuteToModifyMemberInformationViewCommand(object parameter)
        {
            return IsLoggedIn;
        }
        private bool CanExecuteDeleteHouseholdCommand(object parameter)
        {
            return IsLoggedIn;
        }
        private async Task LoadMembers()
        {
            NewHousehold = await service.GetHouseholdAsync(_householdModel.Header.CitizenId);
            Members = NewHousehold.Members;
            NumberOfActiveMembers = NumberOfNotActiveMembers = 0;
            foreach(var mem in Members)
            {
                if (mem.State == 1)
                {
                    NumberOfActiveMembers += 1;
                }
                else if(mem.State == 0)
                {
                    NumberOfNotActiveMembers += 1;
                }
            }
            OnPropertyChanged(nameof(Members));
            OnPropertyChanged(nameof(NewHousehold));
        }
        private async Task ExecuteDeleteHouseholdCommand(object parameter)
        {
            bool isDeleted = await service.DeleteHouseholdAsync(Header.CitizenId);
            if (isDeleted)
            {
                MessageBox.Show("Xóa hộ gia đình thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                HouseholdManagementViewModel householdManagementViewModel = new HouseholdManagementViewModel(_navigationStore, this.IsLoggedIn);
                _navigationStore.CurrentViewModel = householdManagementViewModel;
            }
            else
            {
                //MessageBox.Show("???", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
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
                IsStateListEnabled = false;
                System.Windows.MessageBox.Show("Citizen ID not found!");
            }
            else
            {
                IsStateListEnabled = true;
            }
            OnPropertyChanged(nameof(PersonFound));
        }
        private async Task ExecuteAddPersonToHouseholdCommand(object parameter)
        {
            try
            {
                PersonFound.State = EnteredState;
                OnPropertyChanged(nameof(PersonFound));
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
                    PersonFound = new PersonModel();
                    IsStateListEnabled = false;
                    MessageBox.Show("Thêm nhân khẩu mới thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    //IsAddResidentClicked = false;
                }
                else
                {
                    MessageBox.Show("Nhân khẩu đã thuộc về một hộ gia đình", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("???", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }            
        }
        private bool CanExecuteAddPersonToHouseholdCommand(object parameter )
        {
            return IsStateListEnabled && IsLoggedIn;
        }
    }
}

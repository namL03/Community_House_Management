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
using System.Windows.Input;

namespace Community_House_Management.ViewModels.StartupViewModels.HouseholdManagementViewModels
{
    public class HouseholdDetailsViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private HouseholdModel _householdModel;
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
            _ = LoadMembers();
        }
        private async Task LoadMembers()
        {
            try
            {
                // Kiểm tra xem householdModel có tồn tại hay không
                if (_householdModel != null)
                {
                    // Gán giá trị của Members từ _householdModel
                    Members = _householdModel.Members;
                    Members.Add(Header);
                    Console.WriteLine("LOADED");
                }
                else
                {
                    Console.WriteLine("householdModel is null. Unable to load members.");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                Console.WriteLine($"Error loading household members: {ex.Message}");
            }
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
                foreach (var member in NewMembers)
                {
                    Console.WriteLine(member.Name);
                }
                Console.WriteLine(Header.CitizenId);
                Console.WriteLine(Header.Name);
                Console.WriteLine(Header.HouseholdId);
                Console.WriteLine(_householdModel.Id);
                bool addedSuccessfully = await service.AddMembersAsync(Header.CitizenId, NewMembers);

                if (addedSuccessfully)
                {
                    await LoadMembers();
                    EnteredCitizenId = string.Empty;
                    NewMembers.Clear();
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
            //if (PersonFound != null)
            //{
            //    //Console.WriteLine(PersonFound.Name);
            //    NewMembers.Clear();
            //    NewMembers.Add(PersonFound);
            //    foreach(var member in NewMembers)
            //    {
            //        Console.WriteLine(member.Name);
            //    }
            //    bool addedSuccessfully = await service.AddMembersAsync(Header.CitizenId, NewMembers);

            //    if (addedSuccessfully)
            //    {
            //        await LoadMembers();
            //        EnteredCitizenId = string.Empty;
            //        NewMembers.Clear();
            //        System.Windows.MessageBox.Show("Person has been added successfully");
            //        //IsAddResidentClicked = false;
            //    }
            //    else
            //    {
            //        System.Windows.MessageBox.Show("Error, please check the information");
            //    }
            //}
            //else Console.WriteLine("EMPTY");
            
        }
    }
}

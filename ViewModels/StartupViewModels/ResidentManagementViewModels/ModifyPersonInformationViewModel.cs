using Community_House_Management.Commands;
using Community_House_Management.Models;
using Community_House_Management.Services;
using Community_House_Management.Stores;
using Community_House_Management.ViewModels.StartupViewModels.HouseholdManagementViewModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Community_House_Management.ViewModels.StartupViewModels.ResidentManagementViewModels
{
    public class ModifyPersonInformationViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private Service service = new Service();
        private PersonModel _personModel;
        public PersonModel personModel
        {
            get => _personModel;
            set
            {
                _personModel = value;
                OnPropertyChanged(nameof(personModel));
                OnPropertyChanged(nameof(PersonCitizenId));
                OnPropertyChanged(nameof(PersonName));
            }
        }
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
            get { return personModel.Name; }
            set { }
        }

        public string PersonCitizenId
        {
            get { return personModel.CitizenId; }
            set { }
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
            get { return newAddress; }
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
        public ICommand ToResidentDetailsViewCommand { get; }

        public ICommand SaveChangeInformationCommand { get; }
        public ModifyPersonInformationViewModel(NavigationStore navigationStore, PersonModel personModel, bool isLoggedIn)
        {
            _navigationStore = navigationStore;
            _personModel = personModel;
            this.IsLoggedIn = isLoggedIn;
            ToResidentDetailsViewCommand = new RelayCommand(ExecuteToResidentDetailsViewCommand);
            SaveChangeInformationCommand = new AsyncRelayCommand(ExecuteSaveChangeInformationCommand, CanExecuteSaveChangeInformationCommand);
            _ = LoadPersonInformation();
            NewName = personModel.Name;
            NewAddress = personModel.Address;
            NewCitizenId = personModel.CitizenId;
        }
        private async Task LoadPersonInformation()
        {
            Person = await service.GetPersonByCitizenIdAsync(personModel.CitizenId);
            Header = Person.Header;
            _ = LoadHousehold();
        }
        private async Task LoadHousehold()
        {
            if(Person.Header != null)
            {
                Household = await service.GetHouseholdAsync(Person.Header.CitizenId);
            }
        }
        private async Task ExecuteSaveChangeInformationCommand(object parameter)
        {

            try
            {
                bool isSaved = await service.ChangePersonInformationAsync(personModel.CitizenId, new PersonModel
                {
                    Name = NewName,
                    CitizenId = NewCitizenId,
                    Address = NewAddress,
                });
                if (isSaved)
                {
                    Console.WriteLine(NewName);
                    Console.WriteLine(NewAddress);
                    Console.WriteLine(NewCitizenId);
                    MessageBox.Show("Thay đổi thông tin cư dân thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    personModel.Name = NewName;
                    personModel.CitizenId = NewCitizenId;
                    personModel.Address = NewAddress;
                    personModel = personModel;
                }
                else
                {
                    MessageBox.Show("Số CCCD đã tồn tại!", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Số CCCD đã tồn tại!", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool CanExecuteSaveChangeInformationCommand(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewName) && !string.IsNullOrWhiteSpace(NewCitizenId);
        }

        private void ExecuteToResidentDetailsViewCommand(object parameter)
        {
            //Console.WriteLine(_personModel.Header.CitizenId);
            ResidentDetailsViewModel residentDetailsViewModel = new ResidentDetailsViewModel(_navigationStore, _personModel, this.isLoggedIn);
            _navigationStore.CurrentViewModel = residentDetailsViewModel;
        }

    }
}


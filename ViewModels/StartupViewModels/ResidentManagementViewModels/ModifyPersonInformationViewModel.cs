using Community_House_Management.Commands;
using Community_House_Management.Models;
using Community_House_Management.Services;
using Community_House_Management.Stores;
using Community_House_Management.ViewModels.StartupViewModels.HouseholdManagementViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Community_House_Management.ViewModels.StartupViewModels.ResidentManagementViewModels
{
    public class ModifyPersonInformationViewModel : ViewModelBase
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
            SaveChangeInformationCommand = new AsyncRelayCommand(ExecuteSaveChangeInformationCommand);
            _ = LoadPersonInformation();
            NewName = _personModel.Name;
            NewAddress = _personModel.Address;
            NewCitizenId = _personModel.CitizenId;
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
                bool isSaved = await service.ChangePersonInformationAsync(_personModel.CitizenId, new PersonModel
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


        private void ExecuteToResidentDetailsViewCommand(object parameter)
        {
            //Console.WriteLine(_personModel.Header.CitizenId);
            ResidentDetailsViewModel residentDetailsViewModel = new ResidentDetailsViewModel(_navigationStore, _personModel, this.isLoggedIn);
            _navigationStore.CurrentViewModel = residentDetailsViewModel;
        }

    }
}


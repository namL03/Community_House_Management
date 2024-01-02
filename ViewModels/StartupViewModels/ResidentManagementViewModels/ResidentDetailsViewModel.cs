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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Community_House_Management.ViewModels.StartupViewModels.ResidentManagementViewModels
{
    public class ResidentDetailsViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private PersonModel _personModel;
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
        public string PersonName
        {
            get { return _personModel.Name; }
            set { }
        }
        public string CitizenId
        {
            get { return _personModel.CitizenId; }
            set { }
        }
        public string Address => _personModel.Address;
        public int? HouseholdId => _personModel.HouseholdId;
        public string IsHeader
        {
            get
            {
                if (_personModel.IsHeader)
                {
                    return "YES";
                }
                else
                {
                    return "NO";
                }
            }
        }
        public PersonModel? Header
        {
            get
            {
                return _personModel.Header;
            }
            set { }
        }
        public string StateDisplayed
        {
            get { return _personModel.StateDisplayed; }
            set { }
        }
        public ICommand ToResidentManagementViewCommand { get; }
        public ICommand DeletePersonCommand { get; }
        public ICommand ToModifyPersonInformationViewCommand { get; }
        public ResidentDetailsViewModel(NavigationStore navigationStore, PersonModel personModel, bool isLoggedIn)
        {
            _personModel = personModel;
            this.isLoggedIn = isLoggedIn;
            _navigationStore = navigationStore;
            ToResidentManagementViewCommand = new RelayCommand(ExecuteToResidentManagementViewCommand);
            ToModifyPersonInformationViewCommand = new RelayCommand(ExecuteToModifyPersonInformationViewCommand, CanExecuteToModifyPersonInformationViewCommand);
            
        }
        private void ExecuteToResidentManagementViewCommand(object parameter)
        {
            ResidentManagementViewModel residentManagementViewModel = new ResidentManagementViewModel(_navigationStore, this.IsLoggedIn);
            _navigationStore.CurrentViewModel = residentManagementViewModel;
        }
        private void ExecuteToModifyPersonInformationViewCommand(object parameter)
        {
            ModifyPersonInformationViewModel modifyPersonInformationViewModel = new ModifyPersonInformationViewModel(_navigationStore, _personModel, this.IsLoggedIn);
            _navigationStore.CurrentViewModel = modifyPersonInformationViewModel;
        }
        private bool CanExecuteToModifyPersonInformationViewCommand(object parameter)
        {
            return this.isLoggedIn;
        }
    }
}


using System;
using System.Windows.Input;
using Community_House_Management.Stores;
using System.Linq;
using Community_House_Management.Commands;
using Community_House_Management.Views.StartupViews;
using Community_House_Management.ViewModels.StartupViewModels;
using System.CodeDom.Compiler;
using Community_House_Management.Views;
using System.Windows;

namespace Community_House_Management.ViewModels
{
    public class StartupViewModel : ViewModelBase
    {

        private readonly NavigationStore _navigationStore;
        private readonly NavigationStore _ownNavigationStore;
        public ViewModelBase CurrentViewModel => _ownNavigationStore.CurrentViewModel;

        private bool isPopupOpen;
        public bool IsPopupOpen
        {
            get { return isPopupOpen; }
            set
            {
                if (value != isPopupOpen)
                {
                    isPopupOpen = value;
                    OnPropertyChanged(nameof(IsPopupOpen));
                }
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
        private bool isMenuButtonClicked;
        public bool IsMenuButtonClicked
        {
            get { return isMenuButtonClicked; }
            set
            {
                if(value != isMenuButtonClicked)
                {
                    isMenuButtonClicked = value;
                    OnPropertyChanged(nameof(IsMenuButtonClicked));
                }
            }
        }
        public ICommand OpenPopupCommand { get; }
        public ICommand ToAccountManagementViewCommand { get;}
        public ICommand OpenMenuCommand { get; }
        public ICommand ToFacilityManagementViewCommand { get; }
        public ICommand ToEventManagementViewCommand { get; }
        public ICommand ToResidentManagementViewCommand { get; }
        public ICommand ToHouseholdManagementViewCommand { get; }
        public ICommand OpenLoginWindowCommand { get; }
        
        public StartupViewModel(NavigationStore navigationStore, bool isLoggedIn)
        {
            this.isLoggedIn = isLoggedIn;
            Console.WriteLine(IsLoggedIn);
            _navigationStore = navigationStore;
            _ownNavigationStore = new NavigationStore();
            OpenPopupCommand = new RelayCommand(ExecuteOpenPopupCommand, CanExecuteOpenPopupCommand);
            OpenMenuCommand = new RelayCommand(ExecuteOpenMenuCommand);
            ToAccountManagementViewCommand = new RelayCommand(ExecuteToAccountManagementViewCommand);
            ToFacilityManagementViewCommand = new RelayCommand(ExecuteToFacilityManagementViewCommand);
            ToEventManagementViewCommand = new RelayCommand(ExecuteToEventManagementViewCommand);
            ToHouseholdManagementViewCommand = new RelayCommand(ExecuteToHouseholdManagementViewCommand);
            ToResidentManagementViewCommand = new RelayCommand(ExecuteToResidentManagementViewCommand);
            OpenLoginWindowCommand = new RelayCommand(ExecuteOpenLoginWindowCommand, CanExecuteOpenLoginWindowCommand);
            

            if (!_ownNavigationStore.ViewModels.Any())
            {
                // Set the default view model the first time
                DefaultScreenViewModel defaultScreenViewModel = new DefaultScreenViewModel(_navigationStore);
                _ownNavigationStore.CurrentViewModel = defaultScreenViewModel;
            }
            _ownNavigationStore.CurrentViewModelChanged += OnCurrentChildViewModelChanged;         
        }

        public void OnCurrentChildViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void ExecuteOpenPopupCommand(object parameter)
        {
            IsPopupOpen = !IsPopupOpen;
            //Console.WriteLine("executed");
        }

        private void ExecuteOpenMenuCommand(object parameter)
        {
            IsMenuButtonClicked = !IsMenuButtonClicked;
            
        }
        private void ExecuteToAccountManagementViewCommand(object parameter)
        {         
            AccountManagementViewModel accountManagementViewModel = new AccountManagementViewModel(_ownNavigationStore);
            _ownNavigationStore.CurrentViewModel = accountManagementViewModel;
        }
        
        private void ExecuteToFacilityManagementViewCommand(Object parameter)
        {
            FacilityManagementViewModel facilityManagementViewModel = new FacilityManagementViewModel(_ownNavigationStore, isLoggedIn);
            _ownNavigationStore.CurrentViewModel = facilityManagementViewModel;
        }
        private void ExecuteToEventManagementViewCommand(Object parameter)
        {
            EventManagementViewModel eventManagementViewModel = new EventManagementViewModel(_ownNavigationStore, isLoggedIn);
            _ownNavigationStore.CurrentViewModel = eventManagementViewModel;

        }
        private void ExecuteToHouseholdManagementViewCommand(Object parameter)
        {
            HouseholdManagementViewModel householdManagementViewModel = new HouseholdManagementViewModel(_ownNavigationStore, isLoggedIn);
            _ownNavigationStore.CurrentViewModel = householdManagementViewModel;
        }
        private void ExecuteToResidentManagementViewCommand(Object parameter)
        {
            ResidentManagementViewModel residentManagementViewModel = new ResidentManagementViewModel(_ownNavigationStore, isLoggedIn);
            _ownNavigationStore.CurrentViewModel = residentManagementViewModel;
        }
        private void ExecuteOpenLoginWindowCommand(Object parameter)
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            LoginView loginView = new LoginView();
            loginView.DataContext = loginViewModel;
            loginView.Show();
        }
        
        private bool CanExecuteOpenLoginWindowCommand(object parameter)
        {
            if (IsLoggedIn == false) return true;
            else return false;
        }
        private bool CanExecuteOpenPopupCommand(object parameter)
        {
            if (IsLoggedIn == false) return false;
            else return true;
        }
    }
}

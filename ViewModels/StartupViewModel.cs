
using System;
using System.Windows.Input;
using Community_House_Management.Stores;
using System.Linq;
using Community_House_Management.Commands;
using Community_House_Management.Views.StartupViews;
using Community_House_Management.ViewModels.StartupViewModels;

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
        public StartupViewModel(NavigationStore navigationStore)
        {
            
            _navigationStore = navigationStore;
            _ownNavigationStore = new NavigationStore();
            OpenPopupCommand = new RelayCommand(ExecuteOpenPopupCommand);
            OpenMenuCommand = new RelayCommand(ExecuteOpenMenuCommand);
            ToAccountManagementViewCommand = new RelayCommand(ExecuteToAccountManagementViewCommand);
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
            Console.WriteLine("executed");
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
        
    }
}

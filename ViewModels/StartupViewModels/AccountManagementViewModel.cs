using Community_House_Management.Commands;
using Community_House_Management.Stores;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Community_House_Management.ViewModels.StartupViewModels
{
    public class AccountManagementViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly NavigationStore _ownNavigationStore;
        private string _oldPassword;
        private string _newPassword;
        private string _confirmPassword;
        public ViewModelBase CurrentViewModel => _ownNavigationStore.CurrentViewModel;
        public string OldPassword
        {
            get
            {
                return _oldPassword;
            }
            set
            {
                _oldPassword = value;
                OnPropertyChanged(nameof(OldPassword));
                
            }
        }

        public string NewPassword
        {
            get
            {
                return _newPassword;
            }
            set
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));

            }
        }

        public string ConfirmPassword
        {
            get
            {
                return _confirmPassword;
            }
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));

            }
        }

        public ICommand ToStartupViewCommand { get; }
        public ICommand ToDefaultScreenViewCommand { get; }
        public AccountManagementViewModel(NavigationStore navigationStore)
        {
            ToDefaultScreenViewCommand = new RelayCommand(ExecuteToDefaultScreenViewCommand);
            _ownNavigationStore = new NavigationStore();
            _navigationStore = navigationStore;
            _ownNavigationStore.CurrentViewModelChanged += OnCurrentChildViewModelChanged;
        }
        public void OnCurrentChildViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
        private void ExecuteToDefaultScreenViewCommand(object parameter)
        {
            DefaultScreenViewModel defaultScreenViewModel = new DefaultScreenViewModel(_navigationStore);
            _navigationStore.CurrentViewModel = defaultScreenViewModel;
        }
    }
}

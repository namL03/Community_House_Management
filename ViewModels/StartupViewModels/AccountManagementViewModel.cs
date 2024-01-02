using Community_House_Management.Commands;
using Community_House_Management.Models;
using Community_House_Management.Services;
using Community_House_Management.Stores;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private AccountModel _officialAccount;
        public AccountModel OfficialAccount
        {
            get => _officialAccount;
            set
            {
                _officialAccount = value;
                OnPropertyChanged(nameof(OfficialAccount));
            }
        }
        public ICommand ToStartupViewCommand { get; }
        public ICommand ToDefaultScreenViewCommand { get; }

        public ICommand ChangePasswordCommand { get; }
        public AccountManagementViewModel(NavigationStore navigationStore, AccountModel officialAccount)
        {
            OfficialAccount = officialAccount;
            ToDefaultScreenViewCommand = new RelayCommand(ExecuteToDefaultScreenViewCommand);
            ChangePasswordCommand = new AsyncRelayCommand(ExecuteChangePasswordCommand, CanExecuteChangePasswordCommand);
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
        private Service services = new Service();
        private async Task ExecuteChangePasswordCommand(object parameter)
        {
            if (OldPassword != OfficialAccount.Password)
            {
                MessageBox.Show("Mật khẩu không đúng!",
                "Thất bại",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
            else if(ConfirmPassword != NewPassword)
            {
                MessageBox.Show("Nhập lại mật khẩu mới không đúng!",
                "Thất bại",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
            else
            {
                await services.ChangePasswordAsync(OfficialAccount.Username, OfficialAccount.Password, NewPassword);
                MessageBox.Show("Đổi mật khẩu thành công",
                "Thành công",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
                OfficialAccount.Password = NewPassword;
                DefaultScreenViewModel startupViewModel = new DefaultScreenViewModel(_navigationStore);
                _navigationStore.CurrentViewModel = startupViewModel;
            } 
                
        }
        private bool CanExecuteChangePasswordCommand(object parameter)
        {
            return !string.IsNullOrWhiteSpace(OldPassword) && !string.IsNullOrWhiteSpace(NewPassword) && !string.IsNullOrWhiteSpace(ConfirmPassword);
        }
    }
}

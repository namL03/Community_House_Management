using Community_House_Management.Commands;
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

namespace Community_House_Management.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _userName;
        private string _password;
        private string _errorMessage;
        private bool _isViewVisible;
        private readonly NavigationStore _navigationStore;
        
        public bool IsViewVisible
        {
            get
            {
                return _isViewVisible;
            }
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }
        private bool _isLoginButtonVisible = true;

        public bool IsLoginButtonVisible
        {
            get { return _isLoginButtonVisible; }
            set
            {
                _isLoginButtonVisible = value;
                OnPropertyChanged(nameof(IsLoginButtonVisible));
            }
        }
        public string UserName 
        { 
            get 
            { 
                return _userName; 
            } 
            set 
            { 
                _userName = value; 
                OnPropertyChanged(nameof(UserName));
                //Console.WriteLine($"UserName changed: {_userName}");
            } 
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set 
            { 
                _password = value; 
                OnPropertyChanged(nameof(Password));
                //Console.WriteLine($"Password changed: {_password}");
            }
        }

        

        public ICommand LoginCommand { get; }
        public ICommand RecoverPasswordCommand { get; }
        public ICommand CloseLoginWindowCommand { get; }
        public LoginViewModel()
        {
            LoginCommand = new AsyncRelayCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            CloseLoginWindowCommand = new RelayCommand(ExecuteCloseLoginWindowCommand);
            _navigationStore = new NavigationStore();
        }

        private bool CanExecuteLoginCommand(object parameter)
        {
            bool validData;
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {             
                validData = false;
                //Console.WriteLine(validData);
            }

            else
            {                
                validData = true;
                //Console.WriteLine(validData);
            }
            return validData;
        }
        private Service service = new Service();
        private async Task ExecuteLoginCommand(object parameter)
        {
            bool isValid = await service.CheckAccountAsync(UserName, Password);
            if (isValid)
            {
                MessageBox.Show("Đăng nhập thành công",
                "Thành công",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.MainWindow.DataContext = new MainViewModel(_navigationStore);
                    _navigationStore.CurrentViewModel = new StartupViewModel(_navigationStore, true, new Models.AccountModel 
                    { 
                        Username = UserName, Password = Password
                    });
                    Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                    IsViewVisible = false;
                });
            }
            else
            {
                MessageBox.Show("Tài khoản không hợp lệ!",
                "Thất bại",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
            
        }
        private void ExecuteCloseLoginWindowCommand(object parameter)
        {
            Console.WriteLine("executed close login window");
            foreach (var window in Application.Current.Windows)
            {
                if (window is LoginView loginView)
                {
                    loginView.Close();
                    break;
                }
            }
        }
    }
}

using Community_House_Management.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Community_House_Management.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _userName;
        private string _password;
        private string _errorMessage;
        private bool _isViewVisible;
        
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
                Console.WriteLine($"UserName changed: {_userName}");
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
                Console.WriteLine($"Password changed: {_password}");
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RecoverPasswordCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            RecoverPasswordCommand = new RelayCommand(p => ExecuteRecoverPasswordCommand("", ""));
        }

        private bool CanExecuteLoginCommand(object parameter)
        {
            bool validData;
            if (string.IsNullOrWhiteSpace(UserName) || UserName.Length < 3 || Password == null || Password.Length < 3)
            {             
                validData = false;
                Console.WriteLine(validData);
            }

            else
            {                
                validData = true;
                Console.WriteLine(validData);
            }
            return validData;
        }
        private void ExecuteLoginCommand(object parameter)
        {
            throw new NotImplementedException();
        }
        private void ExecuteRecoverPasswordCommand(string username, string email)
        {
            throw new NotImplementedException();
        }
    }
}

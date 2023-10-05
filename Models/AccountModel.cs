using Community_House_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.Models
{
    class AccountModel : ViewModelBase
    {
        public int Id { get; set; }
        private string _username;
        private string _password;
        private string _citizenId;
        public int PersonId { get; set; }
        public string CitizenId
        {
            get { return _citizenId; }
            set
            {
                _citizenId = value;
                OnPropertyChanged(nameof(CitizenId));
            }
        }
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
    }
}

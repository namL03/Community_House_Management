using Community_House_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.Models
{
    class PersonModel : ViewModelBase
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        private string _citizenId;
        private string _address;
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string CitizenId
        {
            get { return _citizenId; }
            set
            {
                _citizenId = value;
                OnPropertyChanged(nameof(CitizenId));
            }
        }
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
    }
}

using Community_House_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.Models
{
    public class PersonModel : ViewModelBase
    {
        public int Id { get; set; }
        public int? HouseholdId { get; set; }
        private string _citizenId;
        private string? _address;
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
        public string? Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
        public int? HeaderId { get; set; }
        private PersonModel? _header;
        public PersonModel? Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        private int? _state;
        public int? State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged(nameof(State));
            }
        }
        private bool _isHeader;
        public bool IsHeader
        {
            get { return _isHeader; }
            set { _isHeader = value;
            OnPropertyChanged(nameof(IsHeader));}
        }
    }
}

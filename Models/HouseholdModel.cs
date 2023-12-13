using Community_House_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.Models
{
    public class HouseholdModel : ViewModelBase
    {
        public int Id { get; set; }
        private List<PersonModel> _members;
        public List<PersonModel> Members
        {
            get => _members;
            set
            {
                _members = value;
                OnPropertyChanged(nameof(Members));
            }
        }
        private PersonModel _header;
        public PersonModel Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }
    }
}

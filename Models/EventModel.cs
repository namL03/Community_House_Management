using Community_House_Management.ModelsDb;
using Community_House_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community_House_Management.Models
{
    public class EventModel : ViewModelBase
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        private PersonModel _organizer;
        private string _name;
        private DateTime _timeStart;
        private DateTime _timeEnd;
        private List<PropertyTypeModel> propertyTypes;
        public PersonModel Organizer
        {
            get => _organizer;
            set
            {
                _organizer = value;
                OnPropertyChanged(nameof(Organizer));
            }
        }
        public List<PropertyTypeModel> PropertyTypes
        {
            get => propertyTypes;
            set
            {
                propertyTypes = value;
                OnPropertyChanged(nameof(PropertyTypes));
            }
        }
        public string Name
        {
            get { return _name; }
            set 
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public DateTime TimeStart
        {
            get { return _timeStart; }
            set
            {
                _timeStart = value;
                OnPropertyChanged(nameof(TimeStart));
            }
        }
        public DateTime TimeEnd
        {
            get { return _timeEnd; }
            set
            {
                _timeEnd = value;
                OnPropertyChanged(nameof(TimeEnd));
            }
        }
    }
}

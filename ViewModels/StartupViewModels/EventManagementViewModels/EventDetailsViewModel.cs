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
using System.Windows.Input;

namespace Community_House_Management.ViewModels.StartupViewModels.EventManagementViewModels
{
    public class EventDetailsViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private EventModel _eventModel;
        public int PersonId
        {
            get { return _eventModel.PersonId; }
            set { }
        }
        public string EventName
        {
            get { return _eventModel.Name; }
            set { }
        }
        public DateTime EventStartTime => _eventModel?.TimeStart ?? DateTime.MinValue;
        public DateTime EventEndTime => _eventModel?.TimeEnd ?? DateTime.MinValue;

        public EventDetailsViewModel(NavigationStore navigationStore, EventModel eventModel) 
        {
            _navigationStore = navigationStore;
            _eventModel = eventModel;   
        }
    }
}

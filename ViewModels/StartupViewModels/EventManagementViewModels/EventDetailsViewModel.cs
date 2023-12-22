using Community_House_Management.Commands;
using Community_House_Management.Models;
using Community_House_Management.Services;
using Community_House_Management.Stores;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
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
        private EventModel _eventLoaded;
        public EventModel EventLoaded
        {
            get
            {
                return _eventLoaded;
            }
            set
            {
                _eventLoaded = value;
                OnPropertyChanged(nameof(EventLoaded));
            }
        }
        private Service service = new Service();
        public string OrganizerCitizenId
        {
            get { return _eventModel.Organizer.CitizenId; }
            set { }
        }
        public string OrganizerName
        {
            get { return _eventModel.Organizer.Name; }
            set { }
        }
        public string EventName
        {
            get { return _eventModel.Name; }
            set { }
        }
        private List<PropertyTypeModel> pagedPropertyTypesList;
        public List<PropertyTypeModel> PagedPropertyTypesList
        {
            get { return pagedPropertyTypesList; }
            set
            {
                pagedPropertyTypesList = value;
                OnPropertyChanged(nameof(PagedPropertyTypesList));
            }
        }
        private bool isLoggedIn;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                if (value != isLoggedIn)
                {
                    isLoggedIn = value;
                    OnPropertyChanged(nameof(IsLoggedIn));
                }
            }
        }
        public DateTime EventStartTime => _eventModel?.TimeStart ?? DateTime.MinValue;
        public DateTime EventEndTime => _eventModel?.TimeEnd ?? DateTime.MinValue;
        public ICommand ToAddFacilityToEventViewCommand { get; set; }
        public ICommand ToRemoveFacilityFromEventViewCommand { get; set; }
        public ICommand ToEventManagementViewComamnd { get; }
        public ICommand DeleteEventCommand { get; }

        public EventDetailsViewModel(NavigationStore navigationStore, EventModel eventModel, bool isLoggedIn) 
        {
            this.isLoggedIn = isLoggedIn;
            _navigationStore = navigationStore;
            _eventModel = eventModel;
            ToAddFacilityToEventViewCommand = new RelayCommand(ExecuteToAddFacilityToEventViewCommand, CanExecuteToAddFacilityToEventViewCommand);
            ToEventManagementViewComamnd = new RelayCommand(ExecuteToEventManagementViewComamnd);
            DeleteEventCommand = new AsyncRelayCommand(ExecuteDeleteEventCommand, CanExecuteDeleteEventCommand);
            ToRemoveFacilityFromEventViewCommand = new RelayCommand(ExecuteToRemoveFacilityFromEventViewCommand);
            _ = LoadEvent();
        }
        private async Task LoadEvent()
        {
            EventLoaded = await service.GetEventByIdAsync(_eventModel.Id);
        }
        private void ExecuteToAddFacilityToEventViewCommand(object parameter)
        {
            AddFacilityToEventViewModel addFacilityToEventViewModel = new AddFacilityToEventViewModel(_navigationStore, _eventModel, isLoggedIn);
            _navigationStore.CurrentViewModel = addFacilityToEventViewModel;
        }
        private bool CanExecuteToAddFacilityToEventViewCommand(object parameter)
        {
            return IsLoggedIn;
        }
        private void ExecuteToEventManagementViewComamnd(object parameter)
        {
            EventManagementViewModel eventManagementViewModel = new EventManagementViewModel(_navigationStore, isLoggedIn);
            _navigationStore.CurrentViewModel = eventManagementViewModel;
        }
        private async Task ExecuteDeleteEventCommand(object parameter)
        {
            bool isDeleted = await service.DeleteEventAsync(_eventModel.Id);
            if(isDeleted)
            {
                EventManagementViewModel eventManagementViewModel = new EventManagementViewModel(_navigationStore, this.IsLoggedIn);
                _navigationStore.CurrentViewModel = eventManagementViewModel;
            }
            else
            {
                System.Windows.MessageBox.Show("ERROR, something went wrong");
            }
        }
        private bool CanExecuteDeleteEventCommand(object parameter)
        {
            return isLoggedIn;
        }
        private void ExecuteToRemoveFacilityFromEventViewCommand(object parameter)
        {
            RemoveFacilityFromEventViewModel removeFacilityFromEventViewModel = new RemoveFacilityFromEventViewModel(_navigationStore, _eventModel, this.IsLoggedIn);
            _navigationStore.CurrentViewModel = removeFacilityFromEventViewModel;
        }
    }
}

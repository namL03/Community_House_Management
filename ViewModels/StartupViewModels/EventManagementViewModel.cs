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

namespace Community_House_Management.ViewModels.StartupViewModels
{
    public class EventManagementViewModel : ViewModelBase
    {
        private string name;
        public string Name
        {
            get { return name; } 
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string organizerCitizenId;
        public string OrganizerCitizenId
        {
            get { return organizerCitizenId; }
            set
            {
                organizerCitizenId = value;
                OnPropertyChanged(nameof(OrganizerCitizenId));
            }
        }
        private int selectedStartDay;
        public int SelectedStartDay
        {
            get { return selectedStartDay; }
            set
            {
                selectedStartDay = value;
                OnPropertyChanged(nameof(SelectedStartDay));
            }
        }

        private int selectedStartMonth;
        public int SelectedStartMonth
        {
            get { return selectedStartMonth; }
            set
            {
                selectedStartMonth = value;
                OnPropertyChanged(nameof(SelectedStartMonth));
                UpdateDays();
            }
        }
        private int selectedStartYear;
        public int SelectedStartYear
        {
            get { return selectedStartYear; }
            set
            {
                selectedStartYear = value;
                OnPropertyChanged(nameof(SelectedStartYear));
            }
        }
        private int selectedEndDay;
        public int SelectedEndDay
        {
            get { return selectedEndDay; }
            set
            {
                selectedEndDay = value;
                OnPropertyChanged(nameof(SelectedEndDay));
            }
        }

        private int selectedEndMonth;
        public int SelectedEndMonth
        {
            get { return selectedEndMonth; }
            set
            {
                selectedEndMonth = value;
                OnPropertyChanged(nameof(SelectedEndMonth));
                UpdateDays();
            }
        }
        private int selectedEndYear;
        public int SelectedEndYear
        {
            get { return selectedEndYear; }
            set
            {
                selectedEndYear = value;
                OnPropertyChanged(nameof(SelectedEndYear));
            }
        }
        private List<int> days;
        public List<int> Days
        {
            get { return days; }
            set
            {
                days = value;
                OnPropertyChanged(nameof(Days));
            }
        }
        public IEnumerable<int> Months { get; } = Enumerable.Range(1, 12);
        public IEnumerable<int> Years { get; } = Enumerable.Range(2000, 100);
        private DateTime dateStart;
        public DateTime DateStart
        {
            get { return new DateTime(SelectedStartYear, SelectedStartMonth, SelectedStartDay); }
            set
            {
                dateStart = value;
                OnPropertyChanged(nameof(DateStart));
            }
        }

        private DateTime dateEnd;
        public DateTime DateEnd
        {
            get { return new DateTime(SelectedEndYear, SelectedEndMonth, SelectedEndDay); ; }
            set
            {
                dateEnd = value;
                OnPropertyChanged(nameof(DateEnd));
            }
        }

        private Service services = new Service();
        private IEnumerable<EventModel> events;
        public IEnumerable<EventModel> Events
        {
            get => events;
            set
            {
                events = value;
                OnPropertyChanged(nameof(Events));
            }
        }

        private readonly NavigationStore _navigationStore;
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
        private bool isAddEventClicked;
        public bool IsAddEventClicked
        {
            get { return isAddEventClicked; }
            set
            {
                isAddEventClicked = value;
                OnPropertyChanged(nameof(IsAddEventClicked));
            }
        }
        public ICommand OpenAddEventCommand { get; }
        public ICommand AddEventCommand { get; }
        public EventManagementViewModel(NavigationStore navigationStore, bool isLoggedIn) 
        { 
            _navigationStore = navigationStore;
            this.isLoggedIn = isLoggedIn;
            DateStart = DateTime.Now;
            DateEnd = DateTime.Now;
            OpenAddEventCommand = new RelayCommand(ExecuteOpenAddEventCommand, CanExecuteOpenAddEventCommand);
            AddEventCommand = new AsyncRelayCommand(ExecuteAddEventCommand, CanExecuteAddEventCommand);
            _ = LoadEvents();
        }
        private async Task LoadEvents()
        {
            Events = await services.GetEventsAsync();
        }
        private async Task ExecuteAddEventCommand(object parameter)
        {
            PersonModel creator = await services.GetPersonByCitizenIdAsync(OrganizerCitizenId);
            if(creator == null)
            {
                System.Windows.MessageBox.Show("Error. The CitizenId doesn't exist");
            }
            else
            {
                EventModel eventcreated = new EventModel
                {
                    Name = this.Name,
                    TimeStart = DateStart,
                    TimeEnd = DateEnd,
                    PersonId = creator.Id,
                };
                await services.CreateEventAsync(eventcreated);
                await LoadEvents();
                System.Windows.MessageBox.Show("Event created successfully!");
                DateStart = DateTime.Now;
                DateEnd = DateTime.Now;
                Name = string.Empty;
            }
        }
        private bool CanExecuteAddEventCommand(object parameter)
        {
            return Name != null && DateStart < DateEnd && DateStart > DateTime.Now;
        }
        private void ExecuteOpenAddEventCommand(object parameter)
        {         
            IsAddEventClicked = !IsAddEventClicked;
        }
        private bool CanExecuteOpenAddEventCommand(object parameter)
        {
            if (IsLoggedIn == true) return true;
            else return false;
        }
        private void UpdateDays()
        {
            int daysInMonth = DateTime.DaysInMonth(SelectedStartYear, SelectedStartMonth);
            Days = Enumerable.Range(1, daysInMonth).ToList();
            OnPropertyChanged(nameof(Days));
        }
    }
}

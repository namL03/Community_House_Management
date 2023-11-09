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

        private int startHour;
        public int StartHour
        {
            get { return startHour; }
            set
            {
                startHour = value;
                OnPropertyChanged(nameof(StartHour));
            }
        }
        private int startMinute;
        public int StartMinute
        {
            get { return startMinute; }
            set
            {
                startMinute = value;
                OnPropertyChanged(nameof(StartMinute));
            }
        }
        private int startSecond;
        public int StartSecond
        {
            get { return startSecond; }
            set
            {
                startSecond = value;
                OnPropertyChanged(nameof(StartSecond));
            }
        }
        private int endHour;
        public int EndHour
        {
            get { return endHour; }
            set
            {
                endHour = value;
                OnPropertyChanged(nameof(EndHour));
            }
        }
        private int endMinute;
        public int EndMinute
        {
            get { return endMinute; }
            set
            {
                endMinute = value;
                OnPropertyChanged(nameof(EndMinute));
            }
        }
        private int endSecond;
        public int EndSecond
        {
            get { return endSecond; }
            set
            {
                endSecond = value;
                OnPropertyChanged(nameof(EndSecond));
            }
        }
        public IEnumerable<int> Hours { get; } = Enumerable.Range(0, 24);
        public IEnumerable<int> Minutes { get; } = Enumerable.Range(0, 60);
        public IEnumerable<int> Seconds { get; } = Enumerable.Range(0, 60);
        private DateTime dateStart;
        public DateTime DateStart
        {
            get { return dateStart; }
            set
            {
                if (dateStart != value)
                {
                    dateStart = value;
                    Console.WriteLine(dateStart.ToString());
                    OnPropertyChanged(nameof(DateStart));
                }
            }
        }

        private DateTime dateEnd;
        public DateTime DateEnd
        {
            get { return dateEnd; }
            set
            {
                if (dateEnd != value)
                {
                    dateEnd = value;
                    Console.WriteLine(dateEnd.ToString());
                    OnPropertyChanged(nameof(DateEnd));
                }
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
            dateStart = DateTime.Now;
            dateEnd = DateTime.Now;
            //SelectedEndYear = SelectedStartYear = (int)DateTime.Now.Year;
            //SelectedEndMonth = SelectedStartMonth = (int)DateTime.Now.Month;
            //SelectedEndDay = SelectedStartDay = (int)DateTime.Now.Day;
            OpenAddEventCommand = new RelayCommand(ExecuteOpenAddEventCommand, CanExecuteOpenAddEventCommand);
            AddEventCommand = new AsyncRelayCommand(ExecuteAddEventCommand, CanExecuteAddEventCommand);
            _ = LoadEvents();
        }
        private async Task LoadEvents()
        {
            Console.WriteLine(DateTime.Now.ToString());
            Events = await services.GetEventsAsync();
            //foreach(var e in Events)
            //{
            //    Console.WriteLine(e.TimeStart);
            //    Console.WriteLine(e.TimeEnd);
            //}
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
                    //TimeStart = DateStart,
                    //TimeEnd = DateEnd,
                    TimeStart = new DateTime(DateStart.Year, DateStart.Month, DateStart.Day, StartHour, StartMinute, StartSecond),
                    TimeEnd = new DateTime(DateEnd.Year, DateEnd.Month, DateEnd.Day, EndHour, EndMinute, EndSecond),
                    PersonId = creator.Id,
                };
                await services.CreateEventAsync(eventcreated);
                await LoadEvents();
                System.Windows.MessageBox.Show("Event created successfully!");
                //SelectedEndYear = SelectedStartYear = (int)DateTime.Now.Year;
                //SelectedEndMonth = SelectedStartMonth = (int)DateTime.Now.Month;
                //SelectedEndDay = SelectedStartDay = (int)DateTime.Now.Day;
                Name = string.Empty;
            }
        }
        private bool CanExecuteAddEventCommand(object parameter)
        {
            DateTime startDateWithTime = new DateTime(DateStart.Year, DateStart.Month, DateStart.Day, StartHour, StartMinute, StartSecond);
            DateTime endDateWithTime = new DateTime(DateEnd.Year, DateEnd.Month, DateEnd.Day, EndHour, EndMinute, EndSecond);
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"DateStart: {DateStart}");
            Console.WriteLine($"DateEnd: {DateEnd}");
            Console.WriteLine($"StartHour: {StartHour}");
            Console.WriteLine($"EndHour: {EndHour}");
            Console.WriteLine($"StartMinute: {StartMinute}");
            Console.WriteLine($"EndMinute: {EndMinute}");
            Console.WriteLine($"StartSecond: {StartSecond}");
            Console.WriteLine($"EndSecond: {EndSecond}");
            return Name != null && startDateWithTime < endDateWithTime && startDateWithTime > DateTime.Now;
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
        //private void UpdateDays()
        //{
        //    int daysInMonth = DateTime.DaysInMonth(SelectedStartYear, SelectedStartMonth);
        //    Days = Enumerable.Range(1, daysInMonth).ToList();
        //    OnPropertyChanged(nameof(Days));
        //}
    }
}

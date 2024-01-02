using Community_House_Management.Commands;
using Community_House_Management.Models;
using Community_House_Management.Services;
using Community_House_Management.Stores;
using Community_House_Management.ViewModels.StartupViewModels.EventManagementViewModels;
using Community_House_Management.Views;
using Community_House_Management.Views.StartupViews.EventManagementViews;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

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
        private int numberOfEvent;
        public int NumberOfEvent
        {
            get { return numberOfEvent; }
            set
            {
                numberOfEvent = value;
                OnPropertyChanged(nameof(NumberOfEvent));
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
        private List<EventModel> events;
        public List<EventModel> Events
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
        private List<int> _pageNumbers;
        public List<int> PageNumbers
        {
            get { return _pageNumbers; }
            set
            {
                _pageNumbers = value;
                OnPropertyChanged(nameof(PageNumbers));
            }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                UpdatePagedEventsList();
            }
        }
        private int _selectedNumber;
        public int SelectedNumber
        {
            get
            {
                return _selectedNumber;
            }
            set
            {
                _selectedNumber = value;
                OnPropertyChanged(nameof(SelectedNumber));
            }
        }
        private ObservableCollection<EventModel> pagedEventsList;
        public ObservableCollection<EventModel> PagedEventsList
        {
            get { return pagedEventsList; }
            set
            {
                pagedEventsList = value;
                OnPropertyChanged(nameof(PagedEventsList));
            }
        }
        public List<int> NumberOfPropertyTypes
        {
            get
            {
                List<int> numberOfPropertyTypes = new List<int>();
                if (Events != null)
                {
                    for (int i = 0; i <= Events.Count(); i++)
                    {
                        numberOfPropertyTypes.Add(i);
                    }
                }
                return numberOfPropertyTypes;
            }
        }
        private IEnumerable<EventModel> _filteredList;
        public IEnumerable<EventModel> FilteredList
        {
            get { return _filteredList; }
            set
            {
                _filteredList = value;
                OnPropertyChanged(nameof(FilteredList));
            }
        }
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                while (searchText != string.Empty && searchText.EndsWith(' '))
                {
                    searchText = searchText.Remove(searchText.Length - 1);
                }
                OnPropertyChanged(nameof(SearchText));
                UpdatePagedEventsList();
            }
        }
        private int _eventId;
        public int EventId => _eventId;
        public ICommand OpenAddEventCommand { get; }
        public ICommand AddEventCommand { get; }
        public ICommand ToEventDetailsViewCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ChangePageCommand { get; }
        public ICommand SearchByNameCommand { get; }
        public EventManagementViewModel(NavigationStore navigationStore, bool isLoggedIn) 
        { 
            _navigationStore = navigationStore;
            this.isLoggedIn = isLoggedIn;
            dateStart = DateTime.Now;
            dateEnd = DateTime.Now;
            Name = string.Empty;
            OpenAddEventCommand = new RelayCommand(ExecuteOpenAddEventCommand, CanExecuteOpenAddEventCommand);
            AddEventCommand = new AsyncRelayCommand(ExecuteAddEventCommand, CanExecuteAddEventCommand);
            ToEventDetailsViewCommand = new NavigateCommand<EventDetailsViewModel>(_navigationStore, typeof(EventDetailsViewModel), this.isLoggedIn);
            NextPageCommand = new RelayCommand(ExecuteNextPageCommand);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPageCommand);
            ChangePageCommand = new RelayCommand(ExecuteChangePageCommand);
            SearchByNameCommand = new RelayCommand(ExecuteSearchByNameCommand);
            _ = LoadEvents();    
        }
        private async Task LoadEvents()
        {
            Events = await services.GetEventsAsync();
            FilteredList = Events;
            CurrentPage = 1;
            NumberOfEvent = Events.Count();
            OnPropertyChanged(nameof(NumberOfEvent));
            UpdatePagedEventsList();
            UpdatePageNumbers();
            OnPropertyChanged(nameof(Events));
            OnPropertyChanged(nameof(NumberOfPropertyTypes));
            OnPropertyChanged(nameof(CurrentPage));
        }

        int elementsPerPage = 5;
        private async Task ExecuteAddEventCommand(object parameter)
        {
            PersonModel creator = await services.GetPersonByCitizenIdAsync(OrganizerCitizenId);
            if(creator == null)
            {
                MessageBox.Show("Số CCCD không tồn tại", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                EventModel eventcreated = new EventModel
                {
                    Name = this.Name,
                    TimeStart = new DateTime(DateStart.Year, DateStart.Month, DateStart.Day, StartHour, StartMinute, StartSecond),
                    TimeEnd = new DateTime(DateEnd.Year, DateEnd.Month, DateEnd.Day, EndHour, EndMinute, EndSecond),
                    PersonId = creator.Id,
                };
                await services.CreateEventAsync(eventcreated);
                await LoadEvents();
                MessageBox.Show("Thêm sự kiện mới thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                Name = string.Empty;

            }
        }
        private bool CanExecuteAddEventCommand(object parameter)
        {
            DateTime startDateWithTime = new DateTime(DateStart.Year, DateStart.Month, DateStart.Day, StartHour, StartMinute, StartSecond);
            DateTime endDateWithTime = new DateTime(DateEnd.Year, DateEnd.Month, DateEnd.Day, EndHour, EndMinute, EndSecond);
            return !string.IsNullOrWhiteSpace(Name) && startDateWithTime < endDateWithTime;
        }
        private void ExecuteOpenAddEventCommand(object parameter)
        {         
            IsAddEventClicked = !IsAddEventClicked;
        }
        private bool CanExecuteOpenAddEventCommand(object parameter)
        {
            return IsLoggedIn;
        }
        private void UpdatePagedEventsList()
        {
            int startIndex = (CurrentPage - 1) * elementsPerPage;
            PagedEventsList = new ObservableCollection<EventModel>(FilteredList.Skip(startIndex).Take(elementsPerPage));
        }

        private void UpdatePageNumbers()
        {
            if (Events != null)
            {
                int totalPages = (int)Math.Ceiling((double)Events.Count() / elementsPerPage);
                PageNumbers = Enumerable.Range(1, totalPages).ToList();
            }
            else
            {
                PageNumbers = new List<int>();
            }
        }
        private void UpdatePageNumbersAfterSearch()
        {
            if (FilteredList != null)
            {
                int totalPages = (int)Math.Ceiling((double)FilteredList.Count() / elementsPerPage);
                PageNumbers = Enumerable.Range(1, totalPages).ToList();
            }
            else
            {
                PageNumbers = new List<int>();
            }
        }

        private void ExecuteChangePageCommand(object parameter)
        {
            if (parameter is int page)
            {
                CurrentPage = page;
                UpdatePagedEventsList();
            }
        }
        private void ExecutePreviousPageCommand(object parameter)
        {
            if (CanExecutePreviousPageCommand(parameter))
            {
                CurrentPage--;
                //Console.WriteLine(CurrentPage);
                UpdatePagedEventsList();
            }
        }
        private void ExecuteNextPageCommand(object parameter)
        {
            if (CanExecuteNextPageCommand(parameter))
            {
                CurrentPage++;
                //Console.WriteLine(CurrentPage);
                UpdatePagedEventsList();
            }
        }
        private bool CanExecutePreviousPageCommand(object parameter)
        {
            return CurrentPage > 1;
        }

        private bool CanExecuteNextPageCommand(object parameter)
        {
            return CurrentPage < PageNumbers.Count;
        }
        private void ExecuteSearchByNameCommand(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredList = Events.Where(item => item.Name.Equals(SearchText, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine("filterdList count " + FilteredList.Count());
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    PagedEventsList = new ObservableCollection<EventModel>(FilteredList.Take(elementsPerPage));
                    UpdatePageNumbersAfterSearch();
                });
                OnPropertyChanged(nameof(PagedEventsList));
                CurrentPage = 1;
                UpdatePagedEventsList();
            }
            else
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    FilteredList = Events;
                    PagedEventsList = new ObservableCollection<EventModel>(Events.Take(elementsPerPage));
                    UpdatePageNumbers();
                });
            }
        }
    }
}

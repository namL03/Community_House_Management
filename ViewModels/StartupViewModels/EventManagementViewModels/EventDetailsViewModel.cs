using Community_House_Management.Commands;
using Community_House_Management.Models;
using Community_House_Management.Services;
using Community_House_Management.Stores;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                UpdatePagedPropertyTypesList();
            }
        }
        private List<PropertyTypeModel> propertyTypesList;
        public List<PropertyTypeModel> PropertyTypesList
        {
            get { return propertyTypesList; }
            set
            {
                propertyTypesList = value;
                OnPropertyChanged(nameof(PropertyTypesList));
            }
        }
        private int numberOfProperty;
        public int NumberOfProperty
        {
            get
            {
                return numberOfProperty;
            }
            set
            {
                numberOfProperty = value;
                OnPropertyChanged(nameof(NumberOfProperty));
            }
        }
        public List<int> NumberOfPropertyTypes
        {
            get
            {
                List<int> numberOfPropertyTypes = new List<int>();
                if (PropertyTypesList != null)
                {
                    for (int i = 0; i <= PropertyTypesList.Count(); i++)
                    {
                        numberOfPropertyTypes.Add(i);
                    }
                }
                return numberOfPropertyTypes;
            }
        }
        private ObservableCollection<PropertyTypeModel> pagedPropertyTypesList;
        public ObservableCollection<PropertyTypeModel> PagedPropertyTypesList
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
        private IEnumerable<PropertyTypeModel> _filteredList;
        public IEnumerable<PropertyTypeModel> FilteredList
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
                UpdatePagedPropertyTypesList();
            }
        }
        public DateTime EventStartTime => _eventModel?.TimeStart ?? DateTime.MinValue;
        public DateTime EventEndTime => _eventModel?.TimeEnd ?? DateTime.MinValue;
        public ICommand ToAddFacilityToEventViewCommand { get; set; }
        public ICommand ToRemoveFacilityFromEventViewCommand { get; set; }
        public ICommand ToEventManagementViewComamnd { get; }
        public ICommand DeleteEventCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ChangePageCommand { get; }
        public ICommand SearchByTypeCommand { get; }
        public EventDetailsViewModel(NavigationStore navigationStore, EventModel eventModel, bool isLoggedIn) 
        {
            this.isLoggedIn = isLoggedIn;
            _navigationStore = navigationStore;
            _eventModel = eventModel;
            NextPageCommand = new RelayCommand(ExecuteNextPageCommand);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPageCommand);
            ChangePageCommand = new RelayCommand(ExecuteChangePageCommand);
            ToAddFacilityToEventViewCommand = new RelayCommand(ExecuteToAddFacilityToEventViewCommand, CanExecuteToAddFacilityToEventViewCommand);
            ToEventManagementViewComamnd = new RelayCommand(ExecuteToEventManagementViewComamnd);
            DeleteEventCommand = new AsyncRelayCommand(ExecuteDeleteEventCommand, CanExecuteDeleteEventCommand);
            ToRemoveFacilityFromEventViewCommand = new RelayCommand(ExecuteToRemoveFacilityFromEventViewCommand, CanExecuteToRemoveFacilityFromEventViewCommand);
            SearchByTypeCommand = new RelayCommand(ExecuteSearchByTypeCommand);
            _ = LoadEvent();
        }
        private async Task LoadEvent()
        {
            EventLoaded = await service.GetEventByIdAsync(_eventModel.Id);
            PropertyTypesList = EventLoaded.PropertyTypes;
            FilteredList = PropertyTypesList;
            CurrentPage = 1;
            NumberOfProperty = PropertyTypesList.Count();
            OnPropertyChanged(nameof(NumberOfProperty));
            UpdatePagedPropertyTypesList();
            UpdatePageNumbers();
            OnPropertyChanged(nameof(PropertyTypesList));
            OnPropertyChanged(nameof(NumberOfPropertyTypes));
            OnPropertyChanged(nameof(CurrentPage));
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
                MessageBox.Show("Không thể xóa",
                    "Thất bại",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private bool CanExecuteDeleteEventCommand(object parameter)
        {
            return this.isLoggedIn;
        }
        int elementsPerPage = 5;
        private void ExecuteToRemoveFacilityFromEventViewCommand(object parameter)
        {
            RemoveFacilityFromEventViewModel removeFacilityFromEventViewModel = new RemoveFacilityFromEventViewModel(_navigationStore, _eventModel, this.IsLoggedIn);
            _navigationStore.CurrentViewModel = removeFacilityFromEventViewModel;
        }
        private bool CanExecuteToRemoveFacilityFromEventViewCommand(object parameter)
        {
            return this.isLoggedIn;
        }
        private void UpdatePagedPropertyTypesList()
        {
            int startIndex = (CurrentPage - 1) * elementsPerPage;
            PagedPropertyTypesList = new ObservableCollection<PropertyTypeModel>(FilteredList.Skip(startIndex).Take(elementsPerPage));
        }

        private void UpdatePageNumbers()
        {
            if (PropertyTypesList != null)
            {
                int totalPages = (int)Math.Ceiling((double)PropertyTypesList.Count() / elementsPerPage);
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
                UpdatePagedPropertyTypesList();
            }
        }
        private void ExecutePreviousPageCommand(object parameter)
        {
            if (CanExecutePreviousPageCommand(parameter))
            {
                CurrentPage--;
                //Console.WriteLine(CurrentPage);
                UpdatePagedPropertyTypesList();
            }
        }
        private void ExecuteNextPageCommand(object parameter)
        {
            if (CanExecuteNextPageCommand(parameter))
            {
                CurrentPage++;
                //Console.WriteLine(CurrentPage);
                UpdatePagedPropertyTypesList();
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
        private void ExecuteSearchByTypeCommand(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredList = PropertyTypesList.Where(item => item.Type.Equals(SearchText, StringComparison.OrdinalIgnoreCase));
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    PagedPropertyTypesList = new ObservableCollection<PropertyTypeModel>(FilteredList.Take(elementsPerPage));
                    UpdatePageNumbersAfterSearch();
                });
                OnPropertyChanged(nameof(PagedPropertyTypesList));
                CurrentPage = 1;
                UpdatePagedPropertyTypesList();
            }
            else
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    FilteredList = PropertyTypesList;
                    PagedPropertyTypesList = new ObservableCollection<PropertyTypeModel>(PropertyTypesList.Take(elementsPerPage));
                    UpdatePageNumbers();
                });
            }
        }
    }
}

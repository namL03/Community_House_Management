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
    public class RemoveFacilityFromEventViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private EventModel _eventModel;
        private Service service = new Service();
        private bool isLoggedIn;
        public bool IsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                isLoggedIn = value;
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }
        public string Name
        {
            get { return _eventModel.Name; }
            set { }
        }
        private ObservableCollection<PropertyTypeModel> pagedPropertyTypeOfEventList;
        public ObservableCollection<PropertyTypeModel> PagedPropertyTypeOfEventList
        {
            get { return pagedPropertyTypeOfEventList; }
            set
            {
                pagedPropertyTypeOfEventList = value;
                OnPropertyChanged(nameof(PagedPropertyTypeOfEventList));
            }
        }
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
        private PropertyTypeModel _beRemovedProperty;
        public PropertyTypeModel BeRemovedProperty
        {
            get
            {
                return _beRemovedProperty;
            }
            set
            {
                _beRemovedProperty = value;
                OnPropertyChanged(nameof(BeRemovedProperty));
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
                UpdatePagedPropertyTypesList();
            }
        }
        public List<int> NumberOfPropertyTypes
        {
            get
            {
                List<int> numberOfPropertyTypes = new List<int>();
                if (EventLoaded.PropertyTypes != null)
                {
                    for (int i = 0; i <= EventLoaded.PropertyTypes.Count(); i++)
                    {
                        numberOfPropertyTypes.Add(i);
                    }
                }
                return numberOfPropertyTypes;
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
        private bool _isPropertyPopupOpen;
        public bool IsPropertyPopupOpen
        {
            get { return _isPropertyPopupOpen; }
            set
            {
                if (_isPropertyPopupOpen != value)
                {
                    _isPropertyPopupOpen = value;
                    OnPropertyChanged(nameof(IsPropertyPopupOpen));
                }
            }
        }
        private PropertyTypeModel _selectedProperty;
        public PropertyTypeModel SelectedProperty
        {
            get { return _selectedProperty; }
            set
            {
                _selectedProperty = value;
                OnPropertyChanged(nameof(SelectedProperty));
            }
        }
        private int _quantityOfProperty;
        public int QuantityOfProperty
        {
            get { return _quantityOfProperty; }
            set
            {
                _quantityOfProperty = value;
                OnPropertyChanged(nameof(QuantityOfProperty));
            }
        }
        public ICommand ToEventDetailsViewCommand { get; }
        public ICommand RemoveFacilityFromEventCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ChangePageCommand { get; }
        public ICommand SearchByTypeCommand { get; }
        public ICommand OpenRemovePropertyPopupCommand { get; }
        public RemoveFacilityFromEventViewModel(NavigationStore navigationStore, EventModel eventModel, bool isLoggedIn) 
        {
            _navigationStore = navigationStore;
            _eventModel = eventModel;
            Console.WriteLine(_eventModel.Name);
            Console.WriteLine(_eventModel.Id);
            this.IsLoggedIn = isLoggedIn;
            ToEventDetailsViewCommand = new RelayCommand(ExecuteToEventDetailsViewCommand);
            RemoveFacilityFromEventCommand = new AsyncRelayCommand(ExecuteRemoveFacilityFromEventCommand);
            NextPageCommand = new RelayCommand(ExecuteNextPageCommand);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPageCommand);
            ChangePageCommand = new RelayCommand(ExecuteChangePageCommand);
            SearchByTypeCommand = new RelayCommand(ExecuteSearchByTypeCommand);
            OpenRemovePropertyPopupCommand = new RelayCommand(ExecuteOpenRemovePropertyPopupCommand);
            _ = LoadEvent();
        }
        private async Task LoadEvent()
        {
            EventLoaded = await service.GetEventByIdAsync(_eventModel.Id);
            FilteredList = EventLoaded.PropertyTypes;
            CurrentPage = 1;
            UpdatePagedPropertyTypesList();
            UpdatePageNumbers();
            OnPropertyChanged(nameof(EventLoaded.PropertyTypes));
            OnPropertyChanged(nameof(NumberOfPropertyTypes));
            OnPropertyChanged(nameof(CurrentPage));
        }
        private void ExecuteToEventDetailsViewCommand(object parameter)
        {
            EventDetailsViewModel eventDetailsViewModel = new EventDetailsViewModel(_navigationStore, _eventModel, isLoggedIn);
            _navigationStore.CurrentViewModel = eventDetailsViewModel;
        }
        
        int elementsPerPage = 5;
        private void UpdatePagedPropertyTypesList()
        {
            int startIndex = (CurrentPage - 1) * elementsPerPage;
            PagedPropertyTypeOfEventList = new ObservableCollection<PropertyTypeModel>(FilteredList.Skip(startIndex).Take(elementsPerPage));
        }

        private void UpdatePageNumbers()
        {
            if (EventLoaded.PropertyTypes != null)
            {
                int totalPages = (int)Math.Ceiling((double)EventLoaded.PropertyTypes.Count() / elementsPerPage);
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
            if (!string.IsNullOrEmpty(SearchText))
            {
                FilteredList = EventLoaded.PropertyTypes.Where(item => item.Type.Equals(SearchText, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine("filterdList count " + FilteredList.Count());
                foreach (var e in FilteredList)
                {
                    Console.WriteLine(e.Type + " " + e.Count);
                }
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    PagedPropertyTypeOfEventList = new ObservableCollection<PropertyTypeModel>(FilteredList.Take(elementsPerPage));
                    UpdatePageNumbersAfterSearch();
                });
                OnPropertyChanged(nameof(PagedPropertyTypeOfEventList));
                CurrentPage = 1;
                UpdatePagedPropertyTypesList();
            }
            else
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    FilteredList = EventLoaded.PropertyTypes;
                    PagedPropertyTypeOfEventList = new ObservableCollection<PropertyTypeModel>(EventLoaded.PropertyTypes.Take(elementsPerPage));
                    UpdatePageNumbers();
                });
            }
        }
        private void ExecuteOpenRemovePropertyPopupCommand(object parameter)
        {
            if (parameter is PropertyTypeModel selectedProperty)
            {
                SelectedProperty = selectedProperty;
                IsPropertyPopupOpen = true;
            }
        }
        private async Task ExecuteRemoveFacilityFromEventCommand(object parameter)
        {
            BeRemovedProperty = new PropertyTypeModel();
            BeRemovedProperty.Type = SelectedProperty.Type;
            BeRemovedProperty.Count = QuantityOfProperty;
            bool isRemoved = await service.RemovePropertyFromEventAsync(_eventModel.Id, BeRemovedProperty);
            if (isRemoved)
            {
                System.Windows.MessageBox.Show("Facility is removed successfully!");
                
                await LoadEvent();
            }
            else
            {
                System.Windows.MessageBox.Show("Please check the number again");
            }
        }
        private bool CanExecuteRemoveFacilityFromEventCommand(object parameter)
        {
            return QuantityOfProperty > 0 && QuantityOfProperty <= SelectedProperty.Count;
        }
    }
}

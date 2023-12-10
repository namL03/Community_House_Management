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
    public class AddFacilityToEventViewModel : ViewModelBase
    {

        private readonly NavigationStore _navigationStore;
        private EventModel _eventModel;
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
        public string Name
        {
            get { return _eventModel.Name; }
            set { }
        }
        private Service service = new Service();
        private List<PropertyType> propertyTypesList;
        public List<PropertyType> PropertyTypesList
        {
            get { return propertyTypesList; }
            set
            {
                propertyTypesList = value;
                OnPropertyChanged(nameof(PropertyTypesList));
            }
        }
        private int? count;
        public int? Count
        {
            get { return count; }
            set
            {
                count = value;
                OnPropertyChanged(nameof(Count));
            }
        }
        private string type;
        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
                Console.WriteLine(Type);
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
        private bool _isPropertyPopupOpen;
        public bool IsPropertyPopupOpen
        {
            get { return _isPropertyPopupOpen; }
            set
            {
                if(_isPropertyPopupOpen != value)
                {
                    _isPropertyPopupOpen = value;
                    OnPropertyChanged(nameof(IsPropertyPopupOpen));
                }
            }
        }
        private ObservableCollection<PropertyType> pagedPropertyTypesList;
        public ObservableCollection<PropertyType> PagedPropertyTypesList
        {
            get { return pagedPropertyTypesList; }
            set
            {
                pagedPropertyTypesList = value;
                OnPropertyChanged(nameof(PagedPropertyTypesList));
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

        private IEnumerable<PropertyType> _filteredList;
        public IEnumerable<PropertyType> FilteredList
        {
            get { return _filteredList; }
            set
            {
                _filteredList = value;
                OnPropertyChanged(nameof(FilteredList));
            }
        }
        private PropertyType _selectedProperty;
        public PropertyType SelectedProperty
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
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ChangePageCommand { get; }
        public ICommand SearchByTypeCommand { get; }
        public ICommand OpenAddPropertyPopupCommand { get; }
        public ICommand AddPropertyToEventCommand { get; }
        public ICommand ToEventDetailsViewCommand { get; }
        public AddFacilityToEventViewModel(NavigationStore navigationStore, EventModel eventModel, bool isLoggedIn)
        {
            _eventModel = eventModel;
            _navigationStore = navigationStore;
            this.isLoggedIn = isLoggedIn;
            NextPageCommand = new RelayCommand(ExecuteNextPageCommand);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPageCommand);
            ChangePageCommand = new RelayCommand(ExecuteChangePageCommand);
            SearchByTypeCommand = new RelayCommand(ExecuteSearchByTypeCommand);
            OpenAddPropertyPopupCommand = new RelayCommand(ExecuteOpenAddPropertyPopupCommand);
            AddPropertyToEventCommand = new AsyncRelayCommand(ExecuteAddPropertyToEventCommand);
            ToEventDetailsViewCommand = new RelayCommand(ExecuteToEventDetailsViewCommand);
            _ = LoadProperties();
        }
        
        private async Task LoadProperties()
        {
            PropertyTypesList = await service.GetPropertiesTypeAsync();
            FilteredList = PropertyTypesList;
            CurrentPage = 1;
            UpdatePagedPropertyTypesList();
            UpdatePageNumbers();
            OnPropertyChanged(nameof(PropertyTypesList));
            OnPropertyChanged(nameof(NumberOfPropertyTypes));
            OnPropertyChanged(nameof(CurrentPage));
        }
        int elementsPerPage = 5;
        private void UpdatePagedPropertyTypesList()
        {
            int startIndex = (CurrentPage - 1) * elementsPerPage;
            PagedPropertyTypesList = new ObservableCollection<PropertyType>(FilteredList.Skip(startIndex).Take(elementsPerPage));
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
            if (!string.IsNullOrEmpty(SearchText))
            {
                FilteredList = PropertyTypesList.Where(item => item.Type.Equals(SearchText, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine("filterdList count " + FilteredList.Count());
                foreach (var e in FilteredList)
                {
                    Console.WriteLine(e.Type + " " + e.Count);
                }
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    PagedPropertyTypesList = new ObservableCollection<PropertyType>(FilteredList.Take(elementsPerPage));
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
                    PagedPropertyTypesList = new ObservableCollection<PropertyType>(PropertyTypesList.Take(elementsPerPage));
                    UpdatePageNumbers();
                });
            }
        }
        private void ExecuteOpenAddPropertyPopupCommand(object parameter)
        {
            if (parameter is PropertyType selectedProperty)
            {
                Console.WriteLine(selectedProperty.Type);
                SelectedProperty = selectedProperty;
                IsPropertyPopupOpen = true;
            }
        }
        private void ExecuteToEventDetailsViewCommand(object parameter)
        {
            EventDetailsViewModel eventDetailsViewModel = new EventDetailsViewModel(_navigationStore, _eventModel, isLoggedIn);
            _navigationStore.CurrentViewModel = eventDetailsViewModel;
        }
        private async Task ExecuteAddPropertyToEventCommand(object parameter)
        {
            // Thêm logic để lấy danh sách ID của properties vừa tạo
            //List<int> propertyIds = SelectedProperty.Type.;

            // Thêm properties vào event
            //await service.AddPropertiesToEventAsync(_eventModel.Id, propertyIds);

            // Thêm logic bổ sung nếu cần
        }
    }
}

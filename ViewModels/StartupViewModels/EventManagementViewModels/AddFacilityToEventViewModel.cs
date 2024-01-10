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
        private PropertyTypeModel beAddedProperty;
        public PropertyTypeModel BeAddedProperty
        {
            get
            {
                return beAddedProperty;
            }
            set
            {
                beAddedProperty = value;
                OnPropertyChanged(nameof(BeAddedProperty));
            }
        }
        private bool isSortByNameAscending = true;
        private bool isSortByCountAscending = true;
        public bool IsSortByCountAscending => isSortByCountAscending;
        public bool IsSortByNameAscending => isSortByNameAscending;
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ChangePageCommand { get; }
        public ICommand SearchByTypeCommand { get; }
        public ICommand OpenAddPropertyPopupCommand { get; }
        public ICommand ToEventDetailsViewCommand { get; }
        public ICommand AssignFacilityToEventCommand { get; }
        public ICommand SortByPropertyNameCommand { get; }
        public ICommand SortByPropertyCountCommand { get; }
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
            ToEventDetailsViewCommand = new RelayCommand(ExecuteToEventDetailsViewCommand);
            AssignFacilityToEventCommand = new AsyncRelayCommand(ExecuteAssignFacilityToEventCommand, CanExecuteAssignFacilityToEventCommand);
            SortByPropertyNameCommand = new RelayCommand(ExecuteSortByPropertyNameCommand);
            SortByPropertyCountCommand = new RelayCommand(ExecuteSortByPropertyCountCommand);
            _ = LoadAvaiableProperties();
        }
        
        private async Task LoadAvaiableProperties()
        {
            PropertyTypesList = await service.GetAvailablePropertiesForEvent(_eventModel.Id);
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
                Console.WriteLine("filterdList count " + FilteredList.Count());
                foreach (var e in FilteredList)
                {
                    Console.WriteLine(e.Type + " " + e.Count);
                }
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
        private void ExecuteOpenAddPropertyPopupCommand(object parameter)
        {
            if (parameter is PropertyTypeModel selectedProperty)
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
        private async Task ExecuteAssignFacilityToEventCommand(object parameter)
        {
            BeAddedProperty = new PropertyTypeModel();
            BeAddedProperty.Type = SelectedProperty.Type;
            BeAddedProperty.Count = QuantityOfProperty;
            bool isAdded = await service.AssignPropertyToEventAsync(_eventModel.Id, BeAddedProperty);
            if(isAdded)
            {
                MessageBox.Show("Cấp phát CSVC thành công", 
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                IsPropertyPopupOpen= false;
                await LoadAvaiableProperties();
                _eventModel = await service.GetEventByIdAsync(_eventModel.Id);
            }
            else
            {
                MessageBox.Show("Không đủ số lượng để cấp phát",
                    "Thất bại",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private bool CanExecuteAssignFacilityToEventCommand(object paramter)
        {
            return QuantityOfProperty > 0 && QuantityOfProperty < 1000 && QuantityOfProperty is int;
        }
        private void ExecuteSortByPropertyNameCommand(object parameter)
        {
            // Toggle sorting order
            isSortByNameAscending = !isSortByNameAscending;

            if (isSortByNameAscending)
            {
                FilteredList = FilteredList.OrderBy(item => item.Type, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                FilteredList = FilteredList.OrderByDescending(item => item.Type, StringComparer.OrdinalIgnoreCase);
            }

            UpdatePagedPropertyTypesList();
        }

        private void ExecuteSortByPropertyCountCommand(object parameter)
        {
            // Toggle sorting order
            isSortByCountAscending = !isSortByCountAscending;

            if (isSortByCountAscending)
            {
                FilteredList = FilteredList.OrderBy(item => item.Count);
            }
            else
            {
                FilteredList = FilteredList.OrderByDescending(item => item.Count);
            }

            UpdatePagedPropertyTypesList();
        }
    }
}

using Community_House_Management.Commands;
using Community_House_Management.Services;
using Community_House_Management.Models;
using Community_House_Management.Stores;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Data;
using System.Reflection.Metadata;
using System.Collections.ObjectModel;
using System.Windows;

namespace Community_House_Management.ViewModels.StartupViewModels
{
    public class FacilityManagementViewModel : ViewModelBase
    {
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
        private readonly NavigationStore _navigationStore;
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
        private bool isAddFacilityClicked;
        public bool IsAddFacilityClicked
        {
            get { return isAddFacilityClicked; }
            set
            {
                isAddFacilityClicked = value;
                OnPropertyChanged(nameof(IsAddFacilityClicked));
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
        public ICommand OpenAddFacilityCommand { get; }
        public ICommand AddPropertyCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ChangePageCommand { get; }
        public ICommand SearchByTypeCommand { get; }
        public FacilityManagementViewModel(NavigationStore navigationStore, bool isLoggedIn) 
        {          
            _navigationStore = navigationStore;
            this.isLoggedIn = isLoggedIn;
            OpenAddFacilityCommand = new RelayCommand(ExecuteOpenAddFacilityCommand, CanExecuteOpenAddFacilityCommand);
            NextPageCommand = new RelayCommand(ExecuteNextPageCommand);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPageCommand);
            ChangePageCommand = new RelayCommand(ExecuteChangePageCommand);
            AddPropertyCommand = new AsyncRelayCommand(ExecuteAddPropertyCommand, CanExecuteAddPropertyCommand);
            SearchByTypeCommand = new RelayCommand(ExecuteSearchByTypeCommand);
            _ = LoadProperties();
        }
        private async Task LoadProperties()
        {
            PropertyTypesList = await service.GetPropertiesTypeAsync();           
            CurrentPage = 1;
            UpdatePagedPropertyTypesList();
            UpdatePageNumbers();
            OnPropertyChanged(nameof(PropertyTypesList));
            OnPropertyChanged(nameof(NumberOfPropertyTypes));
            OnPropertyChanged(nameof(CurrentPage));
        }
        private async Task ExecuteAddPropertyCommand(object parameter)
        {
            for(int i=0;i<Count;i++)
            {
                PropertyModel propertyModel = new PropertyModel
                {
                    Type = this.Type
                };
                await service.CreatePropertyAsync(propertyModel);
            }
            await LoadProperties();
        }
        
        private bool CanExecuteAddPropertyCommand(object parameter)
        {
            return Count != null && Type != string.Empty;
        }
        private void ExecuteOpenAddFacilityCommand(object parameter)
        {
            IsAddFacilityClicked = !IsAddFacilityClicked;
        }
        private bool CanExecuteOpenAddFacilityCommand(object parameter)
        {
            if (isLoggedIn == true) return true;
            else return false;
        }
        int elementsPerPage = 5;
        private void UpdatePagedPropertyTypesList()
        {
            int startIndex = (CurrentPage - 1) * elementsPerPage;
            PagedPropertyTypesList = new ObservableCollection<PropertyType>(PropertyTypesList.Skip(startIndex).Take(elementsPerPage));
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
            if(FilteredList != null)
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
                    PagedPropertyTypesList = new ObservableCollection<PropertyType>(PropertyTypesList.Take(elementsPerPage));
                    UpdatePageNumbers();
                });
            }
        }
    }
}

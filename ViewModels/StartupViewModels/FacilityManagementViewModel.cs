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
        private int numberOfPropertyType;
        public int NumberOfPropertyType
        {
            get { return numberOfPropertyType; }
            set
            {
                numberOfPropertyType = value;
                OnPropertyChanged(nameof(NumberOfPropertyType));
            }
        }
        public ICommand OpenAddFacilityCommand { get; }
        public ICommand AddPropertyCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ChangePageCommand { get; }
        public ICommand SearchByTypeCommand { get; }
        public ICommand RemovePropertyCommand { get; }
        public FacilityManagementViewModel(NavigationStore navigationStore, bool isLoggedIn) 
        {
            Type = string.Empty;
            _navigationStore = navigationStore;
            this.isLoggedIn = isLoggedIn;
            OpenAddFacilityCommand = new RelayCommand(ExecuteOpenAddFacilityCommand, CanExecuteOpenAddFacilityCommand);
            NextPageCommand = new RelayCommand(ExecuteNextPageCommand);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPageCommand);
            ChangePageCommand = new RelayCommand(ExecuteChangePageCommand);
            AddPropertyCommand = new AsyncRelayCommand(ExecuteAddPropertyCommand, CanExecuteAddPropertyCommand);
            SearchByTypeCommand = new RelayCommand(ExecuteSearchByTypeCommand);
            RemovePropertyCommand = new AsyncRelayCommand(ExecuteRemovePropertyCommand, CanExecuteRemovePropertyCommand);
            _ = LoadProperties();
        }
        private async Task LoadProperties()
        {
            PropertyTypesList = await service.GetPropertiesTypeAsync();
            FilteredList = PropertyTypesList;
            CurrentPage = 1;
            NumberOfPropertyType = PropertyTypesList.Count();
            OnPropertyChanged(nameof(NumberOfPropertyType));
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
                    Type = this.Type.TrimEnd()
                };
                await service.CreatePropertyAsync(propertyModel);
            }
            MessageBox.Show("Thêm CSVC thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            Type = string.Empty;
            Count = null;
            await LoadProperties();
        }
        
        private bool CanExecuteAddPropertyCommand(object parameter)
        {
            return Count != null && !string.IsNullOrWhiteSpace(Type) && Count > 0;
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
        int elementsPerPage = 10;
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

        private async Task ExecuteRemovePropertyCommand(object parameter)
        {
            if (parameter is PropertyTypeModel propertyTypeModel)
            {
                var associatedEvents = await service.RemovePropertyAsync(propertyTypeModel.Type);
                if (associatedEvents.Count == 0)
                {
                    MessageBox.Show($"Loại bỏ thành công 1 {propertyTypeModel.Type}", "Thành công",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var info = new StringBuilder();
                    foreach (EventModel associatedEvent in associatedEvents)
                    {
                        // Ensure that the date and time are formatted appropriately
                        string formattedStartTime = associatedEvent.TimeStart.ToString(); // or another format string
                        string formattedEndTime = associatedEvent.TimeEnd.ToString();

                        info.AppendLine($"{associatedEvent.Name} diễn ra từ {formattedStartTime} đến {formattedEndTime}");
                    }

                    MessageBox.Show($"Không thể loại bỏ 1 {propertyTypeModel.Type} do đã cấp phát cho các sự kiện sau:\n" +
                        info.ToString(), "Thất bại",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                await LoadProperties();
            }
        }
        private bool CanExecuteRemovePropertyCommand(object parameter)
        {
            return isLoggedIn;
        }
    }
}

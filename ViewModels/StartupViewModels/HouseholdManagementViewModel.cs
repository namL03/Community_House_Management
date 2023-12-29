using Community_House_Management.Commands;
using Community_House_Management.Models;
using Community_House_Management.Services;
using Community_House_Management.Stores;
using Community_House_Management.ViewModels.StartupViewModels.HouseholdManagementViewModels;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Community_House_Management.ViewModels.StartupViewModels
{
    public class HouseholdManagementViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private Service service = new Service();
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
        private bool isAddHouseholdClicked;
        public bool IsAddHouseholdClicked
        {
            get { return isAddHouseholdClicked; }
            set
            {
                isAddHouseholdClicked = value;
                OnPropertyChanged(nameof(IsAddHouseholdClicked));
            }
        }
        private PersonModel header;
        public PersonModel Header
        {
            get { return header; }
            set
            {
                header = value;
                OnPropertyChanged(nameof(Header));
            }
        }
        private string headerCitizenId;
        public string HeaderCitizenId
        {
            get { return headerCitizenId;}
            set
            {
                headerCitizenId = value;
                OnPropertyChanged(nameof(HeaderCitizenId));
            }
        }
       
        private int householdId;
        public int HouseholdId
        {
            get { return householdId; }
            set
            {
                householdId = value;
                OnPropertyChanged(nameof(HouseholdId));
            }
        }
        private int id;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        private string citizenId;
        public string CitizenId
        {
            get { return citizenId; }
            set
            {
                citizenId = value;
                OnPropertyChanged(nameof(CitizenId));
            }
        }
        private List<HouseholdModel> householdList;
        public List<HouseholdModel> HouseholdList
        {
            get { return householdList; }
            set
            {
                householdList = value;
                OnPropertyChanged(nameof(HouseholdList));
            }
        }
        private ObservableCollection<HouseholdModel> pagedHouseholdList;
        public ObservableCollection<HouseholdModel> PagedHouseholdList
        {
            get { return pagedHouseholdList;}
            set
            {
                pagedHouseholdList = value;
                OnPropertyChanged(nameof(PagedHouseholdList));
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
                UpdatePagedHouseholdList();
            }
        }
        public List<int> NumberOfHouseholds
        {
            get
            {
                List<int> numberOfHousehold = new List<int>();
                if (HouseholdList != null)
                {
                    for (int i = 0; i <= HouseholdList.Count(); i++)
                    {
                        numberOfHousehold.Add(i);
                    }
                }
                return numberOfHousehold;
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
        private IEnumerable<HouseholdModel> _filteredList;
        public IEnumerable<HouseholdModel> FilteredList
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
                UpdatePagedHouseholdList();
            }
        }
        private int numberOfHousehold;
        public int NumberOfHousehold
        {
            get { return numberOfHousehold; }
            set
            {
                numberOfHousehold = value;
                OnPropertyChanged(nameof(NumberOfHousehold));
            }
        }
        public ICommand OpenAddHouseholdCommand { get; }
        public ICommand AddNewHouseholdCommand { get; }
        public ICommand ToHouseholdDetailsViewCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ChangePageCommand { get; }
        public ICommand SearchByCitizenIdCommand { get; }
        public HouseholdManagementViewModel(NavigationStore navigationStore, bool IsLoggedIn) 
        {
            _navigationStore = navigationStore;
            this.isLoggedIn = IsLoggedIn;
            OpenAddHouseholdCommand = new RelayCommand(ExecuteOpenAddHouseholdCommand, CanExecuteOpenAddHouseholdCommand);
            AddNewHouseholdCommand = new AsyncRelayCommand(ExecuteAddNewHouseholdCommand, CanExecuteAddNewHouseholdCommand);
            ToHouseholdDetailsViewCommand = new NavigateCommand<HouseholdDetailsViewModel>(_navigationStore, typeof(HouseholdDetailsViewModel), this.isLoggedIn);
            NextPageCommand = new RelayCommand(ExecuteNextPageCommand);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPageCommand);
            ChangePageCommand = new RelayCommand(ExecuteChangePageCommand);
            SearchByCitizenIdCommand = new RelayCommand(ExecuteSearchByCitizenIdCommand);
            _ = LoadHousehold();
        }
        private void ExecuteOpenAddHouseholdCommand(object parameter)
        {
            IsAddHouseholdClicked = !IsAddHouseholdClicked;
        }
        private bool CanExecuteOpenAddHouseholdCommand(object parameter)
        {
            if (isLoggedIn == true) return true;
            else return false;
        }
        private async Task ExecuteAddNewHouseholdCommand(object parameter)
        {
            if (CanExecuteAddNewHouseholdCommand(parameter))
            {
                try
                {
                    bool addedSuccessfully = await service.AddNewHouseholdAsync(HeaderCitizenId);
                    Console.WriteLine("HOUSEHOLD IS ADDED");

                    if (addedSuccessfully)
                    {
                        await LoadHousehold();
                        HeaderCitizenId = string.Empty;
                        System.Windows.MessageBox.Show("Household has been added successfully");
                    }
                    else
                    {
                        Console.WriteLine("Person with the same Citizen ID already exists.");
                        System.Windows.MessageBox.Show("Person with the same CitizenId already exists.");
                    }
                }
                catch (Exception ex)
                {
                    //System.Windows.MessageBox.Show($"Error adding new person: {ex.Message}");
                    System.Windows.MessageBox.Show("Person with the same Citizen ID already exists.");
                }
            }
        }
        private bool CanExecuteAddNewHouseholdCommand(object parameter)
        {
            return HeaderCitizenId != null;
        }
        private async Task LoadHousehold()
        {
            HouseholdList = await service.GetAllHouseholdsAsync();
            NumberOfHousehold = HouseholdList.Count();
            FilteredList = HouseholdList;
            CurrentPage = 1;
            OnPropertyChanged(nameof(NumberOfHousehold));
            UpdatePagedHouseholdList();
            UpdatePageNumbers();
            //OnPropertyChanged(nameof(NumberOfPropertyTypes));
            OnPropertyChanged(nameof(CurrentPage));
            OnPropertyChanged(nameof(HouseholdList));
        }
        
        int elementsPerPage = 5;
        private void UpdatePagedHouseholdList()
        {
            int startIndex = (CurrentPage - 1) * elementsPerPage;
            PagedHouseholdList = new ObservableCollection<HouseholdModel>(FilteredList.Skip(startIndex).Take(elementsPerPage));
        }
        private void UpdatePageNumbers()
        {
            if (HouseholdList != null)
            {
                int totalPages = (int)Math.Ceiling((double)HouseholdList.Count() / elementsPerPage);
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
                UpdatePagedHouseholdList();
            }
        }
        private void ExecutePreviousPageCommand(object parameter)
        {
            if (CanExecutePreviousPageCommand(parameter))
            {
                CurrentPage--;
                //Console.WriteLine(CurrentPage);
                UpdatePagedHouseholdList();
            }
        }
        private void ExecuteNextPageCommand(object parameter)
        {
            if (CanExecuteNextPageCommand(parameter))
            {
                CurrentPage++;
                //Console.WriteLine(CurrentPage);
                UpdatePagedHouseholdList();
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
        private void ExecuteSearchByCitizenIdCommand(object parameter)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                FilteredList = HouseholdList.Where(item => item.Header.CitizenId.Equals(SearchText, StringComparison.OrdinalIgnoreCase));

                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    PagedHouseholdList = new ObservableCollection<HouseholdModel>(FilteredList.Take(elementsPerPage));
                    UpdatePageNumbersAfterSearch();                   
                });
                OnPropertyChanged(nameof(PagedHouseholdList));
                CurrentPage = 1;
                UpdatePagedHouseholdList();
            }
            else
            {
                Console.WriteLine("else");
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    FilteredList = HouseholdList;
                    PagedHouseholdList = new ObservableCollection<HouseholdModel>(HouseholdList.Take(elementsPerPage));
                    UpdatePageNumbers();
                });
            }
        }
    }
}

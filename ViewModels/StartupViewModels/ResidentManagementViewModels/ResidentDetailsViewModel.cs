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


namespace Community_House_Management.ViewModels.StartupViewModels.ResidentManagementViewModels
{
    public class ResidentDetailsViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private PersonModel _personModel;
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
        public string PersonName
        {
            get { return _personModel.Name; }
            set { }
        }
        public string CitizenId
        {
            get { return _personModel.CitizenId; }
            set { }
        }
        public string Address => _personModel.Address;
        public int? HouseholdId => _personModel.HouseholdId;
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
        public PersonModel? Header
        {
            get
            {
                return _personModel.Header;
            }
            set { }
        }
        public string StateDisplayed
        {
            get { return _personModel.StateDisplayed; }
            set { }
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
        public ICommand ToResidentManagementViewCommand { get; }
        public ICommand DeletePersonCommand { get; }
        public ICommand ToModifyPersonInformationViewCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ChangePageCommand { get; }
        public ICommand SearchByNameCommand { get; }
        public ResidentDetailsViewModel(NavigationStore navigationStore, PersonModel personModel, bool isLoggedIn)
        {
            _personModel = personModel;
            if(personModel.Events==null) { Console.WriteLine(1); }
            this.isLoggedIn = isLoggedIn;
            _navigationStore = navigationStore;
            ToResidentManagementViewCommand = new RelayCommand(ExecuteToResidentManagementViewCommand);
            ToModifyPersonInformationViewCommand = new RelayCommand(ExecuteToModifyPersonInformationViewCommand, CanExecuteToModifyPersonInformationViewCommand);
            NextPageCommand = new RelayCommand(ExecuteNextPageCommand);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPageCommand);
            ChangePageCommand = new RelayCommand(ExecuteChangePageCommand);
            SearchByNameCommand = new RelayCommand(ExecuteSearchByNameCommand);
            LoadEvents();
        }
        private void LoadEvents()
        {
            Events = _personModel != null ? _personModel.Events : new List<EventModel>();
            FilteredList = Events.OrderBy(item => item.TimeStart);
            CurrentPage = 1;
            NumberOfEvent = Events.Count();
            OnPropertyChanged(nameof(NumberOfEvent));
            UpdatePagedEventsList();
            UpdatePageNumbers();
            OnPropertyChanged(nameof(Events));
            OnPropertyChanged(nameof(CurrentPage));
        }

        int elementsPerPage = 5;
        private void ExecuteToResidentManagementViewCommand(object parameter)
        {
            ResidentManagementViewModel residentManagementViewModel = new ResidentManagementViewModel(_navigationStore, this.IsLoggedIn);
            _navigationStore.CurrentViewModel = residentManagementViewModel;
        }
        private void ExecuteToModifyPersonInformationViewCommand(object parameter)
        {
            ModifyPersonInformationViewModel modifyPersonInformationViewModel = new ModifyPersonInformationViewModel(_navigationStore, _personModel, this.IsLoggedIn);
            _navigationStore.CurrentViewModel = modifyPersonInformationViewModel;
        }
        private bool CanExecuteToModifyPersonInformationViewCommand(object parameter)
        {
            return this.isLoggedIn;
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

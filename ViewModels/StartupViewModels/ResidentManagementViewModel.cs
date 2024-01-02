using Community_House_Management.Commands;
using Community_House_Management.Models;
using Community_House_Management.Services;
using Community_House_Management.Stores;
using Community_House_Management.ViewModels.StartupViewModels.EventManagementViewModels;
using Community_House_Management.ViewModels.StartupViewModels.ResidentManagementViewModels;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Community_House_Management.ViewModels.StartupViewModels
{
    public class ResidentManagementViewModel : ViewModelBase
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
        private bool isAddResidentClicked;
        public bool IsAddResidentClicked
        {
            get { return isAddResidentClicked; }
            set
            {
                isAddResidentClicked = value;
                OnPropertyChanged(nameof(IsAddResidentClicked));
            }
        }
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
        private string? address;
        public string? Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
        private List<PersonModel> peopleList = new List<PersonModel>();
        public List<PersonModel> PeopleList
        {
            get { return peopleList; }
            set
            {
                peopleList = value;
                OnPropertyChanged(nameof(PeopleList));
                OnPropertyChanged(nameof(NumberOfResident));
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
        private ObservableCollection<PersonModel> pagedPeopleList;
        public ObservableCollection<PersonModel> PagedPeopleList
        {
            get { return pagedPeopleList; }
            set
            {
                pagedPeopleList = value;
                OnPropertyChanged(nameof(PagedPeopleList));
            }
        }
        private IEnumerable<PersonModel> _filteredList;
        public IEnumerable<PersonModel> FilteredList
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
        public int NumberOfResident => peopleList.Count;
        public ICommand OpenAddResidentCommand { get; }
        public ICommand AddNewPersonCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand ChangePageCommand { get; }
        public ICommand SearchByCitizenIdCommand { get; }
        public ICommand ToResidentDetailsViewCommand { get; }

        public ResidentManagementViewModel(NavigationStore navigationStore, bool IsLoggedIn)
        {
            //Console.WriteLine("Resident " + IsLoggedIn);
            Name = string.Empty;
            CitizenId = string.Empty;
            _navigationStore = navigationStore;
            this.isLoggedIn = IsLoggedIn;
            OpenAddResidentCommand = new RelayCommand(ExecuteOpenAddResidentCommand, CanExecuteOpenAddResidentCommand);
            AddNewPersonCommand = new AsyncRelayCommand(ExecuteAddNewPersonCommand, CanExecuteAddNewPersonCommand);
            NextPageCommand = new RelayCommand(ExecuteNextPageCommand);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPageCommand);
            ChangePageCommand = new RelayCommand(ExecuteChangePageCommand);
            SearchByCitizenIdCommand = new RelayCommand(ExecuteSearchByCitizenIdCommand);
            ToResidentDetailsViewCommand = new NavigateCommand<ResidentDetailsViewModel>(_navigationStore, typeof(ResidentDetailsViewModel), this.isLoggedIn);
            _ = LoadPeople();

        }

        private async Task ExecuteAddNewPersonCommand(object parameter)
        {
            if (CanExecuteAddNewPersonCommand(parameter))
            {
                try
                {
                    bool addedSuccessfully = await service.AddNewPersonAsync(Name, Address, CitizenId);

                    if (addedSuccessfully)
                    {
                        await LoadPeople();
                        Name = string.Empty;
                        Address = string.Empty;
                        CitizenId = string.Empty;
                        MessageBox.Show("Thêm cư dân thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                        //IsAddResidentClicked = false;
                    }
                    else
                    {
                        Console.WriteLine("Person with the same Citizen ID already exists.");
                        MessageBox.Show("Số CCCD đã tồn tại!", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
                catch (Exception ex)
                {
                    //System.Windows.MessageBox.Show($"Error adding new person: {ex.Message}");
                }
            }
        }
        private bool CanExecuteAddNewPersonCommand(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(CitizenId);
        }
        private async Task LoadPeople()
        {
            PeopleList = await service.GetPeopleAsync();
            FilteredList = PeopleList;
            CurrentPage = 1;
            UpdatePagedEventsList();
            UpdatePageNumbers();
            OnPropertyChanged(nameof(PeopleList));
            //OnPropertyChanged(nameof(NumberOfPropertyTypes));
            OnPropertyChanged(nameof(CurrentPage));
        }
        private void ExecuteOpenAddResidentCommand(object parameter)
        {
            //Console.WriteLine("open");
            IsAddResidentClicked = !IsAddResidentClicked;
        }
        private bool CanExecuteOpenAddResidentCommand(object parameter)
        {
            if (IsLoggedIn == true) return true;
            else return false;
        }
        int elementsPerPage = 5;
        private void UpdatePagedEventsList()
        {
            int startIndex = (CurrentPage - 1) * elementsPerPage;
            PagedPeopleList = new ObservableCollection<PersonModel>(FilteredList.Skip(startIndex).Take(elementsPerPage));
        }

        private void UpdatePageNumbers()
        {
            if (PeopleList != null)
            {
                int totalPages = (int)Math.Ceiling((double)PeopleList.Count() / elementsPerPage);
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
        private void ExecuteSearchByCitizenIdCommand(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredList = PeopleList.Where(item => item.CitizenId.Equals(SearchText, StringComparison.OrdinalIgnoreCase));
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    PagedPeopleList = new ObservableCollection<PersonModel>(FilteredList.Take(elementsPerPage));
                    UpdatePageNumbersAfterSearch();
                });
                OnPropertyChanged(nameof(PagedPeopleList));
                CurrentPage = 1;
                UpdatePagedEventsList();
            }
            else
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    FilteredList = PeopleList;
                    PagedPeopleList = new ObservableCollection<PersonModel>(PeopleList.Take(elementsPerPage));
                    UpdatePageNumbers();
                });
            }
        }
    }
}
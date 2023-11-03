using Community_House_Management.Commands;
using Community_House_Management.Stores;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        private DateTime dateStart;
        public DateTime DateStart
        {
            get { return dateStart; }
            set
            {
                dateStart = value;
                OnPropertyChanged(nameof(DateStart));
            }
        }

        private DateTime dateEnd;
        public DateTime DateEnd
        {
            get { return dateEnd; }
            set
            {
                dateEnd = value;
                OnPropertyChanged(nameof(DateEnd));
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
        public ICommand OpenAddEventCommand { get; }
        public EventManagementViewModel(NavigationStore navigationStore, bool isLoggedIn) 
        { 
            _navigationStore = navigationStore;
            this.isLoggedIn = isLoggedIn;
            OpenAddEventCommand = new RelayCommand(ExecuteOpenAddEventCommand, CanExecuteOpenAddEventCommand);
        }
        private void ExecuteOpenAddEventCommand(object parameter)
        {         
            IsAddEventClicked = !IsAddEventClicked;
        }
        private bool CanExecuteOpenAddEventCommand(object parameter)
        {
            if (IsLoggedIn == true) return true;
            else return false;
        }

    }
}

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

namespace Community_House_Management.ViewModels.StartupViewModels
{
    public class FacilityManagementViewModel : ViewModelBase
    {
        private Service service = new Service();
        private List<PropertyType> propertyTypes;
        public List<PropertyType> PropertyTypes
        {
            get { return propertyTypes; }
            set
            {
                propertyTypes = value;
                OnPropertyChanged(nameof(PropertyTypes));
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
        public ICommand OpenAddFacilityCommand { get; }
        public ICommand AddPropertyCommand { get; }
        public FacilityManagementViewModel(NavigationStore navigationStore, bool isLoggedIn) 
        {
            _ = LoadProperties();
            _navigationStore = navigationStore;
            this.isLoggedIn = isLoggedIn;
            OpenAddFacilityCommand = new RelayCommand(ExecuteOpenAddFacilityCommand, CanExecuteOpenAddFacilityCommand);
            AddPropertyCommand = new AsyncRelayCommand(ExecuteAddPropertyCommand, CanExecuteAddPropertyCommand);
        }
        private async Task LoadProperties()
        {
            PropertyTypes = await service.GetPropertiesTypeAsync();
            OnPropertyChanged(nameof(PropertyTypes));
            foreach (var propertyType in PropertyTypes)
            {
                Console.WriteLine(propertyType.Type);
                Console.WriteLine(propertyType.Count);
            }
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
        private bool canAddProperty => (Count != null && Type != string.Empty);
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
    }
}

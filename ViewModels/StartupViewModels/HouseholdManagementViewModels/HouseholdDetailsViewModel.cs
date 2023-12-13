using Community_House_Management.Commands;
using Community_House_Management.Models;
using Community_House_Management.Services;
using Community_House_Management.Stores;
using Community_House_Management.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Community_House_Management.ViewModels.StartupViewModels.HouseholdManagementViewModels
{
    public class HouseholdDetailsViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private HouseholdModel _householdModel;
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
        public PersonModel Header
        {
            get { return _householdModel.Header; }
            set { }
        }
        private List<PersonModel> members;
        public List<PersonModel> Members
        {
            get { return members; }
            set 
            {
                members = value;
                OnPropertyChanged(nameof(Members));
            }
        }
        public ICommand DeleteHouseholdCommand { get; }
        public HouseholdDetailsViewModel(NavigationStore navigationStore, HouseholdModel householdModel, bool isLoggedIn)
        {
            _navigationStore = navigationStore;
            this.isLoggedIn = isLoggedIn;
            _householdModel = householdModel;
            DeleteHouseholdCommand = new AsyncRelayCommand(ExecuteDeleteHouseholdCommand);
            _ = LoadMembers();
        }
        private async Task LoadMembers()
        {
            try
            {
                // Kiểm tra xem householdModel có tồn tại hay không
                if (_householdModel != null)
                {
                    // Gán giá trị của Members từ _householdModel
                    Members = _householdModel.Members;
                    Members.Add(Header);
                    Console.WriteLine("LOADED");
                }
                else
                {
                    Console.WriteLine("householdModel is null. Unable to load members.");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                Console.WriteLine($"Error loading household members: {ex.Message}");
            }
        }
        private async Task ExecuteDeleteHouseholdCommand(object parameter)
        {
            bool isDeleted = await service.DeletePersonAsync(Header.CitizenId);
            if (isDeleted)
            {
                HouseholdManagementViewModel householdManagementViewModel = new HouseholdManagementViewModel(_navigationStore, this.IsLoggedIn);
                _navigationStore.CurrentViewModel = householdManagementViewModel;
            }
            else
            {
                System.Windows.MessageBox.Show("ERROR, something went wrong");
            }
        }
    }
}

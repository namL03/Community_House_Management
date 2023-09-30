
using System;
using System.Windows.Input;
using Community_House_Management.Stores;
using System.Linq;

namespace Community_House_Management.ViewModels
{
    public class StartupViewModel : ViewModelBase
    {
        public ICommand ToTabbedViewCommand { get; }
        public ICommand ToNewQuizViewCommand { get; }

        private readonly NavigationStore _navigationStore;
        private readonly NavigationStore _ownNavigationStore;
        public ViewModelBase CurrentViewModel => _ownNavigationStore.CurrentViewModel;
        public StartupViewModel(NavigationStore navigationStore)
        {
            
        }
        public void OnCurrentChildViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}

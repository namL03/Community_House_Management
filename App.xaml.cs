using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Community_House_Management;
using Community_House_Management.Stores;
using Community_House_Management.ViewModels;

namespace Community_House_Management
{
    public static class ConsoleHelper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        public static void Initialize()
        {
            AllocConsole();
            Console.WriteLine("Console Initialized!");
        }
    }
    public partial class App : Application
    {
        public static string GetDatabaseFilePath()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string databaseDirectory = Path.Combine(appDirectory, "Database");
            if (!Directory.Exists(databaseDirectory))
            {
                Directory.CreateDirectory(databaseDirectory);
            }
            return Path.Combine(databaseDirectory, "CommunityHouse.mdf");
        }
        private readonly NavigationStore _navigationStore;
        public App()
        {
            _navigationStore = new NavigationStore();
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
            ConsoleHelper.Initialize();
            _navigationStore.CurrentViewModel = new StartupViewModel(_navigationStore, false);
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore)
            };
            MainWindow.Show();
            base.OnStartup(e);
            
        }
    }
}

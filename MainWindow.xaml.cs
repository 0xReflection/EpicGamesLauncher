using EpicGamesLauncher.Logger.EpicGamesLauncher.Logger;
using EpicGamesLauncher.Pages;
using EpicGamesLauncher.Repository;
using EpicGamesLauncher.Services;
using EpicGamesLauncher.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EpicGamesLauncher
{
    public partial class MainWindow : Window
    {
        private StoreViewModel _storeViewModel;
        private LibraryViewModel _libraryViewModel;
        public MainWindow()
        {
            InitializeComponent();
            InitializeViewModels();
            NavigateToStorePage();
        }

        private void InitializeViewModels()
        {
            var userRepository = new UserRepository();
            var gameRepository = new GameRepository();
            var genreRepository = new GenreRepository();
            var platformRepository = new PlatformRepository();
            var dlcRepository = new DLCRepository();
            var transactionRepository = new TransactionRepository();
            var entitlementRepository = new EntitlementRepository();


            var authLogger = new ConsoleLogger<AuthService>();
            var catalogLogger = new ConsoleLogger<CatalogService>();
            var orderLogger = new ConsoleLogger<OrderService>();
            var libraryLogger = new ConsoleLogger<LibraryService>();


            var authService = new AuthService(userRepository, authLogger);
            var catalogService = new CatalogService(gameRepository, genreRepository, platformRepository, dlcRepository, catalogLogger);
            var orderService = new OrderService(transactionRepository, entitlementRepository, userRepository, gameRepository, dlcRepository, orderLogger);
            var libraryService = new LibraryService(entitlementRepository, libraryLogger);


            _storeViewModel = new StoreViewModel(catalogService, orderService, libraryService, authService);
            _libraryViewModel = new LibraryViewModel(libraryService);


            _storeViewModel.NavigateToLibraryRequested += NavigateToLibraryPage;
            _libraryViewModel.NavigateToStoreRequested += NavigateToStorePage;
        }

        private void NavigateToStorePage()
        {
            var storePage = new StorePage { DataContext = _storeViewModel };
            MainFrame.Navigate(storePage);
        }

        private void NavigateToLibraryPage()
        {
            _libraryViewModel.RefreshData();
            var libraryPage = new StorePage { DataContext = _libraryViewModel };
            MainFrame.Navigate(libraryPage);
        }
    }
}
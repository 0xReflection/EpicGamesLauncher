using EpicGamesLauncher.CustomControls;
using EpicGamesLauncher.Models;
using EpicGamesLauncher.Services.Interfaces;
using EpicGamesLauncher.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

namespace EpicGamesLauncher.Pages
{

    public partial class StorePage : Page
    {
        private readonly StoreViewModel _viewModel;

        public StorePage(StoreViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.Games.CollectionChanged += Games_CollectionChanged;
            _viewModel.PurchaseSuccess += OnPurchaseSuccess;
            _viewModel.PurchaseFailed += OnPurchaseFailed;

            PopulateGames();
        }

        private void Games_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PopulateGames();
        }

        private void PopulateGames()
        {
            GamesWrapPanel.Children.Clear();

            foreach (var game in _viewModel.Games)
            {
                var card = new StoreItemCard
                {
                    DataContext = game,
                    Title = game.Title,
                    Developer = game.Developer?.Name,
                    Genre = string.Join(", ", game.Genres.Select(g => g.Name)),
                    Platform = string.Join(", ", game.Platforms.Select(p => p.Name)),
                    ImageSource = game.CoverImage,
                    Price = game.Price > 0 ? $"${game.Price:0.00}" : "Бесплатно",
                    GameId = game.GameId
                };

                card.PurchaseRequested += async (sender, e) =>
                {
                    if (sender is StoreItemCard card)
                        await _viewModel.PurchaseGameByIdAsync(card.GameId);
                };

                card.AddToLibraryRequested += async (sender, e) =>
                {
                    if (sender is StoreItemCard card)
                        await _viewModel.AddFreeGameToLibraryAsync(card.GameId);
                };

                GamesWrapPanel.Children.Add(card);
            }

            GamesWrapPanel.Children.Add(new StoreItemCard());
        }

        private void OnPurchaseSuccess(string message)
        {
            MessageBox.Show(message, "Вы удачно приобрели игру. Спасибо!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnPurchaseFailed(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Pages/SignInPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void BtnSettings_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Pages/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void BtnLibrary_OnClick(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.NavigateToLibraryPage();
        }
    }
}
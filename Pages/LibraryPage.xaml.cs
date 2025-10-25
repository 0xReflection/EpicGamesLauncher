using EpicGamesLauncher.CustomControls;
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
    /// <summary>
    /// Логика взаимодействия для LibraryPage.xaml
    /// </summary>
    public partial class LibraryPage : Page
    {
        private readonly LibraryViewModel _viewModel;
        public LibraryPage(LibraryViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.Games.CollectionChanged += Games_CollectionChanged;

            PopulateGames();
        }

        private void Games_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PopulateGames();
        }

        private void PopulateGames()
        {
            GamesWrapPanel.Children.Clear();

            if (_viewModel?.Games?.Count == 0)
            {
                EmptyLibraryMessage.Visibility = Visibility.Visible;
            }
            else
            {
                EmptyLibraryMessage.Visibility = Visibility.Collapsed;
            }

            if (_viewModel?.Games == null) return;

            foreach (var game in _viewModel.Games)
            {
                var card = new LibraryItemCard
                {
                    DataContext = game,
                    Title = game.Title,
                    Developer = game.Developer?.Name,
                    Genre = string.Join(", ", game.Genres.Select(g => g.Name)),
                    Platform = string.Join(", ", game.Platforms.Select(p => p.Name)),
                    ImageSource = game.CoverImage, // Убедитесь что это свойство заполнено
                    Price = "In Library",
                    GameId = game.GameId
                };

                // Добавьте отладку для проверки пути картинки
                if (string.IsNullOrEmpty(game.CoverImage))
                {
                    System.Diagnostics.Debug.WriteLine($"Game {game.Title} has no cover image");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Game {game.Title} cover: {game.CoverImage}");
                }

                GamesWrapPanel.Children.Add(card);
            }
        }



        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Pages/SignInPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void BtnSettings_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Pages/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
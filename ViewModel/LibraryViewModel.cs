using EpicGamesLauncher.Models;
using EpicGamesLauncher.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGamesLauncher.ViewModel
{
    public class LibraryViewModel : ViewModelBase
    {
        private readonly ILibraryService _libraryService;
        private ObservableCollection<Entitlement> _games;
        private Entitlement _selectedGame;
        private string _searchTerm;
        private bool _isLoading;
        private User _currentUser;

        public LibraryViewModel(ILibraryService libraryService)
        {
            _libraryService = libraryService;

            Games = new ObservableCollection<Entitlement>();
            LoadLibraryCommand = new AsyncCommand(LoadLibraryAsync);
            LaunchGameCommand = new AsyncCommand(LaunchSelectedGameAsync, () => CanLaunchGame);
            NavigateToStoreCommand = new AsyncCommand(NavigateToStore);

            CurrentUser = App.CurrentUser;

            _ = LoadLibraryAsync();
        }

        public ObservableCollection<Entitlement> Games
        {
            get => _games;
            set => SetProperty(ref _games, value);
        }

        public Entitlement SelectedGame
        {
            get => _selectedGame;
            set => SetProperty(ref _selectedGame, value);
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    _ = FilterGamesAsync();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public AsyncCommand LoadLibraryCommand { get; }
        public AsyncCommand LaunchGameCommand { get; }
        public AsyncCommand NavigateToStoreCommand { get; }

        public event Action<string> GameLaunching;
        public event Action<string> GameLaunched;
        public event Action<string> GameLaunchFailed;
        public event Action NavigateToStoreRequested;

        private async Task LoadLibraryAsync()
        {
            if (CurrentUser == null) return;

            try
            {
                IsLoading = true;
                var library = await _libraryService.GetUserLibraryAsync(CurrentUser.UserId);

                Games.Clear();
                foreach (var game in library)
                {
                    Games.Add(game);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading library: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task FilterGamesAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                await LoadLibraryAsync();
                return;
            }

            try
            {
                var library = await _libraryService.GetUserLibraryAsync(CurrentUser.UserId);
                var filteredGames = library.Where(g =>
                    g.Game.Title.IndexOf(SearchTerm, StringComparison.OrdinalIgnoreCase) >= 0);

                Games.Clear();
                foreach (var game in filteredGames)
                {
                    Games.Add(game);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error filtering library: {ex.Message}");
            }
        }

        private bool CanLaunchGame => !IsLoading && SelectedGame != null;

        private async Task LaunchSelectedGameAsync()
        {
            if (SelectedGame == null) return;

            try
            {
                IsLoading = true;
                GameLaunching?.Invoke($"Launching {SelectedGame.Game.Title}...");

                await Task.Delay(2000);

                GameLaunched?.Invoke($"{SelectedGame.Game.Title} launched successfully!");
            }
            catch (Exception ex)
            {
                GameLaunchFailed?.Invoke($"Failed to launch {SelectedGame.Game.Title}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private Task NavigateToStore()
        {
            NavigateToStoreRequested?.Invoke();
            return Task.CompletedTask;
        }
        public void RefreshData()
        {
            CurrentUser = App.CurrentUser;
            _ = LoadLibraryAsync();
        }
    }
}

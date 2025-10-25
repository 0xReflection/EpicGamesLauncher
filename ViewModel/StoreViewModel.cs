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
    public class StoreViewModel : ViewModelBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IOrderService _orderService;
        private readonly ILibraryService _libraryService;
        private readonly IAuthService _authService;

        private ObservableCollection<Game> _games;
        private ObservableCollection<Genre> _genres;
        private ObservableCollection<Platform> _platforms;
        private Game _selectedGame;
        private string _searchTerm;
        private string _selectedGenre;
        private bool _isLoading;
        private User _currentUser;

        public StoreViewModel(
            ICatalogService catalogService,
            IOrderService orderService,
            ILibraryService libraryService,
            IAuthService authService)
        {
            _catalogService = catalogService;
            _orderService = orderService;
            _libraryService = libraryService;
            _authService = authService;

            Games = new ObservableCollection<Game>();
            Genres = new ObservableCollection<Genre>();
            Platforms = new ObservableCollection<Platform>();

            LoadGamesCommand = new AsyncCommand(LoadGamesAsync);
            SearchGamesCommand = new AsyncCommand(SearchGamesAsync, () => !IsLoading);
            PurchaseGameCommand = new AsyncCommand(PurchaseSelectedGameAsync, () => CanPurchaseGame);
            ViewGameDetailsCommand = new AsyncCommand<Game>(ViewGameDetailsAsync);

            _ = InitializeAsync();
        }

        public ObservableCollection<Game> Games
        {
            get => _games;
            set => SetProperty(ref _games, value);
        }

        public ObservableCollection<Genre> Genres
        {
            get => _genres;
            set => SetProperty(ref _genres, value);
        }

        public ObservableCollection<Platform> Platforms
        {
            get => _platforms;
            set => SetProperty(ref _platforms, value);
        }

        public Game SelectedGame
        {
            get => _selectedGame;
            set => SetProperty(ref _selectedGame, value);
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set => SetProperty(ref _searchTerm, value);
        }

        public string SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                if (SetProperty(ref _selectedGenre, value))
                {
                    _ = FilterGamesByGenreAsync();
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
            set
            {
                if (SetProperty(ref _currentUser, value))
                {
                    OnPropertyChanged(nameof(UserBalance));
                    OnPropertyChanged(nameof(UserButtonText));
                }
            }
        }

        public decimal UserBalance => CurrentUser?.Balance ?? 0;

        public string UserButtonText => CurrentUser != null
                 ? $"{CurrentUser.Username} — ${CurrentUser.Balance:F2}"
                 : "Offline";

        public AsyncCommand LoadGamesCommand { get; }
        public AsyncCommand SearchGamesCommand { get; }
        public AsyncCommand PurchaseGameCommand { get; }
        public AsyncCommand<Game> ViewGameDetailsCommand { get; }


        public event Action<Game> GameSelected;
        public event Action<string> PurchaseSuccess;
        public event Action<string> PurchaseFailed;
        public event Action<string> ErrorOccurred;
        public event Action NavigateToLibraryRequested;
        public event Action UserButtonClicked;

        private async Task InitializeAsync()
        {
           
            CurrentUser = App.CurrentUser;

            await LoadGenresAsync();
            await LoadPlatformsAsync();
            await LoadGamesAsync();
        }

        private async Task LoadGamesAsync()
        {
            try
            {
                IsLoading = true;
                var games = await _catalogService.GetAllGamesAsync();

                Games.Clear();
                foreach (var game in games)
                {
                    Games.Add(game);
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Failed to load games");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadGenresAsync()
        {
            try
            {
                var genres = await _catalogService.GetAllGenresAsync();

                Genres.Clear();
                foreach (var genre in genres)
                {
                    Genres.Add(genre);
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Failed to load genres");
            }
        }

        private async Task LoadPlatformsAsync()
        {
            try
            {
                var platforms = await _catalogService.GetAllPlatformsAsync();

                Platforms.Clear();
                foreach (var platform in platforms)
                {
                    Platforms.Add(platform);
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Failed to load platforms");
            }
        }

        private async Task SearchGamesAsync()
        {
            try
            {
                IsLoading = true;
                var games = await _catalogService.SearchGamesAsync(SearchTerm);

                Games.Clear();
                foreach (var game in games)
                {
                    Games.Add(game);
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Failed to search games");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task FilterGamesByGenreAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedGenre))
                {
                    await LoadGamesAsync();
                    return;
                }

                IsLoading = true;
                var games = await _catalogService.GetGamesByGenreAsync(SelectedGenre);

                Games.Clear();
                foreach (var game in games)
                {
                    Games.Add(game);
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Failed to filter games by genre");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanPurchaseGame => !IsLoading && SelectedGame != null && CurrentUser != null;
        public async Task<bool> PurchaseGameByIdAsync(int gameId)
        {
            var game = Games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                SelectedGame = game;
                await PurchaseSelectedGameAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> AddFreeGameToLibraryAsync(int gameId)
        {
            try
            {
                var game = Games.FirstOrDefault(g => g.GameId == gameId);
                if (game != null && game.Price == 0)
                {
                    if (await _libraryService.HasGameAsync(CurrentUser.UserId, gameId))
                    {
                        PurchaseFailed?.Invoke("You already own this game");
                        return false;
                    }

                    await _libraryService.AddFreeGameToLibraryAsync(CurrentUser.UserId, gameId);
                    PurchaseSuccess?.Invoke($"Successfully added {game.Title} to library");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Failed to add game to library");
                return false;
            }
        }

        private async Task PurchaseSelectedGameAsync()
        {
            if (SelectedGame == null) return;

            try
            {
                IsLoading = true;
                if (await _libraryService.HasGameAsync(CurrentUser.UserId, SelectedGame.GameId))
                {
                    PurchaseFailed?.Invoke("You already own this game");
                    return;
                }

                var success = await _orderService.PurchaseGameAsync(CurrentUser.UserId, SelectedGame.GameId);
                if (success)
                {
                    var newBalance = await _authService.GetUserBalanceAsync(CurrentUser.UserId);
                    CurrentUser.Balance = newBalance;

                   
                    OnPropertyChanged(nameof(UserBalance));
                    OnPropertyChanged(nameof(UserButtonText)); 

                    PurchaseSuccess?.Invoke($"Successfully purchased {SelectedGame.Title}");
                }
                else
                {
                    PurchaseFailed?.Invoke("Purchase failed. Please check your balance and try again.");
                }
            }
            catch (Exception ex)
            {
                PurchaseFailed?.Invoke("Purchase failed. Please try again.");
            }
            finally
            {
                IsLoading = false;
            }
        }
        public void RefreshUserData()
        {
            CurrentUser = App.CurrentUser;
            
            OnPropertyChanged(nameof(UserBalance));
            OnPropertyChanged(nameof(UserButtonText));
        }

      
        public void UpdateUserBalance(decimal newBalance)
        {
            if (CurrentUser != null)
            {
                CurrentUser.Balance = newBalance;
                OnPropertyChanged(nameof(UserBalance));
                OnPropertyChanged(nameof(UserButtonText));
            }
        }
        private Task ViewGameDetailsAsync(Game game)
        {
            SelectedGame = game;
            GameSelected?.Invoke(game);
            return Task.CompletedTask;
        } 
    }
}

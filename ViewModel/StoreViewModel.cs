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
        private ObservableCollection<Game> _allGames;
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
            _allGames = new ObservableCollection<Game>();
            LoadGamesCommand = new AsyncCommand(LoadGamesAsync);
            SearchGamesCommand = new AsyncCommand(SearchGamesAsync, () => !IsLoading);
            PurchaseGameCommand = new AsyncCommand(PurchaseSelectedGameAsync, () => CanPurchaseGame);
            ViewGameDetailsCommand = new AsyncCommand<Game>(ViewGameDetailsAsync);
            ClearSearchCommand = new ViewModelCommand(ClearSearch);
            SearchGamesCommandSB = new AsyncCommand(SearchGamesAsyncSB, () => !IsLoading);
            ClearSearchCommand = new ViewModelCommand(ClearSearch);
            SelectedGenre = "All";
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
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                   
                    _ = ApplyFiltersAsync();
                }
            }
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
                 : "Оффлайн";
        public AsyncCommand SearchGamesCommand { get; }
        public AsyncCommand SearchGamesCommandSB { get; }
        public ViewModelCommand ClearSearchCommand { get; }
        public AsyncCommand LoadGamesCommand { get; }
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
                _allGames.Clear();
                Games.Clear();
                foreach (var game in games)
                {
                    Games.Add(game);
                    _allGames.Add(game);
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Ошибка загрузки информации об играх");
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
                ErrorOccurred?.Invoke("Ошибка загрузки жанров");
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
                ErrorOccurred?.Invoke("Ошибка загрузки платформ");
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
                ErrorOccurred?.Invoke("Ошибка поиска");
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
                if (string.IsNullOrEmpty(SelectedGenre) || SelectedGenre == "All")
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
        private async Task ApplyFiltersAsync()
        {
            try
            {
                IsLoading = true;
                IEnumerable<Game> filteredGames = _allGames;         
                if (!string.IsNullOrEmpty(SelectedGenre) && SelectedGenre != "All")
                {
                    filteredGames = filteredGames.Where(game =>
                        game.Genres.Any(g => g.Name == SelectedGenre));
                }
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    var searchLower = SearchTerm.ToLower();
                    filteredGames = filteredGames.Where(game =>
                        game.Title?.ToLower().Contains(searchLower) == true ||
                        game.Description?.ToLower().Contains(searchLower) == true ||
                        game.Developer?.Name?.ToLower().Contains(searchLower) == true ||
                        game.Publisher?.Name?.ToLower().Contains(searchLower) == true ||
                        game.Genres.Any(g => g.Name?.ToLower().Contains(searchLower) == true) ||
                        game.Platforms.Any(p => p.Name?.ToLower().Contains(searchLower) == true)
                    );
                }
                Games.Clear();
                foreach (var game in filteredGames)
                {
                    Games.Add(game);
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Failed to apply filters");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public ObservableCollection<string> GenreFilters { get; } = new ObservableCollection<string>
        {
        "All",
        "Action",
        "RPG",
        "Strategy",
        "Adventure",
        "Simulation",
        "Shooter",
        "MOBA",
        "Battle Royale",
        "Racing",
        "Sports",
        "Horror",
        "Puzzle",
        "Fighting"
        };
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
                        PurchaseFailed?.Invoke("Вы уже купили игру");
                        return false;
                    }

                    await _libraryService.AddFreeGameToLibraryAsync(CurrentUser.UserId, gameId);
                    PurchaseSuccess?.Invoke($"Игра {game.Title} успешно добавлена в библиотеку");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke("Ошибка приобретения ");
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
                    PurchaseFailed?.Invoke("У вас уже есть эта игра. Ошибка добавления");
                    return;
                }

                var success = await _orderService.PurchaseGameAsync(CurrentUser.UserId, SelectedGame.GameId);
                if (success)
                {
                    var newBalance = await _authService.GetUserBalanceAsync(CurrentUser.UserId);
                    CurrentUser.Balance = newBalance;

                   
                    OnPropertyChanged(nameof(UserBalance));
                    OnPropertyChanged(nameof(UserButtonText)); 

                    PurchaseSuccess?.Invoke($"{SelectedGame.Title} Успешно добавлена");
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
        private void ClearSearch(object parameter = null)
        {
            SearchTerm = string.Empty;
        }

        private async Task SearchGamesAsyncSB()
        {
            await ApplyFiltersAsync();
        }
    }
}

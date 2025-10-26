using EpicGamesLauncher.Models;
using EpicGamesLauncher.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace EpicGamesLauncher.ViewModel
{
    public class LibraryViewModel : ViewModelBase
    {
        private readonly ILibraryService _libraryService;
        private readonly IAuthService _authService;
        private ObservableCollection<Game> _games;
        private bool _isLoading;
        private User _currentUser;
        private string _selectedGenre;

        public LibraryViewModel(ILibraryService libraryService, IAuthService authService)
        {
            _libraryService = libraryService;
            _authService = authService;
            Games = new ObservableCollection<Game>();

            GenreFilters = new ObservableCollection<string>
            {
                "All",
                "action",
                "rpg",
                "strategy",
                "adventure",
                "simulation",
                "shooter",
                "moba",
                "battle-royale",
                "racing",
                "sports",
                "horror",
                "puzzle",
                "fighting"
            };

            LoadLibraryCommand = new AsyncCommand(LoadLibraryAsync);
            RefreshDataCommand = new AsyncCommand(RefreshData);
            NavigateToStoreCommand = new AsyncCommand(NavigateToStore);

            SelectedGenre = "All";
            _ = InitializeAsync();
        }

        public ObservableCollection<Game> Games
        {
            get => _games;
            set => SetProperty(ref _games, value);
        }

        public ObservableCollection<string> GenreFilters { get; }

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

        public AsyncCommand LoadLibraryCommand { get; }
        public AsyncCommand RefreshDataCommand { get; }
        public AsyncCommand NavigateToStoreCommand { get; }

        public event Action NavigateToStoreRequested;

        private async Task InitializeAsync()
        {
            await LoadCurrentUserAsync();
            await LoadLibraryAsync();
        }

        private async Task LoadCurrentUserAsync()
        {
            try
            {
                
                CurrentUser = await _authService.GetCurrentUserAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading current user: {ex.Message}");
                CurrentUser = null;
            }
        }

        public async Task LoadLibraryAsync()
        {
            try
            {
                IsLoading = true;

                if (CurrentUser == null)
                {
                    Games.Clear();
                    return;
                }

                var entitlements = await _libraryService.GetUserLibraryAsync(CurrentUser.UserId);

                Games.Clear();
                foreach (var entitlement in entitlements)
                {
                    if (entitlement.Game != null)
                    {
                        Games.Add(entitlement.Game);
                    }
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

        private async Task FilterGamesByGenreAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedGenre) || SelectedGenre == "All")
                {
                    await LoadLibraryAsync();
                    return;
                }

                IsLoading = true;

                if (CurrentUser == null)
                {
                    Games.Clear();
                    return;
                }

                var games = await _libraryService.GetGamesByGenreAsync(CurrentUser.UserId, SelectedGenre);

                Games.Clear();
                foreach (var game in games)
                {
                    Games.Add(game);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to filter games by genre: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
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

        public async Task RefreshData()
        {
            await LoadCurrentUserAsync();
            await LoadLibraryAsync();
        }

        public void RefreshUserData()
        {
            _ = LoadCurrentUserAsync();
        }

        public async Task UpdateUserBalanceAsync()
        {
            if (CurrentUser != null)
            {
                var newBalance = await _authService.GetUserBalanceAsync(CurrentUser.UserId);
                CurrentUser.Balance = newBalance;
                OnPropertyChanged(nameof(UserBalance));
                OnPropertyChanged(nameof(UserButtonText));
            }
        }

        public Task NavigateToStore()
        {
            NavigateToStoreRequested?.Invoke();
            return Task.CompletedTask;
        }
    }
}
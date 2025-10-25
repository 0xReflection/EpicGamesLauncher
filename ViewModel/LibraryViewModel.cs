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
        private ObservableCollection<Game> _games;
        private bool _isLoading;
        private User _currentUser;
        public LibraryViewModel(ILibraryService libraryService)
        {
            _libraryService = libraryService;
            Games = new ObservableCollection<Game>();

            LoadLibraryCommand = new AsyncCommand(LoadLibraryAsync);
            RefreshDataCommand = new AsyncCommand(RefreshData);
            NavigateToStoreCommand = new AsyncCommand(NavigateToStore);
            _ = InitializeAsync();
        }

        public ObservableCollection<Game> Games
        {
            get => _games;
            set => SetProperty(ref _games, value);
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
            await LoadLibraryAsync();
            CurrentUser = App.CurrentUser;
        }
      
        public async Task LoadLibraryAsync()
        {
            try
            {
                IsLoading = true;

                if (App.CurrentUser == null)
                {
                    Games.Clear();
                    return;
                }

                var entitlements = await _libraryService.GetUserLibraryAsync(App.CurrentUser.UserId);

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
            await LoadLibraryAsync();
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
        public Task NavigateToStore()
        {
            NavigateToStoreRequested?.Invoke();
            return Task.CompletedTask;
        }
    }
}
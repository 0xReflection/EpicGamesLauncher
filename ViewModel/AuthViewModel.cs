using EpicGamesLauncher.Logger.EpicGamesLauncher.Logger;
using EpicGamesLauncher.Models;
using EpicGamesLauncher.Models.Interfaces;
using EpicGamesLauncher.Repository;
using EpicGamesLauncher.Services;
using EpicGamesLauncher.Services.Interfaces;
using EpicGamesLauncher.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EpicGamesLauncher.ViewModel
{
    public class AuthViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private string _username;
        private string _email;
        private SecureString _password;
        private SecureString _confirmPassword;
        private bool _isLoginMode = true;
        private bool _isLoading;
        private string _errorMessage;

        public AuthViewModel()
        {
            var userRepository = new UserRepository();
            var logger = new ConsoleLogger<AuthService>();
            _authService = new AuthService(userRepository, logger);

            InitializeCommands();
        }

        public AuthViewModel(IAuthService authService)
        {
            _authService = authService;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            LoginCommand = new AsyncCommand(LoginAsync, () => CanLogin);
            RegisterCommand = new AsyncCommand(RegisterAsync, () => CanRegister);
            SwitchModeCommand = new AsyncCommand(SwitchMode);
            InitImages();
        }

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    RaiseCanExecuteChanged();
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    RaiseCanExecuteChanged();
                }
            }
        }

        public SecureString Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged();
            }
        }

        public SecureString ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged();
            }
        }


        public bool IsLoginMode
        {
            get => _isLoginMode;
            set
            {
                if (SetProperty(ref _isLoginMode, value))
                {
                    RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    RaiseCanExecuteChanged();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        public ObservableCollection<string> ListImages1 { get; set; }
        public ObservableCollection<string> ListImages2 { get; set; }
        public ObservableCollection<string> ListImages3 { get; set; }
        public ObservableCollection<string> ListImages4 { get; set; }
        public ObservableCollection<string> ListImages5 { get; set; }
        public ObservableCollection<string> ListImages6 { get; set; }
        public ObservableCollection<string> ListImages7 { get; set; }
        public ObservableCollection<string> ListImages8 { get; set; }
        public ObservableCollection<string> ListImages9 { get; set; }


        public static readonly ReadOnlyObservableCollection<string> CoverImageUrls =
            new(new ObservableCollection<string>
            {
              "https://cdn.cloudflare.steamstatic.com/steam/apps/1174180/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/570/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/730/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/271590/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/578080/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/292030/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/359550/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/252950/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/1091500/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/582010/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/812140/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/1172470/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/239140/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/620/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/413150/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/4000/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/236850/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/220/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/105600/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/550/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/620980/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/374320/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/413410/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/239030/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/379720/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/582160/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/883710/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/72850/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/992300/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/1113000/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/892970/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/252490/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/10180/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/242760/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/814380/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/881100/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/108600/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/548430/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/289070/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/311210/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/486560/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/218620/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/227300/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/12210/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/200510/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/205100/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/39210/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/594650/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/524220/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/440/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/476600/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/253030/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/208580/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/108710/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/10080/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/552500/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/268910/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/381210/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/739630/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/447820/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/1260/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/256290/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/268500/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/11020/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/33930/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/107410/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/43110/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/201870/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/431960/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/50300/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/41070/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/34870/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/945360/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/500/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/251570/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/686810/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/359320/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/454650/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/418370/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/427520/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/444090/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/252330/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/222880/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/206420/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/287390/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/388180/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/232790/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/211820/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/221100/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/485460/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/47890/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/460930/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/267530/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/371660/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/243870/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/24740/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/281990/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/247020/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/228380/header.jpg",
                "https://cdn.cloudflare.steamstatic.com/steam/apps/108600/header.jpg"
            });
        public AsyncCommand LoginCommand { get; private set; }
        public AsyncCommand RegisterCommand { get; private set; }
        public AsyncCommand SwitchModeCommand { get; private set; }

        public event Action<User> AuthenticationSuccess;
        public event Action<string> AuthenticationFailed;

        private bool CanLogin => !IsLoading &&
                          !string.IsNullOrWhiteSpace(Username) &&
                          Password != null && Password.Length > 0;

        private bool CanRegister => !IsLoading &&
                                  !string.IsNullOrWhiteSpace(Username) &&
                                  !string.IsNullOrWhiteSpace(Email) &&
                                  Password != null && Password.Length > 0 &&
                                  ConfirmPassword != null && ConfirmPassword.Length > 0;

        private void RaiseCanExecuteChanged()
        {
            LoginCommand?.RaiseCanExecuteChanged();
            RegisterCommand?.RaiseCanExecuteChanged();
        }

        private async Task LoginAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                RaiseCanExecuteChanged();

                string password = SecureStringToString(Password);

                var user = await _authService.LoginAsync(Username, password);
                if (user != null)
                {
                    App.CurrentUser = user;
                    AuthenticationSuccess?.Invoke(user);
                }
                else
                {
                    ErrorMessage = "Неверный логин или пароль";
                    AuthenticationFailed?.Invoke(ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ошибка входа. Попробуйте еще раз";
                AuthenticationFailed?.Invoke(ErrorMessage);
            }
            finally
            {
                IsLoading = false;
                RaiseCanExecuteChanged();
            }
        }

        private async Task RegisterAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                RaiseCanExecuteChanged();

                
                string password = SecureStringToString(Password);

                var success = await _authService.RegisterAsync(Username, Email, password);
                if (success)
                {
                    var user = await _authService.LoginAsync(Username, password);
                    if (user != null)
                    {
                        App.CurrentUser = user;
                        AuthenticationSuccess?.Invoke(user);
                    }
                    else
                    {
                        ErrorMessage = "Registration successful but login failed. Please try logging in.";
                        AuthenticationFailed?.Invoke(ErrorMessage);
                    }
                }
                else
                {
                    ErrorMessage = "Registration failed. Username or email may already exist.";
                    AuthenticationFailed?.Invoke(ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Registration failed. Please try again.";
                AuthenticationFailed?.Invoke(ErrorMessage);
            }
            finally
            {
                IsLoading = false;
                RaiseCanExecuteChanged();
            }
        }
        private string SecureStringToString(SecureString secureString)
        {
            if (secureString == null)
                return string.Empty;

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }

        private void InitImages()
        {
            var temp = CoverImageUrls.OrderBy(_ => Guid.NewGuid()).Distinct().ToList();

            ListImages1 = new ObservableCollection<string>(temp.Take(10).ToList());
            ListImages2 = new ObservableCollection<string>(temp.Skip(10).Take(10).ToList());
            ListImages3 = new ObservableCollection<string>(temp.Skip(20).Take(10).ToList());
            ListImages4 = new ObservableCollection<string>(temp.Skip(30).Take(10).ToList());
            ListImages5 = new ObservableCollection<string>(temp.Skip(40).Take(10).ToList());
            ListImages6 = new ObservableCollection<string>(temp.Skip(50).Take(10).ToList());
            ListImages7 = new ObservableCollection<string>(temp.Skip(60).Take(10).ToList());
            ListImages8 = new ObservableCollection<string>(temp.Skip(70).Take(10).ToList());
            ListImages9 = new ObservableCollection<string>(temp.Skip(80).Take(10).ToList());

            OnPropertyChanged(nameof(ListImages1));
            OnPropertyChanged(nameof(ListImages2));
            OnPropertyChanged(nameof(ListImages3));
            OnPropertyChanged(nameof(ListImages4));
            OnPropertyChanged(nameof(ListImages5));
            OnPropertyChanged(nameof(ListImages6));
            OnPropertyChanged(nameof(ListImages7));
            OnPropertyChanged(nameof(ListImages8));
            OnPropertyChanged(nameof(ListImages9));
        }


        private Task SwitchMode()
        {
            IsLoginMode = !IsLoginMode;
            ErrorMessage = string.Empty;
            Password?.Dispose();
            ConfirmPassword?.Dispose();
            Password = new SecureString();
            ConfirmPassword = new SecureString();

            RaiseCanExecuteChanged();
            return Task.CompletedTask;
        }
    }
}
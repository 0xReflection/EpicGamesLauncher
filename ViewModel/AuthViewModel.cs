using EpicGamesLauncher.Models.Interfaces;
using EpicGamesLauncher.Repository;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Windows.Input;

namespace EpicGamesLauncher.ViewModel
{
    public class AuthViewModel : ViewModelBase
    {
        private string _username;
        private SecureString _password;
        private string _errorMessage;
        private bool _isViewVisible = true;

        private IClientRepository userRepository;
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

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public SecureString Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public bool IsViewVisible
        {
            get => _isViewVisible;
            set { _isViewVisible = value; OnPropertyChanged(nameof(IsViewVisible)); }
        }
        public ICommand LoginCommand { get; }
        public ICommand RecoverPasswordCommand { get; }

        public AuthViewModel()
        {
            userRepository = new clientRepository();
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            RecoverPasswordCommand = new ViewModelCommand(p => ExecuteRecoverPassCommand("", ""));

            InitImages();
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

        private bool CanExecuteLoginCommand(object obj)
        {
            bool validData;
            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3 ||
                Password == null || Password.Length < 3)
                validData = false;
            else
                validData = true;
            return validData;
        }

        private void ExecuteLoginCommand(object obj)
        {
            var isValidUser = userRepository.AuthenticateUser(new NetworkCredential(Username, Password));
            if (isValidUser)
            {
                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(Username), null);
                IsViewVisible = false;
               
            }
            else
            {
                ErrorMessage = "*Неверный логин или пароль, попробуйте ещё раз.";
            }
        }

        private void ExecuteRecoverPassCommand(string username, string email)
        {
            throw new NotImplementedException();
        }
    }
}

using EpicGamesLauncher.Models;
using EpicGamesLauncher.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EpicGamesLauncher
{

    public partial class AuthWindow : Window, INotifyPropertyChanged
    {
        private AuthViewModel _authViewModel;

        public AuthWindow()
        {
            InitializeComponent();
            _authViewModel = new AuthViewModel();
            _authViewModel.AuthenticationSuccess += OnAuthenticationSuccess;
            _authViewModel.AuthenticationFailed += OnAuthenticationFailed;

            DataContext = _authViewModel;
        }

        private void OnAuthenticationSuccess(Models.User user)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void OnAuthenticationFailed(string errorMessage)
        {
            System.Diagnostics.Debug.WriteLine($"Authentication failed: {errorMessage}");
        }

        private void AuthWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

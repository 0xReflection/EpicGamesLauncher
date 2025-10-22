using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace EpicGamesLauncher.Pages
{
    /// <summary>
    /// Interaction logic for LoadingPage.xaml
    /// </summary>
    public partial class LoadingPage : Page
    {
        readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        public LoadingPage()
        {
            InitializeComponent();

            // Не запускать в режиме дизайна
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            _dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;

            // Запускать таймер после загрузки страницы — гарантирует, что NavigationService доступен
            Loaded += LoadingPage_Loaded;
        }

        private void LoadingPage_Loaded(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Start();
            Loaded -= LoadingPage_Loaded;
        }

        private void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _dispatcherTimer.Stop();

            if (this.NavigationService != null)
            {
                this.NavigationService.Navigate(new Uri("Pages/StorePage.xaml", UriKind.RelativeOrAbsolute));
                return;
            }
        }
    }
}


using System;
using System.Collections.Generic;
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

namespace EpicGamesLauncher.CustomControls
{
  
    public partial class SearchBox : UserControl
    {
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SearchBox),
                new PropertyMetadata(string.Empty, OnSearchTextChanged));

        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register("SearchCommand", typeof(ICommand), typeof(SearchBox));

        public static readonly DependencyProperty SearchDelayProperty =
            DependencyProperty.Register("SearchDelay", typeof(int), typeof(SearchBox),
                new PropertyMetadata(300));

        private DispatcherTimer _searchTimer;
        public SearchBox()
        {
            InitializeComponent();
            InitializeSearchTimer();
        }

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }

        public ICommand SearchCommand
        {
            get => (ICommand)GetValue(SearchCommandProperty);
            set => SetValue(SearchCommandProperty, value);
        }

        public int SearchDelay
        {
            get => (int)GetValue(SearchDelayProperty);
            set => SetValue(SearchDelayProperty, value);
        }

        private void InitializeSearchTimer()
        {
            _searchTimer = new DispatcherTimer();
            _searchTimer.Tick += OnSearchTimerTick;
            _searchTimer.Interval = TimeSpan.FromMilliseconds(SearchDelay);
        }

        private static void OnSearchTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = d as SearchBox;
            searchBox?.OnSearchTextChanged(e);
        }

        private void OnSearchTextChanged(DependencyPropertyChangedEventArgs e)
        {
          
            _searchTimer.Stop();

            if (string.IsNullOrEmpty(SearchText))
            {
                ExecuteSearch();
                return;
            }

            _searchTimer.Interval = TimeSpan.FromMilliseconds(SearchDelay);
            _searchTimer.Start();
        }

        private void OnSearchTimerTick(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            ExecuteSearch();
        }

        private void ExecuteSearch()
        {
            if (SearchCommand?.CanExecute(SearchText) == true)
            {
                SearchCommand.Execute(SearchText);
            }

            OnSearchExecuted?.Invoke(this, SearchText);
        }

        public event EventHandler<string> OnSearchExecuted;

        public void PerformSearch()
        {
            _searchTimer.Stop();
            ExecuteSearch();
        }
        public void ClearSearch()
        {
            SearchText = string.Empty;
            _searchTimer.Stop();
            ExecuteSearch();
        }
        private void OnClearButtonClick(object sender, RoutedEventArgs e)
        {
            ClearSearch();
        }

        
        public static readonly DependencyProperty ClearSearchCommandProperty =
            DependencyProperty.Register("ClearSearchCommand", typeof(ICommand), typeof(SearchBox));

        public ICommand ClearSearchCommand
        {
            get => (ICommand)GetValue(ClearSearchCommandProperty);
            set => SetValue(ClearSearchCommandProperty, value);
        }
    }
}
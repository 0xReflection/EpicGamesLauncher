using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace EpicGamesLauncher.CustomControls
{
    /// <summary>
    /// Interaction logic for LibraryItemCard.xaml
    /// </summary>
    public partial class LibraryItemCard : UserControl
    {
        public LibraryItemCard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(
        "ImageSource",
        typeof(string),
        typeof(LibraryItemCard),
        new PropertyMetadata(string.Empty));

        public string ImageSource
        {
            get => (string)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(LibraryItemCard), new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty DeveloperProperty =
            DependencyProperty.Register("Developer", typeof(string), typeof(LibraryItemCard), new PropertyMetadata(string.Empty));

        public string Developer
        {
            get => (string)GetValue(DeveloperProperty);
            set => SetValue(DeveloperProperty, value);
        }

        public static readonly DependencyProperty GenreProperty =
            DependencyProperty.Register("Genre", typeof(string), typeof(LibraryItemCard), new PropertyMetadata(string.Empty));

        public string Genre
        {
            get => (string)GetValue(GenreProperty);
            set => SetValue(GenreProperty, value);
        }

        public static readonly DependencyProperty PlatformProperty =
            DependencyProperty.Register("Platform", typeof(string), typeof(LibraryItemCard), new PropertyMetadata(string.Empty));

        public string Platform
        {
            get => (string)GetValue(PlatformProperty);
            set => SetValue(PlatformProperty, value);
        }
        public static readonly DependencyProperty PriceProperty =
      DependencyProperty.Register("Price", typeof(string), typeof(LibraryItemCard), new PropertyMetadata(string.Empty));

        public string Price
        {
            get => (string)GetValue(PriceProperty);
            set => SetValue(PriceProperty, value);
        }
        public static readonly DependencyProperty GameIdProperty =
          DependencyProperty.Register("GameId", typeof(int), typeof(LibraryItemCard),
              new PropertyMetadata(0));

        public int GameId
        {
            get => (int)GetValue(GameIdProperty);
            set => SetValue(GameIdProperty, value);
        }
    
    }
}
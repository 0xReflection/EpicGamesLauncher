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
    public partial class StoreItemCard : UserControl
    {
        public StoreItemCard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(
        "ImageSource",
        typeof(string),
        typeof(StoreItemCard),
        new PropertyMetadata(string.Empty));

        public string ImageSource
        {
            get => (string)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(StoreItemCard), new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty DeveloperProperty =
            DependencyProperty.Register("Developer", typeof(string), typeof(StoreItemCard), new PropertyMetadata(string.Empty));

        public string Developer
        {
            get => (string)GetValue(DeveloperProperty);
            set => SetValue(DeveloperProperty, value);
        }

        public static readonly DependencyProperty GenreProperty =
            DependencyProperty.Register("Genre", typeof(string), typeof(StoreItemCard), new PropertyMetadata(string.Empty));

        public string Genre
        {
            get => (string)GetValue(GenreProperty);
            set => SetValue(GenreProperty, value);
        }

        public static readonly DependencyProperty PlatformProperty =
            DependencyProperty.Register("Platform", typeof(string), typeof(StoreItemCard), new PropertyMetadata(string.Empty));

        public string Platform
        {
            get => (string)GetValue(PlatformProperty);
            set => SetValue(PlatformProperty, value);
        }
        public static readonly DependencyProperty PriceProperty =
      DependencyProperty.Register("Price", typeof(string), typeof(StoreItemCard), new PropertyMetadata(string.Empty));

        public string Price
        {
            get => (string)GetValue(PriceProperty);
            set => SetValue(PriceProperty, value);
        }
        public static readonly DependencyProperty GameIdProperty =
          DependencyProperty.Register("GameId", typeof(int), typeof(StoreItemCard),
              new PropertyMetadata(0));

        public int GameId
        {
            get => (int)GetValue(GameIdProperty);
            set => SetValue(GameIdProperty, value);
        }

        public bool IsFree => IsPriceFree(Price);

        public event RoutedEventHandler PurchaseRequested;
        public event RoutedEventHandler AddToLibraryRequested;

        private static void OnPriceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as StoreItemCard;
            card?.UpdateActionButtonText();
        }

        private void MoreOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            MoreOptionsPopup.IsOpen = true;
            UpdateActionButtonText();
            e.Handled = true;
        }

        private void UpdateActionButtonText()
        {
            if (IsFree)
            {
                ActionButton.Content = "Add to library";
            }
            else
            {
                ActionButton.Content = $"Buy - {Price}";
            }
        }

        private bool IsPriceFree(string price)
        {
            if (string.IsNullOrEmpty(price))
                return true;

         
            var lowerPrice = price.ToLowerInvariant();

           
            if (lowerPrice == "free" || lowerPrice == "бесплатно" ||
                lowerPrice == "0" || lowerPrice == "0.00" ||
                lowerPrice == "$0" || lowerPrice == "$0.00" ||
                lowerPrice == "0 руб" || lowerPrice == "0.00 руб")
            {
                return true;
            }

          
            if (decimal.TryParse(price, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result))
            {
                return result == 0;
            }

         
            var cleanPrice = new string(price.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
            if (decimal.TryParse(cleanPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal cleanResult))
            {
                return cleanResult == 0;
            }

            return false;
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            MoreOptionsPopup.IsOpen = false;

            if (IsFree)
            {
               
                AddToLibraryRequested?.Invoke(this, e);
           
            }
            else
            {
                PurchaseRequested?.Invoke(this, e);
             
            }
        }
    }
}
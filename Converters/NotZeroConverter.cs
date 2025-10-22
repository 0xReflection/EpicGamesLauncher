using System;
using System.Globalization;
using System.Windows.Data;

namespace EpicGamesLauncher.Converters
{
    public class NotZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Handle null value
            if (value == null)
                return false;

            // Try to parse the value to double
            if (double.TryParse(value.ToString(), NumberStyles.Any, culture, out double val))
            {
                return Math.Abs(val) > 0.0;
            }

            return false; // Return false instead of null for binding purposes
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
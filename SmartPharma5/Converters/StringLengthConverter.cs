using System.Globalization;

namespace SmartPharma5.Converters
{
    public class StringLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                string formattedValue = decimalValue.ToString("N0", culture);
                return formattedValue.Length > 8; // Ajustez cette valeur selon vos besoins
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 
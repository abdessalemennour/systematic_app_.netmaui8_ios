using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Converters
{
    public class DecimalFormatter : IValueConverter
    {
        // Méthode pour convertir la valeur (de la source vers la vue)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                // On obtient le nombre de décimales de la valeur et on crée un format dynamique
                int decimalPlaces = BitConverter.GetBytes(decimal.GetBits(decimalValue)[3])[2];
                string format = "{0:F" + decimalPlaces + "}";
                return string.Format(format, decimalValue);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

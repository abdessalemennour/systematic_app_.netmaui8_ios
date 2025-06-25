using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Converters
{
    public class BoolToRowHeightConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Diagnostics.Debug.WriteLine($"Converter input value: {value} (type: {value?.GetType()})");
        
            if (value is bool isVisible)
            {
                return isVisible ? GridLength.Auto : new GridLength(0);
            }
        
            // Fallback si le binding échoue
            return GridLength.Auto;
        }
        // ...


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

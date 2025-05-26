using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Converters
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BindableObject bindableObject && bindableObject.BindingContext is CollectionView collectionView)
            {
                var itemsSource = collectionView.ItemsSource.Cast<object>().ToList();
                return itemsSource.IndexOf(bindableObject.BindingContext);
            }
            return -1; // Retourne -1 si l'index n'est pas trouvé
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

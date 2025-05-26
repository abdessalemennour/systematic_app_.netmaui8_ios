using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Converters
{
    public class ReadStatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            if (value is bool isRead && parameter is int index && index == 0) // Index 0 = dernier message
            {
                return isRead ? "read_icon.png" : "unread_icon.png";
            }
            return null; // Aucune icône pour les autres messages
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    
}
}

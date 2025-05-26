using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.ModelView
{
    public class NotificationViewModel : INotifyPropertyChanged
    {
        private int _totalUnreadMessages;
        public int TotalUnreadMessages
        {
            get => _totalUnreadMessages;
            set
            {
                if (_totalUnreadMessages != value)
                {
                    _totalUnreadMessages = value;
                    Console.WriteLine($"[DEBUG] TotalUnreadMessages mis à jour : {_totalUnreadMessages}");
                    OnPropertyChanged(nameof(TotalUnreadMessages));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}

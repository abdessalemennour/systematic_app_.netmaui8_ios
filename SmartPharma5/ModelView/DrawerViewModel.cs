using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.ModelView
{
    public class DrawerViewModel : INotifyPropertyChanged
    {
        private string _headerText;
        private string _headerImage;
        private Color _contentBackgroundColor;
        private string _buttonText;

        public string HeaderText
        {
            get => _headerText;
            set
            {
                _headerText = value;
                OnPropertyChanged();
            }
        }

        public string HeaderImage
        {
            get => _headerImage;
            set
            {
                _headerImage = value;
                OnPropertyChanged();
            }
        }

        public Color ContentBackgroundColor
        {
            get => _contentBackgroundColor;
            set
            {
                _contentBackgroundColor = value;
                OnPropertyChanged();
            }
        }

        public string ButtonText
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

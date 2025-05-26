using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using SmartPharma5.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Globalization;
using Acr.UserDialogs;


namespace SmartPharma5.ModelView
{
    public class LogsViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<LogModel> _logs;
        public ObservableCollection<LogModel> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged();
            }
        }

        public LogsViewModel()
        {
        }


        private Task DisplayAlert(string v1, string v2, string v3)
        {
            throw new NotImplementedException();
        }

        public async Task LoadLogsAsync(int moduleId, string moduleType)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Chargement...");
                var logs = await LogModel.GetLogsAsync(moduleId, moduleType);
                Logs = new ObservableCollection<LogModel>(logs);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }
    




        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

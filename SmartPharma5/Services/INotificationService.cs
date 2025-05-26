using Acr.UserDialogs;
using SmartPharma5.Model;
using SmartPharma5.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPharma5.Services
{
    public interface INotificationService
    {
        void ShowMessageNotification(string senderName, string message);
        void Initialize();
        void SetUsers(List<UserModel> users);
    }

    public class NotificationService : INotificationService
    {
        private List<UserModel> _users = new();

        public void SetUsers(List<UserModel> users)
        {
            _users = users;
        }

        public void Initialize()
        {
            MessagingCenter.Subscribe<ChatViewModel, MessageModel>(
                this,
                "NewMessageReceived",
                (sender, message) =>
                {
                    var senderUser = _users.FirstOrDefault(u => u.Id == message.Sender);
                    var senderName = senderUser?.Login ?? "Inconnu";

                    ShowMessageNotification(senderName, message.Text);
                });
        }

        public void ShowMessageNotification(string senderName, string message)
        {
            // Cette méthode peut être utilisée pour d'autres types de notifications
            Device.BeginInvokeOnMainThread(() =>
            {
                UserDialogs.Instance.Toast(new ToastConfig($"Nouveau message de {senderName}: {message}")
                {
                    Duration = TimeSpan.FromSeconds(5),
                    Position = ToastPosition.Bottom,
                    BackgroundColor = System.Drawing.Color.Blue,
                    MessageTextColor = System.Drawing.Color.White
                });
            });
        }
    }
}

namespace SmartPharma5.View;

public partial class NotificationsProfilePartner : ContentPage
{
    public NotificationsProfilePartner()
    {
        InitializeComponent();
        BindingContext = new ViewModel.NotificationProfilePartnerMV();
    }
}
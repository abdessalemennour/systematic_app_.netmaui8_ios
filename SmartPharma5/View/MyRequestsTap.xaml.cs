namespace SmartPharma5.View;

public partial class MyRequestsTap : TabbedPage
{
	public MyRequestsTap()
	{
		InitializeComponent();
        BindingContext = new ViewModel.NotificationProfilePartnerMV("My");
      
    }
}